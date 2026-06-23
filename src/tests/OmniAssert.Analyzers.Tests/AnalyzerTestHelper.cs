using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using OmniAssert.Analyzers;
using System.Collections.Immutable;
using System.Reflection;

namespace OmniAssert.Analyzers.Tests;

internal static class AnalyzerTestHelper
{
    public static async Task<IReadOnlyList<Diagnostic>> GetOmniAssertAnalyzerDiagnosticsAsync(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest), path: "Test.cs");
        var refs = MetadataReferencesForOmniAssertConsumer().ToList();
        var compilation = CSharpCompilation.Create(
            "OmniAssertAnalyzersTestAsm",
            [tree],
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        if (errors.Count > 0)
        {
            throw new InvalidOperationException(string.Join(Environment.NewLine, errors));
        }

        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new OmniAssertDiagnosticAnalyzer());
        var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
        var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
        return diagnostics.Where(d => d.Id.StartsWith("OA", StringComparison.Ordinal)).ToList();
    }

    private static IEnumerable<MetadataReference> MetadataReferencesForOmniAssertConsumer()
    {
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        void Collect(Assembly asm)
        {
            if (string.IsNullOrEmpty(asm.Location) || !paths.Add(asm.Location))
                return;

            foreach (var name in asm.GetReferencedAssemblies())
            {
                try
                {
                    Collect(Assembly.Load(name));
                }
                catch
                {
                    // Ignore optional framework facades.
                }
            }
        }

        Collect(typeof(object).Assembly);
        Collect(typeof(Ensure).Assembly);

        foreach (var path in paths)
            yield return MetadataReference.CreateFromFile(path);
    }
}
