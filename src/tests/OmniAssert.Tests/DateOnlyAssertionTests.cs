namespace OmniAssert.Tests;

public class DateOnlyAssertionTests
{
    [Fact]
    public void ToBe_WhenDatesEqual_ShouldSucceed()
    {
        (new DateOnly(2026, 5, 7)).Must().Be(new DateOnly(2026, 5, 7));
    }

    [Fact]
    public void ToBe_WhenDatesDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new DateOnly(2026, 5, 7)).Must().Be(new DateOnly(2026, 5, 8)));
    }

    [Fact]
    public void ToBeBefore_WhenEarlier_ShouldSucceed()
    {
        (new DateOnly(2026, 5, 7)).Must().BeBefore(new DateOnly(2026, 5, 8));
    }

    [Fact]
    public void ToBeBefore_WhenEqualOrLater_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new DateOnly(2026, 5, 7)).Must().BeBefore(new DateOnly(2026, 5, 7)));
    }

    [Fact]
    public void ToBeAfter_WhenLater_ShouldSucceed()
    {
        (new DateOnly(2026, 5, 8)).Must().BeAfter(new DateOnly(2026, 5, 7));
    }

    [Fact]
    public void ToBeAfter_WhenEqualOrEarlier_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new DateOnly(2026, 5, 7)).Must().BeAfter(new DateOnly(2026, 5, 7)));
    }

    [Fact]
    public void ToHaveYear_WhenYearMatches_ShouldSucceed()
    {
        (new DateOnly(2026, 5, 7)).Must().HaveYear(2026);
    }

    [Fact]
    public void ToHaveYear_WhenYearDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new DateOnly(2026, 5, 7)).Must().HaveYear(2025));
    }

    [Fact]
    public void ToHaveMonth_WhenMonthMatches_ShouldSucceed()
    {
        (new DateOnly(2026, 5, 7)).Must().HaveMonth(5);
    }

    [Fact]
    public void ToHaveMonth_WhenMonthDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new DateOnly(2026, 5, 7)).Must().HaveMonth(6));
    }

    [Fact]
    public void ToHaveDay_WhenDayMatches_ShouldSucceed()
    {
        (new DateOnly(2026, 5, 7)).Must().HaveDay(7);
    }

    [Fact]
    public void ToHaveDay_WhenDayDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new DateOnly(2026, 5, 7)).Must().HaveDay(8));
    }
}
