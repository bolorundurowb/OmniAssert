using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniAssert.Generator.Tests;

public class VerifyLoweringFactsTests
{
    [Theory]
    [InlineData("using static OmniAssert.Assert;\npublic static class T { public static void M() { VerifyExpression(true); } }", true)]
    [InlineData("using static OmniAssert.Assert;\npublic static class T { public static void M() { VerifyExpression(true, \"hint\"); } }", true)]
    [InlineData("public static class T { public static void M() { OmniAssert.Assert.VerifyExpression(true); } }", true)]
    [InlineData("public static class T { public static void M() { OmniAssert.Assert.VerifyExpression(true, null); } }", true)]
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
            new[] { tree },
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
}
