using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using OmniAssert.Generator.Rewrite;

namespace OmniAssert.Generator;

/// <summary>
/// Emits interceptors for each interceptable <c>OmniAssert.Assert.VerifyExpression(bool, string?)</c> call site
/// by default. Bare identifiers redirect to fluent <c>Verify(...).ToBeTrue()</c>; other boolean shapes call
/// <c>VerifyExpression(bool, string?)</c> with the captured expression text.
/// Set <c>OmniAssertDisableVerifyInterceptors</c> to <c>true</c> in the project file to opt out.
/// </summary>
[Generator]
public sealed class OmniAssertIncrementalGenerator : IIncrementalGenerator
{
    private const string DisableProperty = "build_property.OmniAssertDisableVerifyInterceptors";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        RegisterRewritePipeline(context);

        var interceptorsEnabled = context.AnalyzerConfigOptionsProvider.Select(
            static (options, _) =>
                !options.GlobalOptions.TryGetValue(DisableProperty, out var v) ||
                !v.Equals("true", StringComparison.OrdinalIgnoreCase));

        var compilationAndFlag = context.CompilationProvider.Combine(interceptorsEnabled);

        context.RegisterSourceOutput(compilationAndFlag, (spc, tuple) =>
        {
            var compilation = tuple.Left;
            var enabled = tuple.Right;
            if (!enabled)
                return;

            var ct = spc.CancellationToken;
            var byTree = new Dictionary<SyntaxTree, List<InterceptorCandidate>>();

            foreach (var tree in compilation.SyntaxTrees)
            {
                if (tree.FilePath is null || !tree.FilePath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                    continue;

                var model = compilation.GetSemanticModel(tree);
                var root = tree.GetRoot(ct);
                foreach (var node in root.DescendantNodes().OfType<InvocationExpressionSyntax>())
                {
                    if (model.GetSymbolInfo(node, ct).Symbol is not IMethodSymbol method)
                        continue;
                    if (!VerifyLoweringFacts.IsAssertVerifyExpression(method))
                        continue;

#pragma warning disable RS0030 // GetInterceptableLocation is the supported interceptor authoring API
                    if (model.GetInterceptableLocation(node, ct) is not { } location)
#pragma warning restore RS0030
                        continue;

                    var boolExpr = GetBooleanExpression(node);
                    if (boolExpr == null)
                        continue;
                    var engine = new VerifyExpansionEngine(model);
                    var simple = engine.IsSimpleBooleanIdentifier(boolExpr);

                    if (!byTree.TryGetValue(tree, out var list))
                    {
                        list = new List<InterceptorCandidate>();
                        byTree[tree] = list;
                    }

                    list.Add(new InterceptorCandidate(location, simple));
                }
            }

            foreach (var kvp in byTree)
            {
                var tree = kvp.Key;
                var candidates = kvp.Value;
                if (candidates.Count == 0)
                    continue;

                var className = MakeFileScopedClassName(tree.FilePath);
                var source = EmitInterceptorsFile(className, candidates);
                var hint = SanitizeFileHint(tree.FilePath);
                spc.AddSource($"OmniAssert.VerifyInterceptors.{hint}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        });
    }

    private void RegisterRewritePipeline(IncrementalGeneratorInitializationContext context)
    {
        var rewriteInputs = context.AdditionalTextsProvider
            .Where(static f => f.Path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            .Combine(context.CompilationProvider);

        context.RegisterSourceOutput(rewriteInputs, (spc, tuple) =>
        {
            var additionalText = tuple.Left;
            var compilation = tuple.Right;
            var ct = spc.CancellationToken;

            var sourceText = additionalText.GetText(ct);
            if (sourceText == null)
                return;

            if (compilation is not CSharpCompilation csharpComp)
                return;

            // Reuse the parse options from an existing tree so all features (nullable, interceptors, etc.) match.
            var parseOptions = (csharpComp.SyntaxTrees.FirstOrDefault()?.Options as CSharpParseOptions)
                               ?? new CSharpParseOptions(csharpComp.LanguageVersion);

            var tree = CSharpSyntaxTree.ParseText(
                sourceText.ToString(),
                parseOptions,
                path: additionalText.Path,
                cancellationToken: ct);

            var tempComp = csharpComp.AddSyntaxTrees(tree);
            var model = tempComp.GetSemanticModel(tree);

            var rewriter = new Rewrite.VerifyCallSiteRewriter(model, ct);
            var newRoot = rewriter.Visit(tree.GetRoot(ct));

            if (!rewriter.Modified || newRoot == null)
                return;

            var hint = SanitizeFileHint(additionalText.Path);
            var header = "// <auto-generated/>\n// OmniAssert: rewritten VerifyExpression call sites with operand capture.\n#nullable enable\n";
            spc.AddSource(
                $"OmniAssert.Rewritten.{hint}.g.cs",
                SourceText.From(header + newRoot.ToFullString(), Encoding.UTF8));
        });
    }

    private static string MakeFileScopedClassName(string filePath)
    {
        var name = Path.GetFileNameWithoutExtension(filePath);
        if (string.IsNullOrEmpty(name))
            name = "Unknown";

        var safe = new string(name.Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray());
        if (safe.Length == 0)
            safe = "File";
        var hash = HexPrefix(Encoding.UTF8.GetBytes(filePath), 4);
        return "OmniAssertVerifyInterceptors_" + safe + "_" + hash;
    }

    private static string SanitizeFileHint(string filePath)
    {
        var bytes = Encoding.UTF8.GetBytes(filePath);
        return HexPrefix(bytes, 8);
    }

    /// <summary>Uppercase hex of the first <paramref name="byteCount"/> bytes of SHA256(input).</summary>
    private static string HexPrefix(byte[] input, int byteCount)
    {
        using var sha = SHA256.Create();
        var full = sha.ComputeHash(input);
        var take = Math.Min(byteCount, full.Length);
        return BitConverter.ToString(full, 0, take).Replace("-", string.Empty);
    }

    private static string EmitInterceptorsFile(string className, List<InterceptorCandidate> candidates)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("// OmniAssert: intercepts Assert.VerifyExpression(bool, ...) per consumer call site.");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("namespace System.Runtime.CompilerServices");
        sb.AppendLine("{");
        sb.AppendLine("    [global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = true)]");
        sb.AppendLine("    file sealed class InterceptsLocationAttribute : global::System.Attribute");
        sb.AppendLine("    {");
        sb.AppendLine("        public InterceptsLocationAttribute(int version, string data) { }");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine("namespace OmniAssert.Generated");
        sb.AppendLine("{");
        sb.AppendLine("    using System.Runtime.CompilerServices;");
        sb.Append("    file static class ");
        sb.AppendLine(className);
        sb.AppendLine("    {");

        for (var i = 0; i < candidates.Count; i++)
        {
            var candidate = candidates[i];
            sb.Append("        ");
            sb.Append("[InterceptsLocation(");
            sb.Append(candidate.Location.Version.ToString(CultureInfo.InvariantCulture));
            sb.Append(", \"");
            sb.Append(EscapeCSharpStringLiteral(candidate.Location.Data));
            sb.AppendLine("\")]");
            sb.Append("        internal static void OmniAssertVerifyIntercept_");
            sb.Append(i.ToString(CultureInfo.InvariantCulture));
            sb.AppendLine("(this bool condition, string? expression = null)");
            sb.AppendLine("        {");
            if (candidate.SimpleIdentifierPath)
            {
                sb.AppendLine("            global::OmniAssert.Ensure.Must(condition, expression).BeTrue();");
            }
            else
            {
                sb.AppendLine("            global::OmniAssert.Ensure.VerifyExpression(condition, expression);");
            }

            sb.AppendLine("        }");
            if (i < candidates.Count - 1)
                sb.AppendLine();
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string EscapeCSharpStringLiteral(string value) =>
        value.Replace("\\", "\\\\").Replace("\"", "\\\"");

    /// <summary>
    /// Returns the boolean expression from a <c>VerifyExpression</c> invocation regardless of call form.
    /// For extension-method calls (<c>expr.VerifyExpression()</c>) the bool is the receiver;
    /// for static calls (<c>Assert.VerifyExpression(expr)</c>) it is the first argument.
    /// </summary>
    private static ExpressionSyntax? GetBooleanExpression(InvocationExpressionSyntax node)
    {
        if (node.ArgumentList.Arguments.Count >= 1)
            return node.ArgumentList.Arguments[0].Expression;

        if (node.Expression is MemberAccessExpressionSyntax memberAccess)
            return memberAccess.Expression;

        return null;
    }

}
