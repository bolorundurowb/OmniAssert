using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using OmniAssert.Analyzers;
using System.Collections.Immutable;
using System.Reflection;

namespace OmniAssert.Analyzers.Tests;

internal static class AnalyzerTestHelper
{
    public static async Task<IReadOnlyList<Diagnostic>> GetOmniAssertAnalyzerDiagnosticsAsync(string source)
    {
        var (compilation, tree) = await CreateCompilationAsync(source);

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

    public static async Task<string> ApplyCodeFixAsync(string source, string diagnosticId)
    {
        var (compilation, tree) = await CreateCompilationAsync(source);
        var document = CreateDocument(source);

        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new OmniAssertDiagnosticAnalyzer());
        var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
        var allDiagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
        var diagnostic = allDiagnostics.FirstOrDefault(d => d.Id == diagnosticId);
        if (diagnostic is null)
            throw new InvalidOperationException($"No diagnostic with id {diagnosticId} was reported. Available: {string.Join(", ", allDiagnostics.Select(d => d.Id))}");

        var fixProvider = new OmniAssertCodeFixProvider();
        var codeFixContext = new CodeFixContext(
            document,
            diagnostic,
            (action, _) => { },
            CancellationToken.None);

        var codeActions = new List<CodeAction>();
        var captureContext = new CodeFixContext(
            document,
            diagnostic,
            (action, _) => codeActions.Add(action),
            CancellationToken.None);

        await fixProvider.RegisterCodeFixesAsync(captureContext);
        if (codeActions.Count == 0)
            throw new InvalidOperationException("No code actions were registered.");
        var actionToApply = codeActions.FirstOrDefault(a => a.EquivalenceKey == diagnosticId)
            ?? codeActions.First();

        var operations = await actionToApply.GetOperationsAsync(CancellationToken.None);
        var applyOperation = operations.OfType<ApplyChangesOperation>().FirstOrDefault();
        if (applyOperation is null)
            throw new InvalidOperationException($"No ApplyChangesOperation. Operations: {string.Join(", ", operations.Select(o => o.GetType().Name))}");
        var changedDocument = applyOperation.ChangedSolution.GetDocument(document.Id)!;
        var changedRoot = await changedDocument.GetSyntaxRootAsync(CancellationToken.None);
        return changedRoot!.ToFullString();
    }

    private static async Task<(CSharpCompilation Compilation, SyntaxTree Tree)> CreateCompilationAsync(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest), path: "Test.cs");
        var refs = MetadataReferencesForOmniAssertConsumer().ToList();
        var compilation = CSharpCompilation.Create(
            "OmniAssertAnalyzersTestAsm",
            [tree],
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        return (compilation, tree);
    }

    private static Document CreateDocument(string source)
    {
        var refs = MetadataReferencesForOmniAssertConsumer().ToList();
        var adhocWorkspace = new AdhocWorkspace();
        var projectId = ProjectId.CreateNewId();
        var documentId = DocumentId.CreateNewId(projectId);
        var projectInfo = ProjectInfo.Create(
            projectId,
            VersionStamp.Create(),
            "OmniAssertAnalyzersTestAsm",
            "OmniAssertAnalyzersTestAsm",
            LanguageNames.CSharp,
            parseOptions: new CSharpParseOptions(LanguageVersion.Latest),
            metadataReferences: refs);
        var solution = adhocWorkspace.CurrentSolution.AddProject(projectInfo).AddDocument(documentId, "Test.cs", source);
        return solution.GetDocument(documentId)!;
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
