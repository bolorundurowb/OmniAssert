using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniAssert.Generator.Rewrite;

namespace OmniAssert.Generator.Tests;

/// <summary>Unit tests for <see cref="VerifyExpansionEngine"/> that do not require a full compile round-trip.</summary>
public class VerifyExpansionEngineTests
{
    // IsSimpleBooleanIdentifier does not consult the SemanticModel, so any valid compilation is fine.
    private static VerifyExpansionEngine CreateEngine()
    {
        var tree = CSharpSyntaxTree.ParseText("public static class _Stub {}", path: "Stub.cs");
        var compilation = CSharpCompilation.Create("StubAsm", new[] { tree });
        return new VerifyExpansionEngine(compilation.GetSemanticModel(tree));
    }

    [Theory]
    [InlineData("flag", true)]
    [InlineData("(flag)", true)]
    [InlineData("((flag))", true)]
    [InlineData("x > 0", false)]
    [InlineData("!flag", false)]
    [InlineData("obj.Property", false)]
    [InlineData("a && b", false)]
    [InlineData("a || b", false)]
    public void IsSimpleBooleanIdentifier_ReturnsExpected(string exprText, bool expected)
    {
        var expr = SyntaxFactory.ParseExpression(exprText);
        var engine = CreateEngine();
        Xunit.Assert.Equal(expected, engine.IsSimpleBooleanIdentifier(expr));
    }

    [Fact]
    public void TryExpandVerifyInvocation_WhenNeitherMemberAccessNorArguments_ReturnsNull()
    {
        // Construct an invocation whose expression is a plain identifier (not MemberAccessExpression)
        // and whose argument list is empty.  This models an unreachable-in-practice code shape that
        // ensures the null-return guard fires.
        var invNode = SyntaxFactory.InvocationExpression(
            SyntaxFactory.IdentifierName("VerifyExpression"),
            SyntaxFactory.ArgumentList());

        var tree = CSharpSyntaxTree.ParseText("public static class _Stub {}", path: "Stub.cs");
        var compilation = CSharpCompilation.Create("StubAsm", new[] { tree });
        var model = compilation.GetSemanticModel(tree);
        var engine = new VerifyExpansionEngine(model);

        var result = engine.TryExpandVerifyInvocation(invNode, default);
        Xunit.Assert.Null(result);
    }

    [Fact]
    public void SkipParentheses_WhenTripleNested_ReturnsInnermostExpression()
    {
        var inner = SyntaxFactory.IdentifierName("x");
        var p1 = SyntaxFactory.ParenthesizedExpression(inner);
        var p2 = SyntaxFactory.ParenthesizedExpression(p1);
        var p3 = SyntaxFactory.ParenthesizedExpression(p2);

        var result = p3.SkipParentheses();
        Xunit.Assert.IsType<IdentifierNameSyntax>(result);
        Xunit.Assert.Equal("x", ((IdentifierNameSyntax)result).Identifier.Text);
    }

    [Fact]
    public void SkipParentheses_WhenNotParenthesized_ReturnsSameNode()
    {
        var expr = SyntaxFactory.IdentifierName("y");
        var result = expr.SkipParentheses();
        Xunit.Assert.Same(expr, result);
    }
}
