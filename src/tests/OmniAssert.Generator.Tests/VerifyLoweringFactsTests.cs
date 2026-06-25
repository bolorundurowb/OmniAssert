using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniAssert.Generator.Tests;

public class VerifyLoweringFactsTests
{
    [Theory]
    [InlineData("using OmniAssert;\npublic static class T { public static void M() { true.VerifyExpression(); } }", true)]
    [InlineData("using OmniAssert;\npublic static class T { public static void M() { true.VerifyExpression(\"hint\"); } }", true)]
    [InlineData("public static class T { public static void M() { OmniAssert.Assert.VerifyExpression(true); } }", true)]
    [InlineData("public static class T { public static void M() { OmniAssert.Ensure.VerifyExpression(true); } }", true)]
    [InlineData("public static class T { public static void M() { OmniAssert.Ensure.VerifyExpression(true, null); } }", true)]
    [InlineData("""
public static class T
{
    public static void M()
    {
        var x = new Decoy();
        x.VerifyExpression(true);
    }
    private sealed class Decoy { public void VerifyExpression(bool b) { } }
}
""", false)]
    [InlineData("""
namespace Other
{
    public static class Assert { public static void VerifyExpression(bool b) { } }
}
public static class T
{
    public static void M() { Other.Assert.VerifyExpression(true); }
}
""", false)]
    public void IsAssertVerifyExpression_MatchesExpected(string source, bool expected)
    {
        var tree = CSharpSyntaxTree.ParseText(source, path: "T.cs");
        var refs = VerifyExpansionEngineCompileTests.MetadataReferencesForOmniAssertConsumer().ToList();
        var compilation = CSharpCompilation.Create(
            "VerifyLoweringFactsAsm",
            [tree],
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        if (errors.Count > 0)
        {
            Xunit.Assert.Fail(string.Join("\n", errors.Select(e => e.ToString())));
        }

        var model = compilation.GetSemanticModel(tree);
        var invocation = tree.GetRoot().DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Single(i => i.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "VerifyExpression" }
                or IdentifierNameSyntax { Identifier.Text: "VerifyExpression" });

        var sym = model.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        Xunit.Assert.NotNull(sym);
        Xunit.Assert.Equal(expected, VerifyLoweringFacts.IsAssertVerifyExpression(sym!));
    }

    [Fact]
    public void IsAssertVerifyExpression_WhenZeroParameters_ReturnsFalse()
    {
        // Build a minimal compilation without the real OmniAssert assembly so we can define a
        // zero-parameter OmniAssert.Assert.VerifyExpression() decoy without type conflicts.
        var source = """
namespace OmniAssert
{
    public static class Assert { public static void VerifyExpression() { } }
}
public static class T
{
    public static void M() { OmniAssert.Assert.VerifyExpression(); }
}
""";
        var tree = CSharpSyntaxTree.ParseText(source, path: "T.cs");
        // Use only mscorlib / System.Runtime — no OmniAssert.Core.
        var refs = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };
        var compilation = CSharpCompilation.Create(
            "ZeroParamAsm",
            [tree],
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var model = compilation.GetSemanticModel(tree);
        var invocation = tree.GetRoot().DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Single(i => i.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "VerifyExpression" });

        var sym = model.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        Xunit.Assert.NotNull(sym);
        Xunit.Assert.False(VerifyLoweringFacts.IsAssertVerifyExpression(sym!));
    }

    [Theory]
    [InlineData("using OmniAssert;\npublic static class T { public static void M() { Ensure.Expression(true); } }", true, true)]
    [InlineData("using OmniAssert;\npublic static class T { public static void M(int x, int y) { Ensure.Expression(x > y); } }", true, true)]
    [InlineData("using OmniAssert;\npublic static class T { public static void M() { true.VerifyExpression(); } }", true, false)]
    [InlineData("""
namespace Other { public static class Ensure { public static void Expression(bool b) { } } }
public static class T { public static void M() { Other.Ensure.Expression(true); } }
""", false, false)]
    public void IsInterceptableBooleanExpression_MatchesExpected(string source, bool interceptable, bool isEnsureExpression)
    {
        var tree = CSharpSyntaxTree.ParseText(source, path: "T.cs");
        var refs = VerifyExpansionEngineCompileTests.MetadataReferencesForOmniAssertConsumer().ToList();
        var compilation = CSharpCompilation.Create(
            "InterceptableExpressionAsm",
            [tree],
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        if (errors.Count > 0)
        {
            Xunit.Assert.Fail(string.Join("\n", errors.Select(e => e.ToString())));
        }

        var model = compilation.GetSemanticModel(tree);
        var invocation = tree.GetRoot().DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Single(i => i.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "Expression" or "VerifyExpression" });

        var sym = model.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        Xunit.Assert.NotNull(sym);
        Xunit.Assert.Equal(interceptable, VerifyLoweringFacts.IsInterceptableBooleanExpression(sym!));
        Xunit.Assert.Equal(isEnsureExpression, VerifyLoweringFacts.IsEnsureExpression(sym!));
    }
}
