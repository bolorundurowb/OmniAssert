using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniAssert.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OmniAssertCodeFixProvider)), Shared]
public sealed class OmniAssertCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
    [
        OmniAssertDiagnosticAnalyzer.LegacyAssertId,
            OmniAssertDiagnosticAnalyzer.LegacyVerifyId,
            OmniAssertDiagnosticAnalyzer.LegacyFluentGrammarId,
            OmniAssertDiagnosticAnalyzer.LegacyVerifyExpressionId
    ];

    public sealed override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
            return;

        foreach (var diagnostic in context.Diagnostics)
        {
            if (!root.FindNode(diagnostic.Location.SourceSpan).DescendantNodesAndSelf().Any() &&
                root.FindNode(diagnostic.Location.SourceSpan) is null)
            {
                continue;
            }

            var node = root.FindNode(diagnostic.Location.SourceSpan);
            var title = diagnostic.Id switch
            {
                OmniAssertDiagnosticAnalyzer.LegacyVerifyExpressionId => "Migrate to Ensure.Expression(...)",
                _ => "Migrate to Ensure/Must/Be* syntax"
            };

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => ApplyFixAsync(context.Document, node, diagnostic.Id, c),
                    equivalenceKey: diagnostic.Id),
                diagnostic);
        }
    }

    private static async Task<Document> ApplyFixAsync(
        Document document,
        SyntaxNode node,
        string diagnosticId,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
            return document;

        SyntaxNode? newRoot = diagnosticId switch
        {
            OmniAssertDiagnosticAnalyzer.LegacyAssertId => ReplaceLegacyAssert(root, node),
            OmniAssertDiagnosticAnalyzer.LegacyVerifyId => ReplaceLegacyVerify(root, node),
            OmniAssertDiagnosticAnalyzer.LegacyFluentGrammarId => ReplaceLegacyFluentMethod(root, node),
            OmniAssertDiagnosticAnalyzer.LegacyVerifyExpressionId => ReplaceLegacyVerifyExpression(root, node),
            _ => null
        };

        return newRoot is null ? document : document.WithSyntaxRoot(newRoot);
    }

    private static SyntaxNode ReplaceLegacyAssert(SyntaxNode root, SyntaxNode node)
    {
        if (node is IdentifierNameSyntax identifier && identifier.Identifier.ValueText == "Assert")
            return root.ReplaceNode(identifier, identifier.WithIdentifier(SyntaxFactory.Identifier("Ensure")));

        if (node.Parent is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Expression is IdentifierNameSyntax assertId &&
            assertId.Identifier.ValueText == "Assert")
        {
            return root.ReplaceNode(
                assertId,
                assertId.WithIdentifier(SyntaxFactory.Identifier("Ensure")));
        }

        var found = FindIdentifier(node);
        if (found is not null && found.Identifier.ValueText == "Assert")
            return root.ReplaceNode(found, found.WithIdentifier(SyntaxFactory.Identifier("Ensure")));

        return root;
    }

    private static SyntaxNode ReplaceLegacyVerify(SyntaxNode root, SyntaxNode node)
    {
        if (node is IdentifierNameSyntax identifier && identifier.Identifier.ValueText == "Verify")
            return root.ReplaceNode(identifier, identifier.WithIdentifier(SyntaxFactory.Identifier("Must")));

        if (node.Parent is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Name.Identifier.ValueText == "Verify")
        {
            return root.ReplaceNode(
                memberAccess.Name,
                memberAccess.Name.WithIdentifier(SyntaxFactory.Identifier("Must")));
        }

        return root;
    }

    private static SyntaxNode ReplaceLegacyFluentMethod(SyntaxNode root, SyntaxNode node)
    {
        var identifier = node as IdentifierNameSyntax ?? FindIdentifier(node);
        if (identifier is null)
            return root;

        var legacyName = identifier.Identifier.ValueText;
        var newName = OmniAssertDiagnosticAnalyzer.SuggestNewFluentName(legacyName);
        return root.ReplaceNode(identifier, identifier.WithIdentifier(SyntaxFactory.Identifier(newName)));
    }

    private static SyntaxNode ReplaceLegacyVerifyExpression(SyntaxNode root, SyntaxNode node)
    {
        var invocation = node.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
        if (invocation?.Expression is not MemberAccessExpressionSyntax memberAccess ||
            memberAccess.Name.Identifier.ValueText != "VerifyExpression")
        {
            return root;
        }

        ExpressionSyntax condition;
        SeparatedSyntaxList<ArgumentSyntax> trailingArgs;

        if (invocation.ArgumentList.Arguments.Count == 0)
        {
            condition = memberAccess.Expression;
            trailingArgs = default;
        }
        else
        {
            condition = invocation.ArgumentList.Arguments[0].Expression;
            trailingArgs = invocation.ArgumentList.Arguments.Count > 1
                ? invocation.ArgumentList.Arguments.RemoveAt(0)
                : default;
        }

        var argumentNodes = new List<ArgumentSyntax> { SyntaxFactory.Argument(condition) };
        if (trailingArgs.Count > 0)
            argumentNodes.AddRange(trailingArgs);

        var newInvocation = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName("Ensure"),
                SyntaxFactory.IdentifierName("Expression")),
            SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(argumentNodes)));

        return root.ReplaceNode(invocation, newInvocation);
    }

    private static IdentifierNameSyntax? FindIdentifier(SyntaxNode node) =>
        node as IdentifierNameSyntax
        ?? node.AncestorsAndSelf().OfType<IdentifierNameSyntax>().FirstOrDefault();
}
