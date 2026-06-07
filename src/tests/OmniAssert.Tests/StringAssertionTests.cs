namespace OmniAssert.Tests;

public class StringAssertionTests
{
    [Fact]
    public void ToBe_WhenStringsEqual_ShouldSucceed()
    {
        ("hello").Verify().ToBe("hello");
    }

    [Fact]
    public void ToBe_WhenStringsDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("abc").Verify().ToBe("xyz"));
    }

    [Fact]
    public void ToBe_WithOrdinalIgnoreCaseComparison_WhenMatch_ShouldSucceed()
    {
        ("HELLO").Verify().ToBe("hello", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToBe_WithOrdinalIgnoreCaseComparison_WhenMismatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("abc").Verify().ToBe("xyz", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ToBeIgnoringCase_WhenStringsMatchIgnoringCase_ShouldSucceed()
    {
        ("HELLO").Verify().ToBeIgnoringCase("hello");
    }

    [Fact]
    public void ToBeIgnoringCase_WhenStringsDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("abc").Verify().ToBeIgnoringCase("xyz"));
    }

    [Fact]
    public void ToBeOneOf_WhenStringIsInSet_ShouldSucceed()
    {
        ("pending").Verify().ToBeOneOf("active", "pending", "archived");
    }

    [Fact]
    public void ToBeOneOf_WhenStringIsNotInSet_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("deleted").Verify().ToBeOneOf("active", "pending", "archived"));
    }

    [Fact]
    public void NotToBe_WhenStringsDiffer_ShouldSucceed()
    {
        ("a").Verify().NotToBe("b");
    }

    [Fact]
    public void NotToBe_WhenStringsEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("same").Verify().NotToBe("same"));
    }

    [Fact]
    public void ToBeNull_WhenNull_ShouldSucceed()
    {
        string? s = null;
        (s).Verify().ToBeNull();
    }

    [Fact]
    public void ToBeNull_WhenNotNull_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("x").Verify().ToBeNull());
    }

    [Fact]
    public void NotToBeNull_WhenNotNull_ShouldSucceed()
    {
        ("ok").Verify().NotToBeNull();
    }

    [Fact]
    public void NotToBeNull_WhenNull_ShouldThrow()
    {
        string? s = null;
        Xunit.Assert.Throws<OmniAssertionException>(() => (s).Verify().NotToBeNull());
    }

    [Fact]
    public void ToBeEmpty_WhenEmpty_ShouldSucceed()
    {
        ("").Verify().ToBeEmpty();
    }

    [Fact]
    public void ToBeEmpty_WhenNotEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("x").Verify().ToBeEmpty());
    }

    [Fact]
    public void NotToBeEmpty_WhenNotEmpty_ShouldSucceed()
    {
        ("hello").Verify().NotToBeEmpty();
    }

    [Fact]
    public void NotToBeEmpty_WhenEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("").Verify().NotToBeEmpty());
    }

    [Fact]
    public void ToBeNullOrEmpty_WhenNull_ShouldSucceed()
    {
        string? s = null;
        (s).Verify().ToBeNullOrEmpty();
    }

    [Fact]
    public void ToBeNullOrEmpty_WhenEmpty_ShouldSucceed()
    {
        ("").Verify().ToBeNullOrEmpty();
    }

    [Fact]
    public void ToBeNullOrEmpty_WhenNotEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("hello").Verify().ToBeNullOrEmpty());
    }

    [Fact]
    public void ToBeNullOrWhiteSpace_WhenNull_ShouldSucceed()
    {
        string? s = null;
        (s).Verify().ToBeNullOrWhiteSpace();
    }

    [Fact]
    public void ToBeNullOrWhiteSpace_WhenWhiteSpaceOnly_ShouldSucceed()
    {
        ("   ").Verify().ToBeNullOrWhiteSpace();
    }

    [Fact]
    public void ToBeNullOrWhiteSpace_WhenNotWhiteSpace_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("hello").Verify().ToBeNullOrWhiteSpace());
    }

    [Fact]
    public void ToHaveLength_WhenLengthMatches_ShouldSucceed()
    {
        ("hello").Verify().ToHaveLength(5);
    }

    [Fact]
    public void ToHaveLength_WhenLengthDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("abc").Verify().ToHaveLength(2));
    }

    [Fact]
    public void ToHaveLengthGreaterThan_WhenLongEnough_ShouldSucceed()
    {
        ("hello").Verify().ToHaveLengthGreaterThan(3);
    }

    [Fact]
    public void ToHaveLengthGreaterThan_WhenTooShort_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("abc").Verify().ToHaveLengthGreaterThan(3));
    }

    [Fact]
    public void ToHaveLengthLessThan_WhenShortEnough_ShouldSucceed()
    {
        ("hello").Verify().ToHaveLengthLessThan(10);
    }

    [Fact]
    public void ToHaveLengthLessThan_WhenTooLong_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("abc").Verify().ToHaveLengthLessThan(3));
    }

    [Fact]
    public void ToContain_WhenSubstringPresent_ShouldSucceed()
    {
        ("abc").Verify().ToContain("b");
    }

    [Fact]
    public void ToContain_WhenSubstringAbsent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("abc").Verify().ToContain("z"));
    }

    [Fact]
    public void ToContain_WhenSubstringIsEmpty_ShouldSucceed()
    {
        ("abc").Verify().ToContain(string.Empty);
    }

    [Fact]
    public void ToContain_WithIgnoreCaseComparison_WhenMatch_ShouldSucceed()
    {
        ("AbC").Verify().ToContain("bc", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToContain_WithOrdinalComparison_WhenMissing_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            ("abc").Verify().ToContain("XX", StringComparison.Ordinal));
    }

    [Fact]
    public void ToContain_WithEmptySubstringAndComparison_ShouldSucceed()
    {
        ("abc").Verify().ToContain(string.Empty, StringComparison.Ordinal);
    }

    [Fact]
    public void ToStartWith_WhenMatchesPrefix_ShouldSucceed()
    {
        ("hello world").Verify().ToStartWith("hello");
    }

    [Fact]
    public void ToStartWith_WhenIgnoreCase_ShouldSucceed()
    {
        ("Hello").Verify().ToStartWith("hel", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToStartWith_WhenMismatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("hello").Verify().ToStartWith("hi"));
    }

    [Fact]
    public void ToEndWith_WhenMatchesSuffix_ShouldSucceed()
    {
        ("hello world").Verify().ToEndWith("world");
    }

    [Fact]
    public void ToEndWith_WhenIgnoreCase_ShouldSucceed()
    {
        ("Hello").Verify().ToEndWith("LO", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToEndWith_WhenMismatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("hello").Verify().ToEndWith("bye"));
    }

    [Fact]
    public void ToMatch_WhenPatternMatches_ShouldSucceed()
    {
        ("123-456").Verify().ToMatch(@"^\d{3}-\d{3}$");
    }

    [Fact]
    public void ToMatch_WhenPatternDoesNotMatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("123").Verify().ToMatch(@"^[a-z]+$"));
    }

    [Fact]
    public void ToMatch_WithRegexOptions_IgnoreCase_ShouldSucceed()
    {
        ("HELLO123").Verify().ToMatch(@"^hello\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }

    [Fact]
    public void ToMatch_WithRegexOptions_Multiline_ShouldSucceed()
    {
        ("line1\nline2\nline3").Verify().ToMatch(@"^line2$", System.Text.RegularExpressions.RegexOptions.Multiline);
    }

    [Fact]
    public void ToMatch_WithRegexOptions_IgnoreCase_WhenMismatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            ("ABC").Verify().ToMatch(@"^\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase));
    }

    [Fact]
    public void ToBe_WithDifferentStringComparisons_OrdinalCase_ShouldSucceed()
    {
        ("hello").Verify().ToBe("hello", StringComparison.Ordinal);
    }

    [Fact]
    public void ToBe_WithDifferentStringComparisons_CurrentCultureIgnoreCase_ShouldSucceed()
    {
        ("HELLO").Verify().ToBe("hello", StringComparison.CurrentCultureIgnoreCase);
    }

    [Fact]
    public void ToBe_WithDifferentStringComparisons_InvariantCulture_ShouldSucceed()
    {
        ("hello").Verify().ToBe("hello", StringComparison.InvariantCulture);
    }

    [Fact]
    public void ToContain_WithUnicodeCharacters_ShouldSucceed()
    {
        ("café 123 naïve").Verify().ToContain("café");
    }

    [Fact]
    public void ToContain_WithSpecialCharacters_ShouldSucceed()
    {
        ("user@example.com").Verify().ToContain("@");
    }

    [Fact]
    public void ToContain_WithEmojiCharacters_ShouldSucceed()
    {
        ("Hello 👋 World").Verify().ToContain("👋");
    }

    [Fact]
    public void ToMatch_WithUnicodePattern_ShouldSucceed()
    {
        ("café").Verify().ToMatch("café");
    }

    [Fact]
    public void ToHaveLength_WithVeryLongString_ShouldSucceed()
    {
        var longStr = new string('x', 10000);
        longStr.Verify().ToHaveLength(10000);
    }

    [Fact]
    public void ToContain_WithVeryLongString_ShouldSucceed()
    {
        var longStr = new string('x', 10000) + "needle" + new string('y', 10000);
        longStr.Verify().ToContain("needle");
    }

    [Fact]
    public void ToContain_WithWhitespaceOnlyString_ShouldSucceed()
    {
        ("   ").Verify().ToContain("   ");
    }

    [Fact]
    public void ToStartWith_WithEmptyPrefix_ShouldSucceed()
    {
        ("hello").Verify().ToStartWith("");
    }

    [Fact]
    public void ToEndWith_WithEmptySuffix_ShouldSucceed()
    {
        ("hello").Verify().ToEndWith("");
    }

    [Fact]
    public void ToBe_WithinScope_WhenMismatch_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            ("a").Verify().ToBe("b");
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void ToBe_WithinScope_MultipleFailures_ShouldThrowAggregate()
    {
        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using var scope = new AssertionScope();
            ("a").Verify().ToBe("b");
            ("c").Verify().ToBe("d");
        });
        Xunit.Assert.Equal(2, ex.InnerExceptions.Count);
    }

    [Fact]
    public void NotToBeNullOrEmpty_WhenNotEmpty_ShouldSucceed()
    {
        ("hello").Verify().NotToBeNullOrEmpty();
    }

    [Fact]
    public void NotToBeNullOrEmpty_WhenEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("").Verify().NotToBeNullOrEmpty());
    }

    [Fact]
    public void NotToBeNullOrWhiteSpace_WhenNotWhiteSpace_ShouldSucceed()
    {
        ("hello").Verify().NotToBeNullOrWhiteSpace();
    }

    [Fact]
    public void NotToBeNullOrWhiteSpace_WhenWhiteSpace_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("   ").Verify().NotToBeNullOrWhiteSpace());
    }

    [Fact]
    public void ToBeWhiteSpace_WhenWhiteSpace_ShouldSucceed()
    {
        ("   ").Verify().ToBeWhiteSpace();
    }

    [Fact]
    public void ToBeWhiteSpace_WhenEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("").Verify().ToBeWhiteSpace());
    }

    [Fact]
    public void ToBeWhiteSpace_WhenNull_ShouldThrow()
    {
        string? s = null;
        Xunit.Assert.Throws<OmniAssertionException>(() => (s).Verify().ToBeWhiteSpace());
    }

    [Fact]
    public void ToBeWhiteSpace_WhenNotWhiteSpace_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("hello").Verify().ToBeWhiteSpace());
    }

    [Fact]
    public void NotToBeWhiteSpace_WhenNotWhiteSpace_ShouldSucceed()
    {
        ("hello").Verify().NotToBeWhiteSpace();
    }

    [Fact]
    public void NotToBeWhiteSpace_WhenNull_ShouldSucceed()
    {
        string? s = null;
        (s).Verify().NotToBeWhiteSpace();
    }

    [Fact]
    public void NotToBeWhiteSpace_WhenEmpty_ShouldSucceed()
    {
        ("").Verify().NotToBeWhiteSpace();
    }

    [Fact]
    public void NotToBeWhiteSpace_WhenWhiteSpace_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("   ").Verify().NotToBeWhiteSpace());
    }

    [Fact]
    public void NotToEndWith_WhenSuffixNotPresent_ShouldSucceed()
    {
        ("hello world").Verify().NotToEndWith("bye");
    }

    [Fact]
    public void NotToEndWith_WhenSuffixPresent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("hello world").Verify().NotToEndWith("world"));
    }

    [Fact]
    public void NotToEndWith_WithIgnoreCase_ShouldSucceed()
    {
        ("Hello").Verify().NotToEndWith("LO", StringComparison.Ordinal);
    }

    [Fact]
    public void NotToEndWith_WithIgnoreCase_WhenPresent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            ("Hello").Verify().NotToEndWith("lo", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void NotToEndWith_WithNullString_ShouldSucceed()
    {
        string? s = null;
        (s).Verify().NotToEndWith("suffix");
    }

    [Fact]
    public void NotToEndWith_WithEmptySuffix_WhenStringNotEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("hello").Verify().NotToEndWith(""));
    }
}
