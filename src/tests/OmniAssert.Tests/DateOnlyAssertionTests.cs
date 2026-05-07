using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class DateOnlyAssertionTests
{
    [Fact]
    public void ToBe_WhenDatesEqual_ShouldSucceed()
    {
        Verify(new DateOnly(2026, 5, 7)).ToBe(new DateOnly(2026, 5, 7));
    }

    [Fact]
    public void ToBe_WhenDatesDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new DateOnly(2026, 5, 7)).ToBe(new DateOnly(2026, 5, 8)));
    }

    [Fact]
    public void ToBeBefore_WhenEarlier_ShouldSucceed()
    {
        Verify(new DateOnly(2026, 5, 7)).ToBeBefore(new DateOnly(2026, 5, 8));
    }

    [Fact]
    public void ToBeBefore_WhenEqualOrLater_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new DateOnly(2026, 5, 7)).ToBeBefore(new DateOnly(2026, 5, 7)));
    }

    [Fact]
    public void ToBeAfter_WhenLater_ShouldSucceed()
    {
        Verify(new DateOnly(2026, 5, 8)).ToBeAfter(new DateOnly(2026, 5, 7));
    }

    [Fact]
    public void ToBeAfter_WhenEqualOrEarlier_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new DateOnly(2026, 5, 7)).ToBeAfter(new DateOnly(2026, 5, 7)));
    }

    [Fact]
    public void HasYear_WhenYearMatches_ShouldSucceed()
    {
        Verify(new DateOnly(2026, 5, 7)).HasYear(2026);
    }

    [Fact]
    public void HasYear_WhenYearDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new DateOnly(2026, 5, 7)).HasYear(2025));
    }

    [Fact]
    public void HasMonth_WhenMonthMatches_ShouldSucceed()
    {
        Verify(new DateOnly(2026, 5, 7)).HasMonth(5);
    }

    [Fact]
    public void HasMonth_WhenMonthDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new DateOnly(2026, 5, 7)).HasMonth(6));
    }

    [Fact]
    public void HasDay_WhenDayMatches_ShouldSucceed()
    {
        Verify(new DateOnly(2026, 5, 7)).HasDay(7);
    }

    [Fact]
    public void HasDay_WhenDayDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new DateOnly(2026, 5, 7)).HasDay(8));
    }
}
