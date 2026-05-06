using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class EnumAssertionTests
{
    public enum TestEnum
    {
        First,
        Second,
        Third
    }

    // ── ToBe ────────────────────────────────────────────────────────────────

    [Fact]
    public void ToBe_WhenValuesEqual_ShouldSucceed()
    {
        Verify(TestEnum.First).ToBe(TestEnum.First);
    }

    [Fact]
    public void ToBe_WhenValuesDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(TestEnum.First).ToBe(TestEnum.Second));
    }

    [Fact]
    public void ToBe_WhenValuesDiffer_MessageContainsExpectedAndActual()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(TestEnum.First).ToBe(TestEnum.Second));
        Xunit.Assert.Contains("Second", ex.Message, StringComparison.Ordinal);
        Xunit.Assert.Contains("First", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ToBe_WithinScope_WhenValuesDiffer_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            Verify(TestEnum.First).ToBe(TestEnum.Second);
        });
        Xunit.Assert.Contains("Second", ex.Message, StringComparison.Ordinal);
    }

    // ── NotToBe ─────────────────────────────────────────────────────────────

    [Fact]
    public void NotToBe_WhenValuesDiffer_ShouldSucceed()
    {
        Verify(TestEnum.First).NotToBe(TestEnum.Second);
    }

    [Fact]
    public void NotToBe_WhenValuesEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(TestEnum.First).NotToBe(TestEnum.First));
    }

    [Fact]
    public void NotToBe_WhenValuesEqual_MessageContainsValue()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(TestEnum.Third).NotToBe(TestEnum.Third));
        Xunit.Assert.Contains("Third", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NotToBe_WithinScope_WhenValuesEqual_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            Verify(TestEnum.First).NotToBe(TestEnum.First);
        });
        Xunit.Assert.NotNull(ex);
    }
}
