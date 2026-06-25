using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace OmniAssert.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class OmniAssertDiagnosticAnalyzer : DiagnosticAnalyzer
{
    public const string LegacyAssertId = "OA001";
    public const string LegacyVerifyId = "OA002";
    public const string LegacyFluentGrammarId = "OA003";
    public const string LegacyVerifyExpressionId = "OA004";

    private static readonly LocalizableString Title = "OmniAssert legacy API usage";
    private static readonly LocalizableString MessageFormat = "{0}";
    private static readonly LocalizableString Description = "Migrate to Ensure, Must(), Be*, and Expression syntax.";

    private static readonly DiagnosticDescriptor LegacyAssertRule = new(
        LegacyAssertId, Title, MessageFormat, "Usage", DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    private static readonly DiagnosticDescriptor LegacyVerifyRule = new(
        LegacyVerifyId, Title, MessageFormat, "Usage", DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    private static readonly DiagnosticDescriptor LegacyFluentGrammarRule = new(
        LegacyFluentGrammarId, Title, MessageFormat, "Usage", DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    private static readonly DiagnosticDescriptor LegacyVerifyExpressionRule = new(
        LegacyVerifyExpressionId, Title, MessageFormat, "Usage", DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        [LegacyAssertRule, LegacyVerifyRule, LegacyFluentGrammarRule, LegacyVerifyExpressionRule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeIdentifier, SyntaxKind.IdentifierName);
        context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
    }

    private static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not MemberAccessExpressionSyntax memberAccess)
            return;

        if (memberAccess.Expression is IdentifierNameSyntax { Identifier.ValueText: "Assert" })
        {
            var assertSymbol = context.SemanticModel.GetSymbolInfo(memberAccess, context.CancellationToken).Symbol;
            if (assertSymbol?.ContainingType.Name == "Assert" && IsOmniAssertNamespace(assertSymbol.ContainingNamespace))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    LegacyAssertRule,
                    memberAccess.Name.GetLocation(),
                    $"Use Ensure.{memberAccess.Name.Identifier.ValueText} instead of legacy Assert.{memberAccess.Name.Identifier.ValueText}."));
                if (memberAccess.Name.Identifier.ValueText != "VerifyExpression")
                    return;
            }
        }

        var name = memberAccess.Name.Identifier.ValueText;

        if (name == "VerifyExpression")
        {
            var symbol = context.SemanticModel.GetSymbolInfo(memberAccess, context.CancellationToken).Symbol;
            if (IsOmniAssertVerifyExpressionMethod(symbol))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    LegacyVerifyExpressionRule,
                    memberAccess.Name.GetLocation(),
                    "Use Ensure.Expression(...) instead of the legacy VerifyExpression()."));
            }

            return;
        }

        if (name == "Verify")
        {
            var symbol = context.SemanticModel.GetSymbolInfo(memberAccess, context.CancellationToken).Symbol;
            if (symbol is IMethodSymbol { Name: "Verify", ContainingType.Name: "Assert" or "Ensure" } method &&
                IsOmniAssertNamespace(method.ContainingNamespace))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    LegacyVerifyRule,
                    memberAccess.Name.GetLocation(),
                    "Use Must() instead of the legacy Verify() fluent entry point."));
            }

            return;
        }

        if (!name.StartsWith("To", StringComparison.Ordinal) &&
            !name.StartsWith("NotTo", StringComparison.Ordinal))
        {
            return;
        }

        if (!IsLegacyFluentAssertionName(name))
            return;

        if (!IsChainedAfterOmniAssertFluentRoot(memberAccess, context.SemanticModel, context.CancellationToken))
            return;

        context.ReportDiagnostic(Diagnostic.Create(
            LegacyFluentGrammarRule,
            memberAccess.Name.GetLocation(),
            $"Use {SuggestNewFluentName(name)}() instead of legacy {name}()."));
    }

    private static bool IsLegacyFluentAssertionName(string name) =>
        name is "ToBeTrue" or "ToBeFalse" or "ToBe" or "ToBeNull" or "ToNotBeNull" or "NotToBeNull" or "NotToBe" or "ToBeEmpty" or "NotToBeEmpty"
        || name.StartsWith("ToBe", StringComparison.Ordinal)
        || name.StartsWith("NotToBe", StringComparison.Ordinal)
        || name.StartsWith("ToHave", StringComparison.Ordinal)
        || name.StartsWith("ToContain", StringComparison.Ordinal)
        || name.StartsWith("ToMatch", StringComparison.Ordinal)
        || name.StartsWith("ToStart", StringComparison.Ordinal)
        || name.StartsWith("ToEnd", StringComparison.Ordinal)
        || name.StartsWith("ToEqual", StringComparison.Ordinal)
        || name.StartsWith("NotToEqual", StringComparison.Ordinal)
        || name.StartsWith("NotToEnd", StringComparison.Ordinal);

    internal static string SuggestNewFluentName(string legacyName)
    {
        if (legacyName.StartsWith("NotTo", StringComparison.Ordinal))
            return "Not" + legacyName.Substring("NotTo".Length);

        if (legacyName.StartsWith("To", StringComparison.Ordinal))
            return legacyName.Substring("To".Length);

        return legacyName;
    }

    private static bool IsChainedAfterOmniAssertFluentRoot(
        MemberAccessExpressionSyntax memberAccess,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        for (ExpressionSyntax? current = memberAccess.Expression; current is not null; current = current switch
        {
            MemberAccessExpressionSyntax ma => ma.Expression,
            InvocationExpressionSyntax inv when inv.Expression is ExpressionSyntax expr => expr,
            ParenthesizedExpressionSyntax p => p.Expression,
            _ => null
        })
        {
            if (current is InvocationExpressionSyntax invocation &&
                invocation.Expression is MemberAccessExpressionSyntax invokedMember)
            {
                var methodName = invokedMember.Name.Identifier.ValueText;
                if (methodName is "Verify" or "Must" or "VerifyNullable")
                {
                    var symbol = semanticModel.GetSymbolInfo(invokedMember, cancellationToken).Symbol;
                    if (symbol is IMethodSymbol method && IsOmniAssertNamespace(method.ContainingNamespace))
                        return true;
                }
            }

            if (current is MemberAccessExpressionSyntax member &&
                member.Name.Identifier.ValueText is "FileExists" or "DirectoryExists")
            {
                var symbol = semanticModel.GetSymbolInfo(member, cancellationToken).Symbol;
                if (symbol is IMethodSymbol method && IsOmniAssertNamespace(method.ContainingNamespace))
                    return true;
            }
        }

        return false;
    }

    private static void AnalyzeIdentifier(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not IdentifierNameSyntax identifier)
            return;

        if (identifier.Identifier.ValueText != "Assert")
            return;

        var symbol = context.SemanticModel.GetSymbolInfo(identifier, context.CancellationToken).Symbol;
        if (symbol is not INamedTypeSymbol typeSymbol)
            return;

        if (!IsOmniAssertAssert(typeSymbol))
            return;

        context.ReportDiagnostic(Diagnostic.Create(
            LegacyAssertRule,
            identifier.GetLocation(),
            "Use Ensure instead of the legacy Assert entry point."));
    }

    private static bool IsOmniAssertAssert(INamedTypeSymbol typeSymbol) =>
        typeSymbol.Name == "Assert" && IsOmniAssertNamespace(typeSymbol.ContainingNamespace);

    private static bool IsOmniAssertNamespace(INamespaceSymbol ns) =>
        ns.ToDisplayString() == "OmniAssert";

    private static bool IsOmniAssertVerifyExpressionMethod(ISymbol? symbol)
    {
        if (symbol is not IMethodSymbol method || method.Name != "VerifyExpression")
            return false;

        if (method.ContainingType.Name is not ("Assert" or "Ensure") || !IsOmniAssertNamespace(method.ContainingNamespace))
            return false;

        var original = method.ReducedFrom ?? method;
        return original.Parameters.Length >= 1 &&
               original.Parameters[0].Type.SpecialType == SpecialType.System_Boolean;
    }
}
