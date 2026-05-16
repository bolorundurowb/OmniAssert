using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniAssert.Generator.Rewrite;

namespace OmniAssert.Generator.Tests;

public class VerifyCallSiteRewriterTests
{
    [Fact]
    public void VisitBlock_WhenExpressionStatementIsVerifyExpression_ShouldExpandAndSetModified()
    {
        var source = """
using OmniAssert;
public static class T
{
    public static void M()
    {
        int x = 1;
        (x > 0).VerifyExpression();
    }
}
""";
        var tree = CSharpSyntaxTree.ParseText(source, path: "T.cs");
        var refs = VerifyExpansionEngineCompileTests.MetadataReferencesForOmniAssertConsumer().ToList();
        var compilation = CSharpCompilation.Create(
            "RewriterAsm",
            new[] { tree },
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        Xunit.Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));

        var model = compilation.GetSemanticModel(tree);
        var method = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.Text == "M");
        var body = method.Body!;

        var rewriter = new VerifyCallSiteRewriter(model, default);
        var rewritten = (BlockSyntax)rewriter.Visit(body)!;

        Xunit.Assert.True(rewriter.Modified);
        Xunit.Assert.True(rewritten.Statements.Count > body.Statements.Count);
        var lowered = rewritten.ToFullString();
        Xunit.Assert.Contains("AssertionCapture", lowered, StringComparison.Ordinal);
    }

    [Fact]
    public void VisitBlock_WhenNoVerifyExpression_ShouldLeaveBlockUnchanged()
    {
        var source = """
public static class T
{
    public static void M()
    {
        int x = 1;
        x++;
    }
}
""";
        var tree = CSharpSyntaxTree.ParseText(source, path: "T.cs");
        var refs = VerifyExpansionEngineCompileTests.MetadataReferencesForOmniAssertConsumer().ToList();
        var compilation = CSharpCompilation.Create(
            "RewriterNoVerifyAsm",
            new[] { tree },
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        Xunit.Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));

        var model = compilation.GetSemanticModel(tree);
        var method = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.Text == "M");
        var body = method.Body!;

        var rewriter = new VerifyCallSiteRewriter(model, default);
        var rewritten = (BlockSyntax)rewriter.Visit(body)!;

        Xunit.Assert.False(rewriter.Modified);
        Xunit.Assert.Equal(body.Statements.Count, rewritten.Statements.Count);
    }
}
