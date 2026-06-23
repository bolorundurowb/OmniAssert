using OmniAssert.Analyzers;

namespace OmniAssert.Analyzers.Tests;

public class FluentGrammarSuggestionTests
{
    [Theory]
    [InlineData("ToBeFalse", "BeFalse")]
    [InlineData("ToBeTrue", "BeTrue")]
    [InlineData("ToBe", "Be")]
    [InlineData("NotToBeNull", "NotBeNull")]
    [InlineData("NotToContain", "NotContain")]
    [InlineData("ToHaveCount", "HaveCount")]
    [InlineData("ToBeGreaterThanOrEqualTo", "BeGreaterThanOrEqualTo")]
    public void SuggestNewFluentName_maps_legacy_names(string legacy, string expected) =>
        Xunit.Assert.Equal(expected, OmniAssertDiagnosticAnalyzer.SuggestNewFluentName(legacy));
}
