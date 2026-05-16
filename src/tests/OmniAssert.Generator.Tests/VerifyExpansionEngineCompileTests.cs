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
using static OmniAssert.Assert;
public static class T
{
    public static void M()
    {
        int x = 1;
        int y = 0;
        VerifyExpression(x > y);
    }
}
""";
        var syntaxTree = CSharpSyntaxTree.ParseText(source, path: "T.cs");
        var refs = MetadataReferencesForOmniAssertConsumer().ToList();

        var compilation = CSharpCompilation.Create(
            "VerifyExpansionTest",
            new[] { syntaxTree },
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var diagnostics = compilation.GetDiagnostics();
        Xunit.Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));

        var model = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var invocation = root.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Single(i => i.Expression is IdentifierNameSyntax { Identifier.Text: "VerifyExpression" });

        var engine = new VerifyExpansionEngine(model);
        var block = engine.TryExpandVerifyInvocation(invocation, default);
        Xunit.Assert.NotNull(block);
        Xunit.Assert.NotEmpty(block!.Statements);
    }

    [Fact]
    public void TryExpandVerifyInvocation_OrWithComparison_Compiles()
    {
        var source = """
using static OmniAssert.Assert;
public static class T
{
    public static void M()
    {
        int x = 2;
        int y = 3;
        int z = 10;
        VerifyExpression(z >= 10 || x > y);
    }
}
""";
        var syntaxTree = CSharpSyntaxTree.ParseText(source, path: "T.cs");
        var refs = MetadataReferencesForOmniAssertConsumer().ToList();

        var compilation = CSharpCompilation.Create(
            "VerifyExpansionOrTest",
            new[] { syntaxTree },
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var diagnostics = compilation.GetDiagnostics();
        Xunit.Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));

        var model = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var invocation = root.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Single(i => i.Expression is IdentifierNameSyntax { Identifier.Text: "VerifyExpression" });

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
            new[] { wrappedTree },
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
using static OmniAssert.Assert;
public static class T
{
    public static void M()
    {
        int x = 2;
        int y = 3;
        int z = 10;
        VerifyExpression(z > 10 || x > y);
    }
}
""";
        var syntaxTree = CSharpSyntaxTree.ParseText(source, path: "T.cs");
        var refs = MetadataReferencesForOmniAssertConsumer().ToList();
        var compilation = CSharpCompilation.Create(
            "VerifyExpansionLiteralTest",
            new[] { syntaxTree },
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        Xunit.Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));

        var model = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.GetRoot().DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Single(i => i.Expression is IdentifierNameSyntax { Identifier.Text: "VerifyExpression" });

        var block = new VerifyExpansionEngine(model).TryExpandVerifyInvocation(invocation, default);
        Xunit.Assert.NotNull(block);

        var text = block!.NormalizeWhitespace().ToFullString();
        Xunit.Assert.DoesNotContain("[\"10\"]", text, StringComparison.Ordinal);
        Xunit.Assert.Contains("[\"z\"]", text, StringComparison.Ordinal);
        Xunit.Assert.Contains("[\"x\"]", text, StringComparison.Ordinal);
        Xunit.Assert.Contains("[\"y\"]", text, StringComparison.Ordinal);
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
