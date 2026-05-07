using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class TimeSpanAssertionTests
{
    [Fact]
    public void ToBePositive_WhenPositive_ShouldSucceed()
    {
        Verify(TimeSpan.FromMilliseconds(1)).ToBePositive();
    }

    [Fact]
    public void ToBePositive_WhenZero_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(TimeSpan.Zero).ToBePositive());
    }

    [Fact]
    public void ToBePositive_WhenNegative_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(TimeSpan.FromMilliseconds(-1)).ToBePositive());
    }

    [Fact]
    public void ToBeNegative_WhenNegative_ShouldSucceed()
    {
        Verify(TimeSpan.FromMilliseconds(-1)).ToBeNegative();
    }

    [Fact]
    public void ToBeNegative_WhenZero_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(TimeSpan.Zero).ToBeNegative());
    }

    [Fact]
    public void ToBeNegative_WhenPositive_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(TimeSpan.FromMilliseconds(1)).ToBeNegative());
    }

    [Fact]
    public void ToBeGreaterThan_WhenGreater_ShouldSucceed()
    {
        Verify(TimeSpan.FromSeconds(2)).ToBeGreaterThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ToBeGreaterThan_WhenEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(TimeSpan.FromSeconds(1)).ToBeGreaterThan(TimeSpan.FromSeconds(1)));
    }

    [Fact]
    public void ToBeGreaterThan_WhenLess_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(TimeSpan.FromSeconds(1)).ToBeGreaterThan(TimeSpan.FromSeconds(2)));
    }

    [Fact]
    public void ToBeLessThan_WhenLess_ShouldSucceed()
    {
        Verify(TimeSpan.FromSeconds(1)).ToBeLessThan(TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void ToBeLessThan_WhenEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(TimeSpan.FromSeconds(1)).ToBeLessThan(TimeSpan.FromSeconds(1)));
    }

    [Fact]
    public void ToBeLessThan_WhenGreater_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(TimeSpan.FromSeconds(2)).ToBeLessThan(TimeSpan.FromSeconds(1)));
    }

    [Fact]
    public void ToBeOneOf_WhenTimeSpanIsInSet_ShouldSucceed()
    {
        Verify(TimeSpan.FromSeconds(2)).ToBeOneOf(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void ToBeOneOf_WhenTimeSpanIsNotInSet_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(TimeSpan.FromSeconds(3)).ToBeOneOf(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
    }

    [Fact]
    public void ToBePositive_WithinScope_WhenNegative_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            Verify(TimeSpan.FromSeconds(-1)).ToBePositive();
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void ToBeNegative_WithinScope_MultipleFailures_ShouldThrowAggregate()
    {
        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using var scope = new AssertionScope();
            Verify(TimeSpan.FromSeconds(1)).ToBeNegative();
            Verify(TimeSpan.Zero).ToBeNegative();
        });
        Xunit.Assert.Equal(2, ex.InnerExceptions.Count);
    }
}
