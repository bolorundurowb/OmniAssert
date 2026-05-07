using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class StringAssertionTests
{
    // ── ToBe ────────────────────────────────────────────────────────────────

    [Fact]
    public void ToBe_WhenStringsEqual_ShouldSucceed()
    {
        Verify("hello").ToBe("hello");
    }

    [Fact]
    public void ToBe_WhenStringsDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("abc").ToBe("xyz"));
    }

    [Fact]
    public void ToBe_WithOrdinalIgnoreCaseComparison_WhenMatch_ShouldSucceed()
    {
        Verify("HELLO").ToBe("hello", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToBe_WithOrdinalIgnoreCaseComparison_WhenMismatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("abc").ToBe("xyz", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ToBeIgnoringCase_WhenStringsMatchIgnoringCase_ShouldSucceed()
    {
        Verify("HELLO").ToBeIgnoringCase("hello");
    }

    [Fact]
    public void ToBeIgnoringCase_WhenStringsDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("abc").ToBeIgnoringCase("xyz"));
    }

    [Fact]
    public void ToBeOneOf_WhenStringIsInSet_ShouldSucceed()
    {
        Verify("pending").ToBeOneOf("active", "pending", "archived");
    }

    [Fact]
    public void ToBeOneOf_WhenStringIsNotInSet_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("deleted").ToBeOneOf("active", "pending", "archived"));
    }

    // ── NotToBe ─────────────────────────────────────────────────────────────

    [Fact]
    public void NotToBe_WhenStringsDiffer_ShouldSucceed()
    {
        Verify("a").NotToBe("b");
    }

    [Fact]
    public void NotToBe_WhenStringsEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("same").NotToBe("same"));
    }

    // ── ToBeNull / NotToBeNull ───────────────────────────────────────────────

    [Fact]
    public void ToBeNull_WhenNull_ShouldSucceed()
    {
        string? s = null;
        Verify(s).ToBeNull();
    }

    [Fact]
    public void ToBeNull_WhenNotNull_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("x").ToBeNull());
    }

    [Fact]
    public void NotToBeNull_WhenNotNull_ShouldSucceed()
    {
        Verify("ok").NotToBeNull();
    }

    [Fact]
    public void NotToBeNull_WhenNull_ShouldThrow()
    {
        string? s = null;
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(s).NotToBeNull());
    }

    // ── ToBeEmpty / NotToBeEmpty ─────────────────────────────────────────────

    [Fact]
    public void ToBeEmpty_WhenEmpty_ShouldSucceed()
    {
        Verify("").ToBeEmpty();
    }

    [Fact]
    public void ToBeEmpty_WhenNotEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("x").ToBeEmpty());
    }

    [Fact]
    public void NotToBeEmpty_WhenNotEmpty_ShouldSucceed()
    {
        Verify("hello").NotToBeEmpty();
    }

    [Fact]
    public void NotToBeEmpty_WhenEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("").NotToBeEmpty());
    }

    // ── ToBeNullOrEmpty ──────────────────────────────────────────────────────

    [Fact]
    public void ToBeNullOrEmpty_WhenNull_ShouldSucceed()
    {
        string? s = null;
        Verify(s).ToBeNullOrEmpty();
    }

    [Fact]
    public void ToBeNullOrEmpty_WhenEmpty_ShouldSucceed()
    {
        Verify("").ToBeNullOrEmpty();
    }

    [Fact]
    public void ToBeNullOrEmpty_WhenNotEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("hello").ToBeNullOrEmpty());
    }

    // ── ToBeNullOrWhiteSpace ─────────────────────────────────────────────────

    [Fact]
    public void ToBeNullOrWhiteSpace_WhenNull_ShouldSucceed()
    {
        string? s = null;
        Verify(s).ToBeNullOrWhiteSpace();
    }

    [Fact]
    public void ToBeNullOrWhiteSpace_WhenWhiteSpaceOnly_ShouldSucceed()
    {
        Verify("   ").ToBeNullOrWhiteSpace();
    }

    [Fact]
    public void ToBeNullOrWhiteSpace_WhenNotWhiteSpace_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("hello").ToBeNullOrWhiteSpace());
    }

    // ── HasLength ────────────────────────────────────────────────────────────

    [Fact]
    public void HasLength_WhenLengthMatches_ShouldSucceed()
    {
        Verify("hello").HasLength(5);
    }

    [Fact]
    public void HasLength_WhenLengthDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("abc").HasLength(2));
    }

    // ── HasLengthGreaterThan ─────────────────────────────────────────────────

    [Fact]
    public void HasLengthGreaterThan_WhenLongEnough_ShouldSucceed()
    {
        Verify("hello").HasLengthGreaterThan(3);
    }

    [Fact]
    public void HasLengthGreaterThan_WhenTooShort_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("abc").HasLengthGreaterThan(3));
    }

    // ── HasLengthLessThan ────────────────────────────────────────────────────

    [Fact]
    public void HasLengthLessThan_WhenShortEnough_ShouldSucceed()
    {
        Verify("hello").HasLengthLessThan(10);
    }

    [Fact]
    public void HasLengthLessThan_WhenTooLong_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("abc").HasLengthLessThan(3));
    }

    // ── ToContain ────────────────────────────────────────────────────────────

    [Fact]
    public void ToContain_WhenSubstringPresent_ShouldSucceed()
    {
        Verify("abc").ToContain("b");
    }

    [Fact]
    public void ToContain_WhenSubstringAbsent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("abc").ToContain("z"));
    }

    [Fact]
    public void ToContain_WhenSubstringIsEmpty_ShouldSucceed()
    {
        Verify("abc").ToContain(string.Empty);
    }

    [Fact]
    public void ToContain_WithIgnoreCaseComparison_WhenMatch_ShouldSucceed()
    {
        Verify("AbC").ToContain("bc", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToContain_WithOrdinalComparison_WhenMissing_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify("abc").ToContain("XX", StringComparison.Ordinal));
    }

    [Fact]
    public void ToContain_WithEmptySubstringAndComparison_ShouldSucceed()
    {
        Verify("abc").ToContain(string.Empty, StringComparison.Ordinal);
    }

    // ── ToStartWith ──────────────────────────────────────────────────────────

    [Fact]
    public void ToStartWith_WhenMatchesPrefix_ShouldSucceed()
    {
        Verify("hello world").ToStartWith("hello");
    }

    [Fact]
    public void ToStartWith_WhenIgnoreCase_ShouldSucceed()
    {
        Verify("Hello").ToStartWith("hel", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToStartWith_WhenMismatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("hello").ToStartWith("hi"));
    }

    // ── ToEndWith ────────────────────────────────────────────────────────────

    [Fact]
    public void ToEndWith_WhenMatchesSuffix_ShouldSucceed()
    {
        Verify("hello world").ToEndWith("world");
    }

    [Fact]
    public void ToEndWith_WhenIgnoreCase_ShouldSucceed()
    {
        Verify("Hello").ToEndWith("LO", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToEndWith_WhenMismatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("hello").ToEndWith("bye"));
    }

    // ── ToMatch ──────────────────────────────────────────────────────────────

    [Fact]
    public void ToMatch_WhenPatternMatches_ShouldSucceed()
    {
        Verify("123-456").ToMatch(@"^\d{3}-\d{3}$");
    }

    [Fact]
    public void ToMatch_WhenPatternDoesNotMatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("123").ToMatch(@"^[a-z]+$"));
    }

    // ── Scope ────────────────────────────────────────────────────────────────

    [Fact]
    public void ToBe_WithinScope_WhenMismatch_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            Verify("a").ToBe("b");
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void ToBe_WithinScope_MultipleFailures_ShouldThrowAggregate()
    {
        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using var scope = new AssertionScope();
            Verify("a").ToBe("b");
            Verify("c").ToBe("d");
        });
        Xunit.Assert.Equal(2, ex.InnerExceptions.Count);
    }
}
