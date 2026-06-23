namespace OmniAssert.Tests;

public class StringAssertionTests
{
    [Fact]
    public void ToBe_WhenStringsEqual_ShouldSucceed()
    {
        ("hello").Must().Be("hello");
    }

    [Fact]
    public void ToBe_WhenStringsDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("abc").Must().Be("xyz"));
    }

    [Fact]
    public void ToBe_WithOrdinalIgnoreCaseComparison_WhenMatch_ShouldSucceed()
    {
        ("HELLO").Must().Be("hello", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToBe_WithOrdinalIgnoreCaseComparison_WhenMismatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("abc").Must().Be("xyz", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ToBeIgnoringCase_WhenStringsMatchIgnoringCase_ShouldSucceed()
    {
        ("HELLO").Must().BeIgnoringCase("hello");
    }

    [Fact]
    public void ToBeIgnoringCase_WhenStringsDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("abc").Must().BeIgnoringCase("xyz"));
    }

    [Fact]
    public void ToBeOneOf_WhenStringIsInSet_ShouldSucceed()
    {
        ("pending").Must().BeOneOf("active", "pending", "archived");
    }

    [Fact]
    public void ToBeOneOf_WhenStringIsNotInSet_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("deleted").Must().BeOneOf("active", "pending", "archived"));
    }

    [Fact]
    public void NotToBe_WhenStringsDiffer_ShouldSucceed()
    {
        ("a").Must().NotBe("b");
    }

    [Fact]
    public void NotToBe_WhenStringsEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("same").Must().NotBe("same"));
    }

    [Fact]
    public void ToBeNull_WhenNull_ShouldSucceed()
    {
        string? s = null;
        (s).Must().BeNull();
    }

    [Fact]
    public void ToBeNull_WhenNotNull_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("x").Must().BeNull());
    }

    [Fact]
    public void NotToBeNull_WhenNotNull_ShouldSucceed()
    {
        ("ok").Must().NotBeNull();
    }

    [Fact]
    public void NotToBeNull_WhenNull_ShouldThrow()
    {
        string? s = null;
        Xunit.Assert.Throws<OmniAssertionException>(() => (s).Must().NotBeNull());
    }

    [Fact]
    public void ToBeEmpty_WhenEmpty_ShouldSucceed()
    {
        ("").Must().BeEmpty();
    }

    [Fact]
    public void ToBeEmpty_WhenNotEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("x").Must().BeEmpty());
    }

    [Fact]
    public void NotToBeEmpty_WhenNotEmpty_ShouldSucceed()
    {
        ("hello").Must().NotBeEmpty();
    }

    [Fact]
    public void NotToBeEmpty_WhenEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("").Must().NotBeEmpty());
    }

    [Fact]
    public void ToBeNullOrEmpty_WhenNull_ShouldSucceed()
    {
        string? s = null;
        (s).Must().BeNullOrEmpty();
    }

    [Fact]
    public void ToBeNullOrEmpty_WhenEmpty_ShouldSucceed()
    {
        ("").Must().BeNullOrEmpty();
    }

    [Fact]
    public void ToBeNullOrEmpty_WhenNotEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("hello").Must().BeNullOrEmpty());
    }

    [Fact]
    public void ToBeNullOrWhiteSpace_WhenNull_ShouldSucceed()
    {
        string? s = null;
        (s).Must().BeNullOrWhiteSpace();
    }

    [Fact]
    public void ToBeNullOrWhiteSpace_WhenWhiteSpaceOnly_ShouldSucceed()
    {
        ("   ").Must().BeNullOrWhiteSpace();
    }

    [Fact]
    public void ToBeNullOrWhiteSpace_WhenNotWhiteSpace_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("hello").Must().BeNullOrWhiteSpace());
    }

    [Fact]
    public void ToHaveLength_WhenLengthMatches_ShouldSucceed()
    {
        ("hello").Must().HaveLength(5);
    }

    [Fact]
    public void ToHaveLength_WhenLengthDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("abc").Must().HaveLength(2));
    }

    [Fact]
    public void ToHaveLengthGreaterThan_WhenLongEnough_ShouldSucceed()
    {
        ("hello").Must().HaveLengthGreaterThan(3);
    }

    [Fact]
    public void ToHaveLengthGreaterThan_WhenTooShort_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("abc").Must().HaveLengthGreaterThan(3));
    }

    [Fact]
    public void ToHaveLengthLessThan_WhenShortEnough_ShouldSucceed()
    {
        ("hello").Must().HaveLengthLessThan(10);
    }

    [Fact]
    public void ToHaveLengthLessThan_WhenTooLong_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("abc").Must().HaveLengthLessThan(3));
    }

    [Fact]
    public void ToContain_WhenSubstringPresent_ShouldSucceed()
    {
        ("abc").Must().Contain("b");
    }

    [Fact]
    public void ToContain_WhenSubstringAbsent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("abc").Must().Contain("z"));
    }

    [Fact]
    public void ToContain_WhenSubstringIsEmpty_ShouldSucceed()
    {
        ("abc").Must().Contain(string.Empty);
    }

    [Fact]
    public void ToContain_WithIgnoreCaseComparison_WhenMatch_ShouldSucceed()
    {
        ("AbC").Must().Contain("bc", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToContain_WithOrdinalComparison_WhenMissing_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            ("abc").Must().Contain("XX", StringComparison.Ordinal));
    }

    [Fact]
    public void ToContain_WithEmptySubstringAndComparison_ShouldSucceed()
    {
        ("abc").Must().Contain(string.Empty, StringComparison.Ordinal);
    }

    [Fact]
    public void ToStartWith_WhenMatchesPrefix_ShouldSucceed()
    {
        ("hello world").Must().StartWith("hello");
    }

    [Fact]
    public void ToStartWith_WhenIgnoreCase_ShouldSucceed()
    {
        ("Hello").Must().StartWith("hel", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToStartWith_WhenMismatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("hello").Must().StartWith("hi"));
    }

    [Fact]
    public void ToEndWith_WhenMatchesSuffix_ShouldSucceed()
    {
        ("hello world").Must().EndWith("world");
    }

    [Fact]
    public void ToEndWith_WhenIgnoreCase_ShouldSucceed()
    {
        ("Hello").Must().EndWith("LO", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToEndWith_WhenMismatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("hello").Must().EndWith("bye"));
    }

    [Fact]
    public void ToMatch_WhenPatternMatches_ShouldSucceed()
    {
        ("123-456").Must().Match(@"^\d{3}-\d{3}$");
    }

    [Fact]
    public void ToMatch_WhenPatternDoesNotMatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("123").Must().Match(@"^[a-z]+$"));
    }

    [Fact]
    public void ToMatch_WithRegexOptions_IgnoreCase_ShouldSucceed()
    {
        ("HELLO123").Must().Match(@"^hello\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }

    [Fact]
    public void ToMatch_WithRegexOptions_Multiline_ShouldSucceed()
    {
        ("line1\nline2\nline3").Must().Match(@"^line2$", System.Text.RegularExpressions.RegexOptions.Multiline);
    }

    [Fact]
    public void ToMatch_WithRegexOptions_IgnoreCase_WhenMismatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            ("ABC").Must().Match(@"^\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase));
    }

    [Fact]
    public void ToBe_WithDifferentStringComparisons_OrdinalCase_ShouldSucceed()
    {
        ("hello").Must().Be("hello", StringComparison.Ordinal);
    }

    [Fact]
    public void ToBe_WithDifferentStringComparisons_CurrentCultureIgnoreCase_ShouldSucceed()
    {
        ("HELLO").Must().Be("hello", StringComparison.CurrentCultureIgnoreCase);
    }

    [Fact]
    public void ToBe_WithDifferentStringComparisons_InvariantCulture_ShouldSucceed()
    {
        ("hello").Must().Be("hello", StringComparison.InvariantCulture);
    }

    [Fact]
    public void ToContain_WithUnicodeCharacters_ShouldSucceed()
    {
        ("café 123 naïve").Must().Contain("café");
    }

    [Fact]
    public void ToContain_WithSpecialCharacters_ShouldSucceed()
    {
        ("user@example.com").Must().Contain("@");
    }

    [Fact]
    public void ToContain_WithEmojiCharacters_ShouldSucceed()
    {
        ("Hello 👋 World").Must().Contain("👋");
    }

    [Fact]
    public void ToMatch_WithUnicodePattern_ShouldSucceed()
    {
        ("café").Must().Match("café");
    }

    [Fact]
    public void ToHaveLength_WithVeryLongString_ShouldSucceed()
    {
        var longStr = new string('x', 10000);
        longStr.Must().HaveLength(10000);
    }

    [Fact]
    public void ToContain_WithVeryLongString_ShouldSucceed()
    {
        var longStr = new string('x', 10000) + "needle" + new string('y', 10000);
        longStr.Must().Contain("needle");
    }

    [Fact]
    public void ToContain_WithWhitespaceOnlyString_ShouldSucceed()
    {
        ("   ").Must().Contain("   ");
    }

    [Fact]
    public void ToStartWith_WithEmptyPrefix_ShouldSucceed()
    {
        ("hello").Must().StartWith("");
    }

    [Fact]
    public void ToEndWith_WithEmptySuffix_ShouldSucceed()
    {
        ("hello").Must().EndWith("");
    }

    [Fact]
    public void ToBe_WithinScope_WhenMismatch_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            ("a").Must().Be("b");
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void ToBe_WithinScope_MultipleFailures_ShouldThrowAggregate()
    {
        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using var scope = new AssertionScope();
            ("a").Must().Be("b");
            ("c").Must().Be("d");
        });
        Xunit.Assert.Equal(2, ex.InnerExceptions.Count);
    }

    [Fact]
    public void NotToBeNullOrEmpty_WhenNotEmpty_ShouldSucceed()
    {
        ("hello").Must().NotBeNullOrEmpty();
    }

    [Fact]
    public void NotToBeNullOrEmpty_WhenEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("").Must().NotBeNullOrEmpty());
    }

    [Fact]
    public void NotToBeNullOrWhiteSpace_WhenNotWhiteSpace_ShouldSucceed()
    {
        ("hello").Must().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void NotToBeNullOrWhiteSpace_WhenWhiteSpace_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("   ").Must().NotBeNullOrWhiteSpace());
    }

    [Fact]
    public void ToBeWhiteSpace_WhenWhiteSpace_ShouldSucceed()
    {
        ("   ").Must().BeWhiteSpace();
    }

    [Fact]
    public void ToBeWhiteSpace_WhenEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("").Must().BeWhiteSpace());
    }

    [Fact]
    public void ToBeWhiteSpace_WhenNull_ShouldThrow()
    {
        string? s = null;
        Xunit.Assert.Throws<OmniAssertionException>(() => (s).Must().BeWhiteSpace());
    }

    [Fact]
    public void ToBeWhiteSpace_WhenNotWhiteSpace_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("hello").Must().BeWhiteSpace());
    }

    [Fact]
    public void NotToBeWhiteSpace_WhenNotWhiteSpace_ShouldSucceed()
    {
        ("hello").Must().NotBeWhiteSpace();
    }

    [Fact]
    public void NotToBeWhiteSpace_WhenNull_ShouldSucceed()
    {
        string? s = null;
        (s).Must().NotBeWhiteSpace();
    }

    [Fact]
    public void NotToBeWhiteSpace_WhenEmpty_ShouldSucceed()
    {
        ("").Must().NotBeWhiteSpace();
    }

    [Fact]
    public void NotToBeWhiteSpace_WhenWhiteSpace_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("   ").Must().NotBeWhiteSpace());
    }

    [Fact]
    public void NotToEndWith_WhenSuffixNotPresent_ShouldSucceed()
    {
        ("hello world").Must().NotEndWith("bye");
    }

    [Fact]
    public void NotToEndWith_WhenSuffixPresent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("hello world").Must().NotEndWith("world"));
    }

    [Fact]
    public void NotToEndWith_WithIgnoreCase_ShouldSucceed()
    {
        ("Hello").Must().NotEndWith("LO", StringComparison.Ordinal);
    }

    [Fact]
    public void NotToEndWith_WithIgnoreCase_WhenPresent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            ("Hello").Must().NotEndWith("lo", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void NotToEndWith_WithNullString_ShouldSucceed()
    {
        string? s = null;
        (s).Must().NotEndWith("suffix");
    }

    [Fact]
    public void NotToEndWith_WithEmptySuffix_WhenStringNotEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("hello").Must().NotEndWith(""));
    }
}
