using OmniAssert;
namespace OmniAssert.Tests;

public class StringAssertionTests
{
    // ── ToBe ────────────────────────────────────────────────────────────────

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

    // ── NotToBe ─────────────────────────────────────────────────────────────

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

    // ── ToBeNull / NotToBeNull ───────────────────────────────────────────────

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

    // ── ToBeEmpty / NotToBeEmpty ─────────────────────────────────────────────

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

    // ── ToBeNullOrEmpty ──────────────────────────────────────────────────────

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

    // ── ToBeNullOrWhiteSpace ─────────────────────────────────────────────────

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

    // ── HasLength ────────────────────────────────────────────────────────────

    [Fact]
    public void HasLength_WhenLengthMatches_ShouldSucceed()
    {
        ("hello").Verify().HasLength(5);
    }

    [Fact]
    public void HasLength_WhenLengthDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("abc").Verify().HasLength(2));
    }

    // ── HasLengthGreaterThan ─────────────────────────────────────────────────

    [Fact]
    public void HasLengthGreaterThan_WhenLongEnough_ShouldSucceed()
    {
        ("hello").Verify().HasLengthGreaterThan(3);
    }

    [Fact]
    public void HasLengthGreaterThan_WhenTooShort_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("abc").Verify().HasLengthGreaterThan(3));
    }

    // ── HasLengthLessThan ────────────────────────────────────────────────────

    [Fact]
    public void HasLengthLessThan_WhenShortEnough_ShouldSucceed()
    {
        ("hello").Verify().HasLengthLessThan(10);
    }

    [Fact]
    public void HasLengthLessThan_WhenTooLong_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ("abc").Verify().HasLengthLessThan(3));
    }

    // ── ToContain ────────────────────────────────────────────────────────────

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

    // ── ToStartWith ──────────────────────────────────────────────────────────

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

    // ── ToEndWith ────────────────────────────────────────────────────────────

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

    // ── ToMatch ──────────────────────────────────────────────────────────────

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

    // ── Scope ────────────────────────────────────────────────────────────────

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
}
