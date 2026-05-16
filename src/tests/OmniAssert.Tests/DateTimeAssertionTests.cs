using OmniAssert;
namespace OmniAssert.Tests;

public class DateTimeAssertionTests
{
    // ── DateTime.ToBeAfter ───────────────────────────────────────────────────

    [Fact]
    public void ToBeAfter_WhenAfter_ShouldSucceed()
    {
        var now = DateTime.UtcNow;
        (now).Verify().ToBeAfter(now.AddSeconds(-1));
    }

    [Fact]
    public void ToBeAfter_WhenBefore_ShouldThrow()
    {
        var now = DateTime.UtcNow;
        Xunit.Assert.Throws<OmniAssertionException>(() => (now).Verify().ToBeAfter(now.AddSeconds(1)));
    }

    [Fact]
    public void ToBeAfter_WhenEqual_ShouldThrow()
    {
        var now = DateTime.UtcNow;
        Xunit.Assert.Throws<OmniAssertionException>(() => (now).Verify().ToBeAfter(now));
    }

    // ── DateTime.ToBeBefore ──────────────────────────────────────────────────

    [Fact]
    public void ToBeBefore_WhenBefore_ShouldSucceed()
    {
        var now = DateTime.UtcNow;
        (now).Verify().ToBeBefore(now.AddSeconds(1));
    }

    [Fact]
    public void ToBeBefore_WhenAfter_ShouldThrow()
    {
        var now = DateTime.UtcNow;
        Xunit.Assert.Throws<OmniAssertionException>(() => (now).Verify().ToBeBefore(now.AddSeconds(-1)));
    }

    [Fact]
    public void ToBeBefore_WhenEqual_ShouldThrow()
    {
        var now = DateTime.UtcNow;
        Xunit.Assert.Throws<OmniAssertionException>(() => (now).Verify().ToBeBefore(now));
    }

    // ── DateTime.ToBeWithin ──────────────────────────────────────────────────

    [Fact]
    public void ToBeWithin_WhenWithinPrecision_ShouldSucceed()
    {
        var now = DateTime.UtcNow;
        (now).Verify().ToBeWithin(TimeSpan.FromSeconds(1), now.AddMilliseconds(500));
    }

    [Fact]
    public void ToBeWithin_WhenOutsidePrecision_ShouldThrow()
    {
        var now = DateTime.UtcNow;
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (now).Verify().ToBeWithin(TimeSpan.FromMilliseconds(100), now.AddSeconds(1)));
    }

    // ── DateTime scope ───────────────────────────────────────────────────────

    [Fact]
    public void ToBeAfter_WithinScope_WhenBefore_ShouldCollectRatherThanThrow()
    {
        var now = DateTime.UtcNow;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (now).Verify().ToBeAfter(now.AddSeconds(1));
        });
        Xunit.Assert.NotNull(ex);
    }

    // ── DateTimeOffset.ToBeAfter ─────────────────────────────────────────────

    [Fact]
    public void DateTimeOffset_ToBeAfter_WhenAfter_ShouldSucceed()
    {
        var now = DateTimeOffset.UtcNow;
        (now).Verify().ToBeAfter(now.AddSeconds(-1));
    }

    [Fact]
    public void DateTimeOffset_ToBeAfter_WhenNotAfter_ShouldThrow()
    {
        var t = DateTimeOffset.Parse("2020-01-01T00:00:00Z");
        Xunit.Assert.Throws<OmniAssertionException>(() => (t).Verify().ToBeAfter(t.AddDays(1)));
    }

    // ── DateTimeOffset.ToBeBefore ────────────────────────────────────────────

    [Fact]
    public void DateTimeOffset_ToBeBefore_WhenBefore_ShouldSucceed()
    {
        var now = DateTimeOffset.UtcNow;
        (now).Verify().ToBeBefore(now.AddSeconds(1));
    }

    [Fact]
    public void DateTimeOffset_ToBeBefore_WhenNotBefore_ShouldThrow()
    {
        var t = DateTimeOffset.Parse("2020-06-01T00:00:00Z");
        Xunit.Assert.Throws<OmniAssertionException>(() => (t).Verify().ToBeBefore(t.AddDays(-1)));
    }

    // ── DateTimeOffset.ToBeWithin ────────────────────────────────────────────

    [Fact]
    public void DateTimeOffset_ToBeWithin_WhenWithinPrecision_ShouldSucceed()
    {
        var now = DateTimeOffset.UtcNow;
        (now).Verify().ToBeWithin(TimeSpan.FromSeconds(1), now.AddMilliseconds(500));
    }

    [Fact]
    public void DateTimeOffset_ToBeWithin_WhenOutside_ShouldThrow()
    {
        var t = DateTimeOffset.Parse("2030-01-01T12:00:00Z");
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (t).Verify().ToBeWithin(TimeSpan.FromSeconds(1), t.AddHours(1)));
    }

    // ── DateTimeOffset scope ─────────────────────────────────────────────────

    [Fact]
    public void DateTimeOffset_ToBeAfter_WithinScope_WhenBefore_ShouldCollectRatherThanThrow()
    {
        var t = DateTimeOffset.UtcNow;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (t).Verify().ToBeAfter(t.AddDays(1));
        });
        Xunit.Assert.NotNull(ex);
    }
}
