namespace OmniAssert.Tests;

public class DateOnlyAssertionTests
{
    [Fact]
    public void ToBe_WhenDatesEqual_ShouldSucceed()
    {
        (new DateOnly(2026, 5, 7)).Verify().ToBe(new DateOnly(2026, 5, 7));
    }

    [Fact]
    public void ToBe_WhenDatesDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new DateOnly(2026, 5, 7)).Verify().ToBe(new DateOnly(2026, 5, 8)));
    }

    [Fact]
    public void ToBeBefore_WhenEarlier_ShouldSucceed()
    {
        (new DateOnly(2026, 5, 7)).Verify().ToBeBefore(new DateOnly(2026, 5, 8));
    }

    [Fact]
    public void ToBeBefore_WhenEqualOrLater_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new DateOnly(2026, 5, 7)).Verify().ToBeBefore(new DateOnly(2026, 5, 7)));
    }

    [Fact]
    public void ToBeAfter_WhenLater_ShouldSucceed()
    {
        (new DateOnly(2026, 5, 8)).Verify().ToBeAfter(new DateOnly(2026, 5, 7));
    }

    [Fact]
    public void ToBeAfter_WhenEqualOrEarlier_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new DateOnly(2026, 5, 7)).Verify().ToBeAfter(new DateOnly(2026, 5, 7)));
    }

    [Fact]
    public void ToHaveYear_WhenYearMatches_ShouldSucceed()
    {
        (new DateOnly(2026, 5, 7)).Verify().ToHaveYear(2026);
    }

    [Fact]
    public void ToHaveYear_WhenYearDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new DateOnly(2026, 5, 7)).Verify().ToHaveYear(2025));
    }

    [Fact]
    public void ToHaveMonth_WhenMonthMatches_ShouldSucceed()
    {
        (new DateOnly(2026, 5, 7)).Verify().ToHaveMonth(5);
    }

    [Fact]
    public void ToHaveMonth_WhenMonthDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new DateOnly(2026, 5, 7)).Verify().ToHaveMonth(6));
    }

    [Fact]
    public void ToHaveDay_WhenDayMatches_ShouldSucceed()
    {
        (new DateOnly(2026, 5, 7)).Verify().ToHaveDay(7);
    }

    [Fact]
    public void ToHaveDay_WhenDayDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new DateOnly(2026, 5, 7)).Verify().ToHaveDay(8));
    }
}
