namespace OmniAssert.Tests;

public class DateTimeAssertionTests
{
    [Fact]
    public void ToBeAfter_WhenAfter_ShouldSucceed()
    {
        var now = DateTime.UtcNow;
        (now).Must().BeAfter(now.AddSeconds(-1));
    }

    [Fact]
    public void ToBeAfter_WhenBefore_ShouldThrow()
    {
        var now = DateTime.UtcNow;
        Xunit.Assert.Throws<OmniAssertionException>(() => (now).Must().BeAfter(now.AddSeconds(1)));
    }

    [Fact]
    public void ToBeAfter_WhenEqual_ShouldThrow()
    {
        var now = DateTime.UtcNow;
        Xunit.Assert.Throws<OmniAssertionException>(() => (now).Must().BeAfter(now));
    }

    [Fact]
    public void ToBeBefore_WhenBefore_ShouldSucceed()
    {
        var now = DateTime.UtcNow;
        (now).Must().BeBefore(now.AddSeconds(1));
    }

    [Fact]
    public void ToBeBefore_WhenAfter_ShouldThrow()
    {
        var now = DateTime.UtcNow;
        Xunit.Assert.Throws<OmniAssertionException>(() => (now).Must().BeBefore(now.AddSeconds(-1)));
    }

    [Fact]
    public void ToBeBefore_WhenEqual_ShouldThrow()
    {
        var now = DateTime.UtcNow;
        Xunit.Assert.Throws<OmniAssertionException>(() => (now).Must().BeBefore(now));
    }

    [Fact]
    public void ToBeWithin_WhenWithinPrecision_ShouldSucceed()
    {
        var now = DateTime.UtcNow;
        (now).Must().BeWithin(TimeSpan.FromSeconds(1), now.AddMilliseconds(500));
    }

    [Fact]
    public void ToBeWithin_WhenOutsidePrecision_ShouldThrow()
    {
        var now = DateTime.UtcNow;
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (now).Must().BeWithin(TimeSpan.FromMilliseconds(100), now.AddSeconds(1)));
    }

    [Fact]
    public void ToBeAfter_WithinScope_WhenBefore_ShouldCollectRatherThanThrow()
    {
        var now = DateTime.UtcNow;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (now).Must().BeAfter(now.AddSeconds(1));
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void DateTimeOffset_ToBeAfter_WhenAfter_ShouldSucceed()
    {
        var now = DateTimeOffset.UtcNow;
        (now).Must().BeAfter(now.AddSeconds(-1));
    }

    [Fact]
    public void DateTimeOffset_ToBeAfter_WhenNotAfter_ShouldThrow()
    {
        var t = DateTimeOffset.Parse("2020-01-01T00:00:00Z");
        Xunit.Assert.Throws<OmniAssertionException>(() => (t).Must().BeAfter(t.AddDays(1)));
    }

    [Fact]
    public void DateTimeOffset_ToBeBefore_WhenBefore_ShouldSucceed()
    {
        var now = DateTimeOffset.UtcNow;
        (now).Must().BeBefore(now.AddSeconds(1));
    }

    [Fact]
    public void DateTimeOffset_ToBeBefore_WhenNotBefore_ShouldThrow()
    {
        var t = DateTimeOffset.Parse("2020-06-01T00:00:00Z");
        Xunit.Assert.Throws<OmniAssertionException>(() => (t).Must().BeBefore(t.AddDays(-1)));
    }

    [Fact]
    public void DateTimeOffset_ToBeWithin_WhenWithinPrecision_ShouldSucceed()
    {
        var now = DateTimeOffset.UtcNow;
        (now).Must().BeWithin(TimeSpan.FromSeconds(1), now.AddMilliseconds(500));
    }

    [Fact]
    public void DateTimeOffset_ToBeWithin_WhenOutside_ShouldThrow()
    {
        var t = DateTimeOffset.Parse("2030-01-01T12:00:00Z");
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (t).Must().BeWithin(TimeSpan.FromSeconds(1), t.AddHours(1)));
    }

    [Fact]
    public void DateTime_ToBeAfter_WithMinDateTime_ShouldSucceed()
    {
        DateTime.MinValue.AddSeconds(1).Must().BeAfter(DateTime.MinValue);
    }

    [Fact]
    public void DateTime_ToBeBefore_WithMaxDateTime_ShouldSucceed()
    {
        DateTime.MaxValue.AddSeconds(-1).Must().BeBefore(DateTime.MaxValue);
    }

    [Fact]
    public void DateTime_ToBeWithin_WithLargeTimespan_ShouldSucceed()
    {
        var t = DateTime.Parse("2020-01-01");
        t.Must().BeWithin(TimeSpan.FromDays(365), t.AddDays(180));
    }

    [Fact]
    public void DateTimeOffset_ToBeAfter_WithMinValue_ShouldSucceed()
    {
        DateTimeOffset.MinValue.AddSeconds(1).Must().BeAfter(DateTimeOffset.MinValue);
    }

    [Fact]
    public void DateTimeOffset_ToBeBefore_WithMaxValue_ShouldSucceed()
    {
        DateTimeOffset.MaxValue.AddSeconds(-1).Must().BeBefore(DateTimeOffset.MaxValue);
    }

    [Fact]
    public void DateTimeOffset_ToBeAfter_WithinScope_WhenBefore_ShouldCollectRatherThanThrow()
    {
        var t = DateTimeOffset.UtcNow;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (t).Must().BeAfter(t.AddDays(1));
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void DateTime_ToBe_WhenEqual_ShouldSucceed()
    {
        var now = DateTime.UtcNow;
        now.Must().Be(now);
    }

    [Fact]
    public void DateTime_ToBe_WhenDifferent_ShouldThrow()
    {
        var now = DateTime.UtcNow;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => now.Must().Be(now.AddSeconds(1)));
        Xunit.Assert.Contains("to be", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DateTime_NotToBe_WhenDifferent_ShouldSucceed()
    {
        var now = DateTime.UtcNow;
        now.Must().NotBe(now.AddSeconds(1));
    }

    [Fact]
    public void DateTime_NotToBe_WhenEqual_ShouldThrow()
    {
        var now = DateTime.UtcNow;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => now.Must().NotBe(now));
        Xunit.Assert.Contains("not to be", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DateTimeOffset_ToBe_WhenEqual_ShouldSucceed()
    {
        var now = DateTimeOffset.UtcNow;
        now.Must().Be(now);
    }

    [Fact]
    public void DateTimeOffset_ToBe_WhenDifferent_ShouldThrow()
    {
        var now = DateTimeOffset.UtcNow;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => now.Must().Be(now.AddSeconds(1)));
        Xunit.Assert.Contains("to be", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DateTimeOffset_NotToBe_WhenDifferent_ShouldSucceed()
    {
        var now = DateTimeOffset.UtcNow;
        now.Must().NotBe(now.AddSeconds(1));
    }

    [Fact]
    public void DateTimeOffset_NotToBe_WhenEqual_ShouldThrow()
    {
        var now = DateTimeOffset.UtcNow;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => now.Must().NotBe(now));
        Xunit.Assert.Contains("not to be", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
