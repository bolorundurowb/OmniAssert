using System.Security.Cryptography;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace OmniAssert.Build;

public sealed class OmniAssertRewriteTask : Microsoft.Build.Utilities.Task
{
    [Required]
    public ITaskItem[] Compile { get; set; } = Array.Empty<ITaskItem>();

    public ITaskItem[] ReferencePath { get; set; } = Array.Empty<ITaskItem>();

    [Required]
    public string IntermediateOutputPath { get; set; } = "";

    [Required]
    public string OmniAssertCoreAssembly { get; set; } = "";

    public string ProjectDirectory { get; set; } = "";

    public bool OmniAssertRewrite { get; set; } = true;

    [Output]
    public ITaskItem[] RewrittenCompile { get; set; } = Array.Empty<ITaskItem>();

    public override bool Execute()
    {
        if (!OmniAssertRewrite)
        {
            RewrittenCompile = Compile;
            return true;
        }

        if (string.IsNullOrWhiteSpace(OmniAssertCoreAssembly) || !File.Exists(OmniAssertCoreAssembly))
        {
            Log.LogWarning("OmniAssert: OmniAssertCoreAssembly not found; skipping rewrite.");
            RewrittenCompile = Compile;
            return true;
        }

        var outDir = Path.Combine(IntermediateOutputPath.TrimEnd(Path.DirectorySeparatorChar), "omniassert", "rewritten");
        Directory.CreateDirectory(outDir);

        var refs = new List<MetadataReference>();
        foreach (var r in ReferencePath ?? Array.Empty<ITaskItem>())
        {
            var path = r.ItemSpec;
            if (File.Exists(path))
                refs.Add(MetadataReference.CreateFromFile(path));
        }

        refs.Add(MetadataReference.CreateFromFile(OmniAssertCoreAssembly));

        var parseOpts = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
        var trees = new List<SyntaxTree>();
        var treeByFullPath = new Dictionary<string, SyntaxTree>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in Compile)
        {
            var path = item.ItemSpec;
            if (!path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                continue;
            if (!File.Exists(path))
                continue;

            var full = Path.GetFullPath(path);
            var text = File.ReadAllText(path);
            var tree = CSharpSyntaxTree.ParseText(SourceText.From(text, Encoding.UTF8), parseOpts, path);
            trees.Add(tree);
            treeByFullPath[full] = tree;
        }

        if (trees.Count == 0)
        {
            RewrittenCompile = Compile;
            return true;
        }

        var syntaxErrors = trees.SelectMany(t => t.GetDiagnostics()).Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        if (syntaxErrors.Count > 0)
        {
            foreach (var d in syntaxErrors.Take(8))
                Log.LogWarning("OmniAssert rewrite parse: {0}", d);
            Log.LogWarning("OmniAssert: parse errors; skipping rewrite.");
            RewrittenCompile = Compile;
            return true;
        }

        var compilation = CSharpCompilation.Create(
            "OmniAssertRewrite",
            trees,
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        var outputItems = new List<ITaskItem>();

        foreach (var item in Compile)
        {
            var path = item.ItemSpec;
            if (!path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) || !File.Exists(path))
            {
                outputItems.Add(item);
                continue;
            }

            var full = Path.GetFullPath(path);
            if (!treeByFullPath.TryGetValue(full, out var tree))
            {
                outputItems.Add(item);
                continue;
            }

            var model = compilation.GetSemanticModel(tree);
            var root = tree.GetRoot();
            var rewriter = new VerifySyntaxRewriter(model);
            var newRoot = rewriter.Visit(root);
            if (!rewriter.Changed)
            {
                outputItems.Add(item);
                continue;
            }

            var rel = GetRelativePathSafe(ProjectDirectory, path);
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(full)));
            var outPath = Path.Combine(outDir, hash[..16], rel.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
            var outFolder = Path.GetDirectoryName(outPath);
            if (!string.IsNullOrEmpty(outFolder))
                Directory.CreateDirectory(outFolder);

            var textOut = newRoot.NormalizeWhitespace().ToFullString();
            File.WriteAllText(outPath, textOut, Encoding.UTF8);

            var taskItem = new TaskItem(outPath);
            item.CopyMetadataTo(taskItem);
            taskItem.SetMetadata("OmniAssertOriginal", path);
            outputItems.Add(taskItem);
        }

        RewrittenCompile = outputItems.ToArray();
        return true;
    }

    private static string GetRelativePathSafe(string baseDir, string fullPath)
    {
        if (string.IsNullOrEmpty(baseDir))
            return Path.GetFileName(fullPath);
        try
        {
            return Path.GetRelativePath(baseDir, fullPath);
        }
        catch
        {
            return Path.GetFileName(fullPath);
        }
    }
}
