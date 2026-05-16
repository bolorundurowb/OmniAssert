using OmniAssert;
namespace OmniAssert.Tests;

public class TimeOnlyAssertionTests
{
    [Fact]
    public void ToBe_WhenTimesEqual_ShouldSucceed()
    {
        (new TimeOnly(10, 30, 0)).Verify().ToBe(new TimeOnly(10, 30, 0));
    }

    [Fact]
    public void ToBe_WhenTimesDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new TimeOnly(10, 30, 0)).Verify().ToBe(new TimeOnly(10, 31, 0)));
    }

    [Fact]
    public void ToBeBefore_WhenEarlier_ShouldSucceed()
    {
        (new TimeOnly(10, 30, 0)).Verify().ToBeBefore(new TimeOnly(10, 31, 0));
    }

    [Fact]
    public void ToBeBefore_WhenEqualOrLater_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new TimeOnly(10, 30, 0)).Verify().ToBeBefore(new TimeOnly(10, 30, 0)));
    }

    [Fact]
    public void ToBeAfter_WhenLater_ShouldSucceed()
    {
        (new TimeOnly(10, 31, 0)).Verify().ToBeAfter(new TimeOnly(10, 30, 0));
    }

    [Fact]
    public void ToBeAfter_WhenEqualOrEarlier_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new TimeOnly(10, 30, 0)).Verify().ToBeAfter(new TimeOnly(10, 30, 0)));
    }
}
