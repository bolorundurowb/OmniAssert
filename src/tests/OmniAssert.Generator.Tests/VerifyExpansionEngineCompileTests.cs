using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniAssert.Generator.Rewrite;

namespace OmniAssert.Generator.Tests;

public class VerifyExpansionEngineCompileTests
{
    [Fact]
    public void TryExpandVerifyInvocation_WhenEmbeddedInMethod_Compiles()
    {
        var source = """
using OmniAssert;
public static class T
{
    public static void M()
    {
        int x = 1;
        int y = 0;
        (x > y).VerifyExpression();
    }
}
""";
        var syntaxTree = CSharpSyntaxTree.ParseText(source, path: "T.cs");
        var refs = MetadataReferencesForOmniAssertConsumer().ToList();

        var compilation = CSharpCompilation.Create(
            "VerifyExpansionTest",
            [syntaxTree],
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var diagnostics = compilation.GetDiagnostics();
        Xunit.Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));

        var model = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var invocation = root.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Single(i => i.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "VerifyExpression" });

        var engine = new VerifyExpansionEngine(model);
        var block = engine.TryExpandVerifyInvocation(invocation, default);
        Xunit.Assert.NotNull(block);
        Xunit.Assert.NotEmpty(block!.Statements);
    }

    [Fact]
    public void TryExpandVerifyInvocation_OrWithComparison_Compiles()
    {
        var source = """
using OmniAssert;
public static class T
{
    public static void M()
    {
        int x = 2;
        int y = 3;
        int z = 10;
        (z >= 10 || x > y).VerifyExpression();
    }
}
""";
        var syntaxTree = CSharpSyntaxTree.ParseText(source, path: "T.cs");
        var refs = MetadataReferencesForOmniAssertConsumer().ToList();

        var compilation = CSharpCompilation.Create(
            "VerifyExpansionOrTest",
            [syntaxTree],
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var diagnostics = compilation.GetDiagnostics();
        Xunit.Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));

        var model = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var invocation = root.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Single(i => i.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "VerifyExpression" });

        var engine = new VerifyExpansionEngine(model);
        var block = engine.TryExpandVerifyInvocation(invocation, default);
        Xunit.Assert.NotNull(block);

        var lowered = ((BlockSyntax)block!.NormalizeWhitespace()).Statements
            .Where(s => s.ToFullString().Trim() is not "#line hidden" and not "#line default" and not "#line hidden;" and not "#line default;")
            .ToList();

        var body = string.Join(
            "\n",
            lowered.Select(s => s.ToFullString()));

        var wrapped = $@"using static OmniAssert.Assert;
public static class Wrapped
{{
    public static void M()
    {{
        int x = 2;
        int y = 3;
        int z = 10;
{body}
    }}
}}";

        var wrappedTree = CSharpSyntaxTree.ParseText(wrapped, path: "Wrapped.cs");
        var wrappedCompilation = CSharpCompilation.Create(
            "WrappedAsm",
            [wrappedTree],
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var wrappedDiags = wrappedCompilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        if (wrappedDiags.Count > 0)
        {
            var text = wrapped + "\n---\n" + string.Join("\n", wrappedDiags.Select(d => d.ToString()));
            Xunit.Assert.Fail(text);
        }
    }

    [Fact]
    public void TryExpandVerifyInvocation_DoesNotRecordLiteralOperandsInDictionary()
    {
        var source = """
using OmniAssert;
public static class T
{
    public static void M()
    {
        int x = 2;
        int y = 3;
        int z = 10;
        (z > 10 || x > y).VerifyExpression();
    }
}
""";
        var syntaxTree = CSharpSyntaxTree.ParseText(source, path: "T.cs");
        var refs = MetadataReferencesForOmniAssertConsumer().ToList();
        var compilation = CSharpCompilation.Create(
            "VerifyExpansionLiteralTest",
            [syntaxTree],
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        Xunit.Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));

        var model = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.GetRoot().DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Single(i => i.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "VerifyExpression" });

        var block = new VerifyExpansionEngine(model).TryExpandVerifyInvocation(invocation, default);
        Xunit.Assert.NotNull(block);

        var text = block!.NormalizeWhitespace().ToFullString();
        Xunit.Assert.DoesNotContain("[\"10\"]", text, StringComparison.Ordinal);
        Xunit.Assert.Contains("[\"z\"]", text, StringComparison.Ordinal);
        Xunit.Assert.Contains("[\"x\"]", text, StringComparison.Ordinal);
        Xunit.Assert.Contains("[\"y\"]", text, StringComparison.Ordinal);
    }

    [Fact]
    public void TryExpandVerifyInvocation_WithAndExpression_Compiles()
    {
        var source = """
using OmniAssert;
public static class T
{
    public static void M(int x, int y, int z)
    {
        (x > 0 && y < z).VerifyExpression();
    }
}
""";
        var syntaxTree = CSharpSyntaxTree.ParseText(source, path: "T.cs");
        var refs = MetadataReferencesForOmniAssertConsumer().ToList();
        var compilation = CSharpCompilation.Create(
            "VerifyExpansionAndTest",
            [syntaxTree],
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        Xunit.Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));

        var model = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.GetRoot().DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Single(i => i.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "VerifyExpression" });

        var block = new VerifyExpansionEngine(model).TryExpandVerifyInvocation(invocation, default);
        Xunit.Assert.NotNull(block);

        // AND short-circuit: both operands must be captured
        var text = block!.NormalizeWhitespace().ToFullString();
        Xunit.Assert.Contains("[\"x\"]", text, StringComparison.Ordinal);
        Xunit.Assert.Contains("[\"y\"]", text, StringComparison.Ordinal);
        Xunit.Assert.Contains("[\"z\"]", text, StringComparison.Ordinal);
    }

    [Fact]
    public void TryExpandVerifyInvocation_WithNegatedExpression_Compiles()
    {
        var source = """
using OmniAssert;
public static class T
{
    public static void M(bool flag)
    {
        (!flag).VerifyExpression();
    }
}
""";
        var syntaxTree = CSharpSyntaxTree.ParseText(source, path: "T.cs");
        var refs = MetadataReferencesForOmniAssertConsumer().ToList();
        var compilation = CSharpCompilation.Create(
            "VerifyExpansionNotTest",
            [syntaxTree],
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        Xunit.Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));

        var model = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.GetRoot().DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Single(i => i.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "VerifyExpression" });

        var block = new VerifyExpansionEngine(model).TryExpandVerifyInvocation(invocation, default);
        Xunit.Assert.NotNull(block);

        var text = block!.NormalizeWhitespace().ToFullString();
        // Negation should be present in the final expression
        Xunit.Assert.Contains("!", text, StringComparison.Ordinal);
        Xunit.Assert.Contains("[\"flag\"]", text, StringComparison.Ordinal);
    }

    [Fact]
    public void TryExpandVerifyInvocation_WithStaticCallForm_Compiles()
    {
        var source = """
public static class T
{
    public static void M(int x, int y)
    {
        OmniAssert.Assert.VerifyExpression(x > y);
    }
}
""";
        var syntaxTree = CSharpSyntaxTree.ParseText(source, path: "T.cs");
        var refs = MetadataReferencesForOmniAssertConsumer().ToList();
        var compilation = CSharpCompilation.Create(
            "VerifyExpansionStaticTest",
            [syntaxTree],
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        Xunit.Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));

        var model = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.GetRoot().DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Single(i => i.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "VerifyExpression" });

        var block = new VerifyExpansionEngine(model).TryExpandVerifyInvocation(invocation, default);
        Xunit.Assert.NotNull(block);
        Xunit.Assert.NotEmpty(block!.Statements);

        var text = block.NormalizeWhitespace().ToFullString();
        Xunit.Assert.Contains("[\"x\"]", text, StringComparison.Ordinal);
        Xunit.Assert.Contains("[\"y\"]", text, StringComparison.Ordinal);
    }

    [Fact]
    public void TryExpandVerifyInvocation_WithRepeatedOperand_DisambiguatesKeys()
    {
        // When the same sub-expression appears more than once (e.g. x > 0 && x < 10),
        // the second occurrence of "x" should get a disambiguated key like "x (2)".
        var source = """
using OmniAssert;
public static class T
{
    public static void M(int x)
    {
        (x > 0 && x < 10).VerifyExpression();
    }
}
""";
        var syntaxTree = CSharpSyntaxTree.ParseText(source, path: "T.cs");
        var refs = MetadataReferencesForOmniAssertConsumer().ToList();
        var compilation = CSharpCompilation.Create(
            "VerifyExpansionDupKeyTest",
            [syntaxTree],
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        Xunit.Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));

        var model = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.GetRoot().DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Single(i => i.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "VerifyExpression" });

        var block = new VerifyExpansionEngine(model).TryExpandVerifyInvocation(invocation, default);
        Xunit.Assert.NotNull(block);

        var text = block!.NormalizeWhitespace().ToFullString();
        Xunit.Assert.Contains("[\"x\"]", text, StringComparison.Ordinal);
        Xunit.Assert.Contains("[\"x (2)\"]", text, StringComparison.Ordinal);
    }

    internal static IEnumerable<MetadataReference> MetadataReferencesForOmniAssertConsumer()
    {
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        void Collect(Assembly asm)
        {
            if (string.IsNullOrEmpty(asm.Location) || !paths.Add(asm.Location))
                return;

            foreach (var na in asm.GetReferencedAssemblies())
            {
                try
                {
                    Collect(Assembly.Load(na));
                }
                catch
                {
                    // Ignore optional framework facades.
                }
            }
        }

        Collect(typeof(object).Assembly);
        Collect(typeof(OmniAssert.Assert).Assembly);

        foreach (var p in paths)
            yield return MetadataReference.CreateFromFile(p);
    }
}
