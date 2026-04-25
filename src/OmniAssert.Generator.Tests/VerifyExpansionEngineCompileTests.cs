using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniAssert.Generator.Rewrite;

namespace OmniAssert.Generator.Tests;

/// <summary>Ensures <see cref="OmniAssert.Generator.Rewrite.VerifyExpansionEngine.TryExpandVerifyInvocation"/> emits compilable syntax for a <c>VerifyExpression</c>-shaped call.</summary>
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

    private static IEnumerable<MetadataReference> MetadataReferencesForOmniAssertConsumer()
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
