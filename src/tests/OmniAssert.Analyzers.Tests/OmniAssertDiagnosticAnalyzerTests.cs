using OmniAssert.Analyzers;

namespace OmniAssert.Analyzers.Tests;

public class OmniAssertDiagnosticAnalyzerTests
{
    [Fact]
    public async Task LegacyAssert_identifier_reports_OA001()
    {
        var source = """
using OmniAssert;
public static class T
{
    public static void M() { var t = typeof(Assert); }
}
""";

        var diagnostics = await AnalyzerTestHelper.GetOmniAssertAnalyzerDiagnosticsAsync(source);
        Xunit.Assert.Contains(diagnostics, d => d.Id == OmniAssertDiagnosticAnalyzer.LegacyAssertId);
    }

    [Fact]
    public async Task LegacyAssert_member_access_reports_OA001_and_OA004()
    {
        var source = """
public static class T
{
    public static void M() { OmniAssert.Assert.VerifyExpression(true); }
}
""";

        var diagnostics = await AnalyzerTestHelper.GetOmniAssertAnalyzerDiagnosticsAsync(source);
        Xunit.Assert.Contains(diagnostics, d => d.Id == OmniAssertDiagnosticAnalyzer.LegacyAssertId);
        Xunit.Assert.Contains(diagnostics, d => d.Id == OmniAssertDiagnosticAnalyzer.LegacyVerifyExpressionId);
    }

    [Fact]
    public async Task LegacyVerifyExpression_extension_reports_OA004()
    {
        var source = """
using OmniAssert;
public static class T
{
    public static void M(int x, int y) { (x > y).VerifyExpression(); }
}
""";

        var diagnostics = await AnalyzerTestHelper.GetOmniAssertAnalyzerDiagnosticsAsync(source);
        Xunit.Assert.Contains(diagnostics, d => d.Id == OmniAssertDiagnosticAnalyzer.LegacyVerifyExpressionId);
    }

    [Fact]
    public async Task LegacyVerifyExpression_static_style_reports_OA004()
    {
        var source = """
using OmniAssert;
public static class T
{
    public static void M(int x, int y) { Ensure.VerifyExpression(x > y); }
}
""";

        var diagnostics = await AnalyzerTestHelper.GetOmniAssertAnalyzerDiagnosticsAsync(source);
        Xunit.Assert.Contains(diagnostics, d => d.Id == OmniAssertDiagnosticAnalyzer.LegacyVerifyExpressionId);
    }

    [Fact]
    public async Task Ensure_Expression_reports_no_legacy_diagnostics()
    {
        var source = """
using OmniAssert;
public static class T
{
    public static void M(int x, int y) { Ensure.Expression(x > y); }
}
""";

        var diagnostics = await AnalyzerTestHelper.GetOmniAssertAnalyzerDiagnosticsAsync(source);
        Xunit.Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task Verify_fluent_entry_reports_OA002()
    {
        var source = """
using OmniAssert;
public static class T
{
    public static void M(int x) { x.Verify().ToBeGreaterThan(0); }
}
""";

        var diagnostics = await AnalyzerTestHelper.GetOmniAssertAnalyzerDiagnosticsAsync(source);
        Xunit.Assert.Contains(diagnostics, d => d.Id == OmniAssertDiagnosticAnalyzer.LegacyVerifyId);
    }

    [Fact]
    public async Task LegacyToGrammar_after_Verify_reports_OA003()
    {
        var source = """
using OmniAssert;
public static class T
{
    public static void M(string? s) { s.Verify().NotToBeNull(); }
}
""";

        var diagnostics = await AnalyzerTestHelper.GetOmniAssertAnalyzerDiagnosticsAsync(source);
        Xunit.Assert.Contains(diagnostics, d => d.Id == OmniAssertDiagnosticAnalyzer.LegacyFluentGrammarId);
    }

    [Fact]
    public async Task ModernMust_syntax_reports_no_legacy_diagnostics()
    {
        var source = """
using OmniAssert;
public static class T
{
    public static void M(int x, string? s)
    {
        x.Must().BeGreaterThan(0);
        s.Must().NotBeNull();
    }
}
""";

        var diagnostics = await AnalyzerTestHelper.GetOmniAssertAnalyzerDiagnosticsAsync(source);
        Xunit.Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task ToBeFalse_without_fluent_root_does_not_report_OA003()
    {
        var source = """
public static class T
{
    public static void M() { ToBeFalse(); }
    private static void ToBeFalse() { }
}
""";

        var diagnostics = await AnalyzerTestHelper.GetOmniAssertAnalyzerDiagnosticsAsync(source);
        Xunit.Assert.DoesNotContain(diagnostics, d => d.Id == OmniAssertDiagnosticAnalyzer.LegacyFluentGrammarId);
    }

    [Fact]
    public async Task LegacyToGrammar_after_Must_reports_OA003()
    {
        var source = """
using System.Collections.Generic;
using OmniAssert;
public static class T
{
    public static void M(List<int> items) { items.Verify().ToBeEmpty(); }
}
""";

        var diagnostics = await AnalyzerTestHelper.GetOmniAssertAnalyzerDiagnosticsAsync(source);
        Xunit.Assert.Contains(diagnostics, d => d.Id == OmniAssertDiagnosticAnalyzer.LegacyFluentGrammarId);
    }
}
