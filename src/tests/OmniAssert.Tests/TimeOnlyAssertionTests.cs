using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class TimeOnlyAssertionTests
{
    [Fact]
    public void ToBe_WhenTimesEqual_ShouldSucceed()
    {
        Verify(new TimeOnly(10, 30, 0)).ToBe(new TimeOnly(10, 30, 0));
    }

    [Fact]
    public void ToBe_WhenTimesDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new TimeOnly(10, 30, 0)).ToBe(new TimeOnly(10, 31, 0)));
    }

    [Fact]
    public void ToBeBefore_WhenEarlier_ShouldSucceed()
    {
        Verify(new TimeOnly(10, 30, 0)).ToBeBefore(new TimeOnly(10, 31, 0));
    }

    [Fact]
    public void ToBeBefore_WhenEqualOrLater_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new TimeOnly(10, 30, 0)).ToBeBefore(new TimeOnly(10, 30, 0)));
    }

    [Fact]
    public void ToBeAfter_WhenLater_ShouldSucceed()
    {
        Verify(new TimeOnly(10, 31, 0)).ToBeAfter(new TimeOnly(10, 30, 0));
    }

    [Fact]
    public void ToBeAfter_WhenEqualOrEarlier_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new TimeOnly(10, 30, 0)).ToBeAfter(new TimeOnly(10, 30, 0)));
    }
}
