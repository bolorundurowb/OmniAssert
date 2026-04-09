using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniAssert.Build;

internal sealed class VerifySyntaxRewriter : CSharpSyntaxRewriter
{
    private readonly SemanticModel _model;
    private readonly VerifyExpansionEngine _engine;
    private bool _changed;

    public VerifySyntaxRewriter(SemanticModel model)
    {
        _model = model;
        _engine = new VerifyExpansionEngine(model);
    }

    public bool Changed => _changed;

    public override SyntaxNode? VisitExpressionStatement(ExpressionStatementSyntax node)
    {
        if (node.Expression is InvocationExpressionSyntax inv &&
            IsAssertVerifyBoolean(inv, CancellationToken.None) &&
            _engine.TryExpandVerifyInvocation(inv, CancellationToken.None) is { } block)
        {
            _changed = true;
            return block;
        }

        return base.VisitExpressionStatement(node);
    }

    private bool IsAssertVerifyBoolean(InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
    {
        var sym = _model.GetSymbolInfo(invocation, cancellationToken).Symbol as IMethodSymbol;
        if (sym is null || sym.Name == "VerifyBoolean")
            return false;
        if (sym.Name != "Verify")
            return false;

        if (sym.ContainingType?.Name != "Assert" || sym.ContainingNamespace?.Name != "OmniAssert")
            return false;

        if (sym.Parameters.Length < 1 || sym.Parameters.Length > 2)
            return false;

        if (sym.Parameters[0].Type.SpecialType != SpecialType.System_Boolean)
            return false;

        if (sym.Parameters.Length == 2)
        {
            var t = sym.Parameters[1].Type;
            if (t.SpecialType != SpecialType.System_String && t.Name != "String" && t.ToDisplayString() != "string?")
                return false;
        }

        return true;
    }
}
