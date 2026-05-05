using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace OmniAssert.Generator.Tests;

/// <summary>
/// Tests for <see cref="OmniAssertIncrementalGenerator"/> covering the incremental source-generation pipeline,
/// private helper methods (via reflection), and the <see cref="VerifyLoweringFacts"/> discriminator used
/// during candidate collection.
/// </summary>
public class OmniAssertIncrementalGeneratorTests
{
    // ── Generator pipeline ─────────────────────────────────────────────────

    [Fact]
    public void Generator_WhenInterceptorsDisabled_ShouldProduceNoSource()
    {
        var source = """
using static OmniAssert.Assert;
public static class T
{
    public static void M()
    {
        VerifyExpression(1 > 0);
    }
}
""";
        var (diagnostics, generatedSources) = RunGenerator(source, enableInterceptors: false);

        Xunit.Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Xunit.Assert.Empty(generatedSources.Where(s => s.HintName.Contains("VerifyInterceptors")));
    }

    [Fact]
    public void Generator_WhenInterceptorsEnabled_WithVerifyExpression_ShouldProduceInterceptorSource()
    {
        var source = """
using static OmniAssert.Assert;
public static class T
{
    public static void M()
    {
        VerifyExpression(1 > 0);
    }
}
""";
        var (diagnostics, generatedSources) = RunGenerator(source, enableInterceptors: true);

        Xunit.Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        var interceptorSource = generatedSources.FirstOrDefault(s => s.HintName.Contains("VerifyInterceptors"));
        Xunit.Assert.NotNull(interceptorSource);
    }

    [Fact]
    public void Generator_WithComplexExpression_ShouldEmitVerifyExpressionPath()
    {
        var source = """
using static OmniAssert.Assert;
public static class T
{
    public static void M(int x, int y)
    {
        VerifyExpression(x > y);
    }
}
""";
        var (_, generatedSources) = RunGenerator(source, enableInterceptors: true);

        var interceptor = generatedSources.FirstOrDefault(s => s.HintName.Contains("VerifyInterceptors"));
        Xunit.Assert.NotNull(interceptor);
        Xunit.Assert.Contains("VerifyExpression", interceptor!.SourceText.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void Generator_WithSimpleIdentifierExpression_ShouldEmitToBeTruePath()
    {
        var source = """
using static OmniAssert.Assert;
public static class T
{
    public static void M(bool flag)
    {
        VerifyExpression(flag);
    }
}
""";
        var (_, generatedSources) = RunGenerator(source, enableInterceptors: true);

        var interceptor = generatedSources.FirstOrDefault(s => s.HintName.Contains("VerifyInterceptors"));
        Xunit.Assert.NotNull(interceptor);
        Xunit.Assert.Contains("ToBeTrue()", interceptor!.SourceText.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void Generator_WithMultipleCallSites_ShouldEmitOneInterceptorPerCallSite()
    {
        var source = """
using static OmniAssert.Assert;
public static class T
{
    public static void M(int a, int b, bool flag)
    {
        VerifyExpression(a > b);
        VerifyExpression(flag);
    }
}
""";
        var (_, generatedSources) = RunGenerator(source, enableInterceptors: true);

        var interceptor = generatedSources.FirstOrDefault(s => s.HintName.Contains("VerifyInterceptors"));
        Xunit.Assert.NotNull(interceptor);

        var text = interceptor!.SourceText.ToString();
        Xunit.Assert.Contains("OmniAssertVerifyIntercept_0", text, StringComparison.Ordinal);
        Xunit.Assert.Contains("OmniAssertVerifyIntercept_1", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Generator_GeneratedSource_ContainsInterceptsLocationAttribute()
    {
        var source = """
using static OmniAssert.Assert;
public static class T
{
    public static void M() { VerifyExpression(true); }
}
""";
        var (_, generatedSources) = RunGenerator(source, enableInterceptors: true);

        var interceptor = generatedSources.FirstOrDefault(s => s.HintName.Contains("VerifyInterceptors"));
        Xunit.Assert.NotNull(interceptor);
        Xunit.Assert.Contains("InterceptsLocation", interceptor!.SourceText.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void Generator_GeneratedSource_ContainsNullableEnable()
    {
        var source = """
using static OmniAssert.Assert;
public static class T
{
    public static void M() { VerifyExpression(true); }
}
""";
        var (_, generatedSources) = RunGenerator(source, enableInterceptors: true);

        var interceptor = generatedSources.FirstOrDefault(s => s.HintName.Contains("VerifyInterceptors"));
        Xunit.Assert.NotNull(interceptor);
        Xunit.Assert.Contains("#nullable enable", interceptor!.SourceText.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void Generator_WhenNoVerifyExpressionCallSites_ShouldProduceNoInterceptorSource()
    {
        var source = """
public static class T
{
    public static void M() { var x = 1 + 2; }
}
""";
        var (_, generatedSources) = RunGenerator(source, enableInterceptors: true);

        Xunit.Assert.Empty(generatedSources.Where(s => s.HintName.Contains("VerifyInterceptors")));
    }

    // ── EscapeCSharpStringLiteral (via reflection) ──────────────────────────

    [Theory]
    [InlineData("plain", "plain")]
    [InlineData("with\\backslash", "with\\\\backslash")]
    [InlineData("with\"quote", "with\\\"quote")]
    [InlineData("both\\and\"here", "both\\\\and\\\"here")]
    [InlineData("", "")]
    public void EscapeCSharpStringLiteral_ReturnsExpected(string input, string expected)
    {
        var method = typeof(OmniAssertIncrementalGenerator)
            .GetMethod("EscapeCSharpStringLiteral", BindingFlags.NonPublic | BindingFlags.Static);
        Xunit.Assert.NotNull(method);

        var result = (string)method!.Invoke(null, new object[] { input })!;
        Xunit.Assert.Equal(expected, result);
    }

    // ── MakeFileScopedClassName (via reflection) ─────────────────────────────

    [Fact]
    public void MakeFileScopedClassName_WithNormalPath_ReturnsSafeIdentifier()
    {
        var method = typeof(OmniAssertIncrementalGenerator)
            .GetMethod("MakeFileScopedClassName", BindingFlags.NonPublic | BindingFlags.Static);
        Xunit.Assert.NotNull(method);

        var result = (string)method!.Invoke(null, new object[] { "/path/to/MyFile.cs" })!;

        Xunit.Assert.StartsWith("OmniAssertVerifyInterceptors_MyFile_", result, StringComparison.Ordinal);
        Xunit.Assert.True(result.All(c => char.IsLetterOrDigit(c) || c == '_'),
            $"Class name should be a valid identifier; got: {result}");
    }

    [Fact]
    public void MakeFileScopedClassName_DifferentPaths_ProduceDifferentClassNames()
    {
        var method = typeof(OmniAssertIncrementalGenerator)
            .GetMethod("MakeFileScopedClassName", BindingFlags.NonPublic | BindingFlags.Static);
        Xunit.Assert.NotNull(method);

        var name1 = (string)method!.Invoke(null, new object[] { "/a/FileA.cs" })!;
        var name2 = (string)method!.Invoke(null, new object[] { "/b/FileB.cs" })!;

        Xunit.Assert.NotEqual(name1, name2);
    }

    [Fact]
    public void MakeFileScopedClassName_SamePath_ProducesSameClassName()
    {
        var method = typeof(OmniAssertIncrementalGenerator)
            .GetMethod("MakeFileScopedClassName", BindingFlags.NonPublic | BindingFlags.Static);
        Xunit.Assert.NotNull(method);

        var name1 = (string)method!.Invoke(null, new object[] { "/src/MyFile.cs" })!;
        var name2 = (string)method!.Invoke(null, new object[] { "/src/MyFile.cs" })!;

        Xunit.Assert.Equal(name1, name2);
    }

    [Fact]
    public void MakeFileScopedClassName_WithSpecialCharsInFileName_ReplacesNonAlphanumericWithUnderscore()
    {
        var method = typeof(OmniAssertIncrementalGenerator)
            .GetMethod("MakeFileScopedClassName", BindingFlags.NonPublic | BindingFlags.Static);
        Xunit.Assert.NotNull(method);

        var result = (string)method!.Invoke(null, new object[] { "/path/my-file.name.cs" })!;

        // Name segment is "my-file.name" → "my_file_name"
        Xunit.Assert.StartsWith("OmniAssertVerifyInterceptors_my_file_name_", result, StringComparison.Ordinal);
    }

    // ── SanitizeFileHint (via reflection) ────────────────────────────────────

    [Fact]
    public void SanitizeFileHint_SamePath_ReturnsSameHash()
    {
        var method = typeof(OmniAssertIncrementalGenerator)
            .GetMethod("SanitizeFileHint", BindingFlags.NonPublic | BindingFlags.Static);
        Xunit.Assert.NotNull(method);

        var hint1 = (string)method!.Invoke(null, new object[] { "/src/File.cs" })!;
        var hint2 = (string)method!.Invoke(null, new object[] { "/src/File.cs" })!;

        Xunit.Assert.Equal(hint1, hint2);
    }

    [Fact]
    public void SanitizeFileHint_DifferentPaths_ReturnDifferentHints()
    {
        var method = typeof(OmniAssertIncrementalGenerator)
            .GetMethod("SanitizeFileHint", BindingFlags.NonPublic | BindingFlags.Static);
        Xunit.Assert.NotNull(method);

        var hint1 = (string)method!.Invoke(null, new object[] { "/src/A.cs" })!;
        var hint2 = (string)method!.Invoke(null, new object[] { "/src/B.cs" })!;

        Xunit.Assert.NotEqual(hint1, hint2);
    }

    [Fact]
    public void SanitizeFileHint_ResultIs16HexChars()
    {
        var method = typeof(OmniAssertIncrementalGenerator)
            .GetMethod("SanitizeFileHint", BindingFlags.NonPublic | BindingFlags.Static);
        Xunit.Assert.NotNull(method);

        var hint = (string)method!.Invoke(null, new object[] { "/some/path.cs" })!;

        // 8 bytes → 16 hex characters (no dashes)
        Xunit.Assert.Equal(16, hint.Length);
        Xunit.Assert.True(hint.All(c => char.IsAsciiHexDigit(c)), $"Expected hex string, got: {hint}");
    }

    // ── VerifyLoweringFacts ──────────────────────────────────────────────────

    [Fact]
    public void IsAssertVerifyExpression_WhenSymbolNameIsNotVerifyExpression_ReturnsFalse()
    {
        // Build a minimal method symbol whose name is different
        var source = """
namespace OmniAssert
{
    public static class Assert { public static void Verify(bool b) { } }
}
public static class T { public static void M() { OmniAssert.Assert.Verify(true); } }
""";
        var tree = CSharpSyntaxTree.ParseText(source, path: "T.cs");
        var refs = VerifyExpansionEngineCompileTests.MetadataReferencesForOmniAssertConsumer().ToList();
        var compilation = CSharpCompilation.Create(
            "VerifyLoweringFactsNameAsm",
            new[] { tree },
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var model = compilation.GetSemanticModel(tree);
        var invocation = tree.GetRoot().DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax>()
            .First();

        var sym = model.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        Xunit.Assert.NotNull(sym);
        Xunit.Assert.False(VerifyLoweringFacts.IsAssertVerifyExpression(sym!));
    }

    [Fact]
    public void IsAssertVerifyExpression_WhenFirstParamIsNotBool_ReturnsFalse()
    {
        var source = """
namespace OmniAssert
{
    public static class Assert { public static void VerifyExpression(string s) { } }
}
public static class T { public static void M() { OmniAssert.Assert.VerifyExpression("x"); } }
""";
        var tree = CSharpSyntaxTree.ParseText(source, path: "T.cs");
        var refs = VerifyExpansionEngineCompileTests.MetadataReferencesForOmniAssertConsumer().ToList();
        var compilation = CSharpCompilation.Create(
            "VerifyLoweringFactsParamAsm",
            new[] { tree },
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var model = compilation.GetSemanticModel(tree);
        var invocation = tree.GetRoot().DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax>()
            .First();

        var sym = model.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        Xunit.Assert.NotNull(sym);
        Xunit.Assert.False(VerifyLoweringFacts.IsAssertVerifyExpression(sym!));
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static (IReadOnlyList<Diagnostic> Diagnostics, IReadOnlyList<GeneratedSourceResult> GeneratedSources)
        RunGenerator(string source, bool enableInterceptors)
    {
        var parseOptions = new CSharpParseOptions(LanguageVersion.Latest);
        var tree = CSharpSyntaxTree.ParseText(source, parseOptions, path: "T.cs");
        var refs = VerifyExpansionEngineCompileTests.MetadataReferencesForOmniAssertConsumer().ToList();
        var compilation = CSharpCompilation.Create(
            "GeneratorTestAsm",
            new[] { tree },
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new OmniAssertIncrementalGenerator();
        var optionsProvider = new TestAnalyzerConfigOptionsProvider(enableInterceptors);

        var driver = CSharpGeneratorDriver
            .Create(generator)
            .WithUpdatedAnalyzerConfigOptions(optionsProvider)
            .RunGenerators(compilation);

        var result = driver.GetRunResult();
        return (result.Diagnostics, result.Results[0].GeneratedSources);
    }

    // ── TestAnalyzerConfigOptionsProvider ────────────────────────────────────

    private sealed class TestAnalyzerConfigOptionsProvider : Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider
    {
        private readonly bool _interceptorsEnabled;

        public TestAnalyzerConfigOptionsProvider(bool interceptorsEnabled)
        {
            _interceptorsEnabled = interceptorsEnabled;
        }

        public override Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions GlobalOptions =>
            new TestAnalyzerConfigOptions(_interceptorsEnabled);

        public override Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions GetOptions(SyntaxTree tree) =>
            new TestAnalyzerConfigOptions(_interceptorsEnabled);

        public override Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions GetOptions(AdditionalText textFile) =>
            new TestAnalyzerConfigOptions(_interceptorsEnabled);
    }

    private sealed class TestAnalyzerConfigOptions : Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions
    {
        private readonly bool _interceptorsEnabled;

        public TestAnalyzerConfigOptions(bool interceptorsEnabled)
        {
            _interceptorsEnabled = interceptorsEnabled;
        }

        public override bool TryGetValue(string key, out string value)
        {
            if (key == "build_property.OmniAssertEnableVerifyInterceptors")
            {
                value = _interceptorsEnabled ? "true" : "false";
                return true;
            }

            value = string.Empty;
            return false;
        }
    }
}
