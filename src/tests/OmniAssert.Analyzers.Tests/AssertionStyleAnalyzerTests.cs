using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OmniAssert.Analyzers;

namespace OmniAssert.Analyzers.Tests;

public class AssertionStyleAnalyzerTests
{
    public class OA005ComparisonInBoolAssert
    {
        [Theory]
        [InlineData("(a < b).Must().BeTrue()", "OA005")]
        [InlineData("(a <= b).Must().BeTrue()", "OA005")]
        [InlineData("(a > b).Must().BeTrue()", "OA005")]
        [InlineData("(a >= b).Must().BeTrue()", "OA005")]
        [InlineData("(a == b).Must().BeTrue()", "OA005")]
        [InlineData("(a != b).Must().BeTrue()", "OA005")]
        [InlineData("(a < b).Must().BeFalse()", "OA005")]
        [InlineData("(a > b).Must().BeFalse()", "OA005")]
        public async Task Detects_comparison_in_bool_assert(string assertion, string expectedId)
        {
            var source = $$"""
                using OmniAssert;
                public static class T
                {
                    public static void M(int a, int b) { {{assertion}}; }
                }
                """;

            var diagnostics = await AnalyzerTestHelper.GetOmniAssertAnalyzerDiagnosticsAsync(source);
            Xunit.Assert.Contains(diagnostics, d => d.Id == expectedId);
        }

        [Fact]
        public async Task Does_not_report_for_plain_bool_subject()
        {
            var source = """
                using OmniAssert;
                public static class T
                {
                    public static void M(bool flag) { flag.Must().BeTrue(); }
                }
                """;

            var diagnostics = await AnalyzerTestHelper.GetOmniAssertAnalyzerDiagnosticsAsync(source);
            Xunit.Assert.DoesNotContain(diagnostics, d => d.Id == OmniAssertDiagnosticAnalyzer.ComparisonInBoolAssertId);
        }

        [Fact]
        public async Task Does_not_report_for_logical_and_combination()
        {
            var source = """
                using OmniAssert;
                public static class T
                {
                    public static void M(int a, int b, int c, int d) { (a < b && c > d).Must().BeTrue(); }
                }
                """;

            var diagnostics = await AnalyzerTestHelper.GetOmniAssertAnalyzerDiagnosticsAsync(source);
            Xunit.Assert.DoesNotContain(diagnostics, d => d.Id == OmniAssertDiagnosticAnalyzer.ComparisonInBoolAssertId);
        }

        [Fact]
        public async Task Reports_unfixable_for_non_numeric_string_subject()
        {
            var source = """
                using OmniAssert;
                public static class T
                {
                    public static void M(string a, string b) { (a == b).Must().BeTrue(); }
                }
                """;

            var diagnostics = await AnalyzerTestHelper.GetOmniAssertAnalyzerDiagnosticsAsync(source);
            var oa005 = diagnostics.Single(d => d.Id == OmniAssertDiagnosticAnalyzer.ComparisonInBoolAssertId);
            Xunit.Assert.False(oa005.Properties.ContainsKey("assertionMethod"));
            Xunit.Assert.Contains("Ensure.Expression", oa005.GetMessage());
        }

        [Theory]
        [InlineData("BeLessThan", SyntaxKind.LessThanExpression, true)]
        [InlineData("BeLessThanOrEqualTo", SyntaxKind.LessThanOrEqualExpression, true)]
        [InlineData("BeGreaterThan", SyntaxKind.GreaterThanExpression, true)]
        [InlineData("BeGreaterThanOrEqualTo", SyntaxKind.GreaterThanOrEqualExpression, true)]
        [InlineData("Be", SyntaxKind.EqualsExpression, true)]
        [InlineData("NotBe", SyntaxKind.NotEqualsExpression, true)]
        [InlineData("BeGreaterThanOrEqualTo", SyntaxKind.LessThanExpression, false)]
        [InlineData("BeGreaterThan", SyntaxKind.LessThanOrEqualExpression, false)]
        [InlineData("BeLessThanOrEqualTo", SyntaxKind.GreaterThanExpression, false)]
        [InlineData("BeLessThan", SyntaxKind.GreaterThanOrEqualExpression, false)]
        [InlineData("NotBe", SyntaxKind.EqualsExpression, false)]
        [InlineData("Be", SyntaxKind.NotEqualsExpression, false)]
        public void TryGetComparisonMapping_maps_operators(string expected, SyntaxKind kind, bool isBeTrue) =>
            Xunit.Assert.True(OmniAssertDiagnosticAnalyzer.TryGetComparisonMapping(kind, isBeTrue, out var mapped) && mapped == expected);

        [Theory]
        [InlineData("(a < b).Must().BeTrue()", "a.Must().BeLessThan(b)")]
        [InlineData("(a <= b).Must().BeTrue()", "a.Must().BeLessThanOrEqualTo(b)")]
        [InlineData("(a > b).Must().BeTrue()", "a.Must().BeGreaterThan(b)")]
        [InlineData("(a >= b).Must().BeTrue()", "a.Must().BeGreaterThanOrEqualTo(b)")]
        [InlineData("(a == b).Must().BeTrue()", "a.Must().Be(b)")]
        [InlineData("(a != b).Must().BeTrue()", "a.Must().NotBe(b)")]
        [InlineData("(a < b).Must().BeFalse()", "a.Must().BeGreaterThanOrEqualTo(b)")]
        [InlineData("(a > b).Must().BeFalse()", "a.Must().BeLessThanOrEqualTo(b)")]
        public async Task CodeFix_rewrites_to_subject_first(string awkward, string expected)
        {
            var source = $$"""
                using OmniAssert;
                public static class T
                {
                    public static void M(int a, int b) { {{awkward}}; }
                }
                """;

            var fixedSource = await AnalyzerTestHelper.ApplyCodeFixAsync(source, OmniAssertDiagnosticAnalyzer.ComparisonInBoolAssertId);
            Xunit.Assert.Contains(expected, fixedSource);
            Xunit.Assert.DoesNotContain(awkward, fixedSource);
        }

        [Fact]
        public async Task CodeFix_supports_member_access_subject()
        {
            var source = """
                using OmniAssert;
                public static class T
                {
                    public static void M(Order o, int limit) { (o.Total < limit).Must().BeTrue(); }
                }
                public class Order { public int Total { get; set; } }
                """;

            var fixedSource = await AnalyzerTestHelper.ApplyCodeFixAsync(source, OmniAssertDiagnosticAnalyzer.ComparisonInBoolAssertId);
            Xunit.Assert.Contains("o.Total.Must().BeLessThan(limit)", fixedSource);
        }

        [Fact]
        public async Task CodeFix_supports_timespan_subject()
        {
            var source = """
                using System;
                using OmniAssert;
                public static class T
                {
                    public static void M(TimeSpan a, TimeSpan b) { (a < b).Must().BeTrue(); }
                }
                """;

            var fixedSource = await AnalyzerTestHelper.ApplyCodeFixAsync(source, OmniAssertDiagnosticAnalyzer.ComparisonInBoolAssertId);
            Xunit.Assert.Contains("a.Must().BeLessThan(b)", fixedSource);
        }
    }

    public class OA006HaveCountZero
    {
        [Fact]
        public async Task Detects_HaveCount_zero_on_collection()
        {
            var source = """
                using System.Collections.Generic;
                using OmniAssert;
                public static class T
                {
                    public static void M(List<int> items) { items.Must().HaveCount(0); }
                }
                """;

            var diagnostics = await AnalyzerTestHelper.GetOmniAssertAnalyzerDiagnosticsAsync(source);
            Xunit.Assert.Contains(diagnostics, d => d.Id == OmniAssertDiagnosticAnalyzer.HaveCountZeroId);
        }

        [Fact]
        public async Task Does_not_report_HaveCount_nonzero()
        {
            var source = """
                using System.Collections.Generic;
                using OmniAssert;
                public static class T
                {
                    public static void M(List<int> items) { items.Must().HaveCount(3); }
                }
                """;

            var diagnostics = await AnalyzerTestHelper.GetOmniAssertAnalyzerDiagnosticsAsync(source);
            Xunit.Assert.DoesNotContain(diagnostics, d => d.Id == OmniAssertDiagnosticAnalyzer.HaveCountZeroId);
        }

        [Fact]
        public async Task CodeFix_replaces_HaveCount_zero_with_BeEmpty()
        {
            var source = """
                using System.Collections.Generic;
                using OmniAssert;
                public static class T
                {
                    public static void M(List<int> items) { items.Must().HaveCount(0); }
                }
                """;

            var fixedSource = await AnalyzerTestHelper.ApplyCodeFixAsync(source, OmniAssertDiagnosticAnalyzer.HaveCountZeroId);
            Xunit.Assert.Contains("items.Must().BeEmpty()", fixedSource);
            Xunit.Assert.DoesNotContain("HaveCount", fixedSource);
        }
    }

    public class OA007RedundantNotNullAfterNew
    {
        [Fact]
        public async Task Detects_NotBeNull_after_new()
        {
            var source = """
                using OmniAssert;
                public static class T
                {
                    public static void M() { new Widget().Must().NotBeNull(); }
                }
                public class Widget { }
                """;

            var diagnostics = await AnalyzerTestHelper.GetOmniAssertAnalyzerDiagnosticsAsync(source);
            Xunit.Assert.Contains(diagnostics, d => d.Id == OmniAssertDiagnosticAnalyzer.RedundantNotNullAfterNewId
                && d.Severity == DiagnosticSeverity.Info);
        }

        [Fact]
        public async Task Does_not_report_NotBeNull_on_identifier_subject()
        {
            var source = """
                using OmniAssert;
                public static class T
                {
                    public static void M(Widget? w) { w.Must().NotBeNull(); }
                }
                public class Widget { }
                """;

            var diagnostics = await AnalyzerTestHelper.GetOmniAssertAnalyzerDiagnosticsAsync(source);
            Xunit.Assert.DoesNotContain(diagnostics, d => d.Id == OmniAssertDiagnosticAnalyzer.RedundantNotNullAfterNewId);
        }

        [Fact]
        public async Task CodeFix_removes_the_redundant_statement()
        {
            var source = """
                using OmniAssert;
                public static class T
                {
                    public static void M()
                    {
                        new Widget().Must().NotBeNull();
                    }
                }
                public class Widget { }
                """;

            var fixedSource = await AnalyzerTestHelper.ApplyCodeFixAsync(source, OmniAssertDiagnosticAnalyzer.RedundantNotNullAfterNewId);
            Xunit.Assert.DoesNotContain("NotBeNull", fixedSource);
            Xunit.Assert.DoesNotContain("new Widget()", fixedSource);
        }
    }
}
