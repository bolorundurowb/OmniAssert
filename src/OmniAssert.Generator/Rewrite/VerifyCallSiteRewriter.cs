using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniAssert.Generator.Rewrite;

/// <summary>
/// Rewrites <c>VerifyExpression(expr)</c> call sites in a block by expanding them into the
/// full operand-capture form produced by <see cref="VerifyExpansionEngine.TryExpandVerifyInvocation"/>.
/// Intended to be applied to a source file that is supplied as an <c>AdditionalFile</c> so the
/// generator can emit the rewritten version in place of the original compilation unit.
/// </summary>
internal sealed class VerifyCallSiteRewriter(SemanticModel model, CancellationToken cancellationToken)
    : CSharpSyntaxRewriter
{
    private readonly VerifyExpansionEngine _engine = new(model);

    public bool Modified { get; private set; }

    public override SyntaxNode? VisitBlock(BlockSyntax node)
    {
        var visited = (BlockSyntax)base.VisitBlock(node)!;
        var newStatements = new List<StatementSyntax>();
        var changed = false;

        foreach (var stmt in visited.Statements)
        {
            if (TryExpand(stmt, out var expandedStatements))
            {
                newStatements.AddRange(expandedStatements!);
                changed = true;
                Modified = true;
                continue;
            }

            newStatements.Add(stmt);
        }

        return changed
            ? visited.WithStatements(SyntaxFactory.List(newStatements))
            : visited;
    }

    private bool TryExpand(StatementSyntax stmt, out IEnumerable<StatementSyntax>? expandedStatements)
    {
        expandedStatements = null;

        if (stmt is not ExpressionStatementSyntax { Expression: InvocationExpressionSyntax inv })
            return false;

        if (model.GetSymbolInfo(inv, cancellationToken).Symbol is not IMethodSymbol method)
            return false;

        if (!VerifyLoweringFacts.IsAssertVerifyExpression(method))
            return false;

        var expanded = _engine.TryExpandVerifyInvocation(inv, cancellationToken);
        if (expanded == null)
            return false;

        // Readable formatting; strip #line directives (they suit embedded interceptor bodies, not a standalone rewrite file).
        var normalized = (BlockSyntax)expanded.NormalizeWhitespace();
        expandedStatements = normalized.Statements.Where(s => !IsLineDirectiveStatement(s));
        return true;
    }

    private static bool IsLineDirectiveStatement(StatementSyntax s)
    {
        var text = s.ToFullString().Trim();
        return text is "#line hidden" or "#line default";
    }
}
