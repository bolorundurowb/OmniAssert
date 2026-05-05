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

        // NormalizeWhitespace adds newlines and indentation between tokens so the emitted
        // source is readable and compiles without token-run-together errors.
        // Also drop the leading #line hidden and trailing #line default statements — those
        // are only useful when embedding generated code inside an interceptor body so the
        // debugger skips over it.  In a rewritten source file the generated code IS the
        // source, so the directives are both unnecessary and harmful.
        var normalized = (BlockSyntax)expanded.NormalizeWhitespace();
        expandedStatements = normalized.Statements.Where(s => !IsLineDirectiveStatement(s));
        return true;
    }

    private static bool IsLineDirectiveStatement(StatementSyntax s)
    {
        // ParseStatement("#line hidden") / ParseStatement("#line default") produce an
        // EmptyStatementSyntax whose leading trivia contains the directive text.
        var text = s.ToFullString().Trim();
        return text is "#line hidden" or "#line default" or "#line hidden;" or "#line default;";
    }
}
