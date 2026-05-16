using OmniAssert;
namespace OmniAssert.Tests;

public class TimeSpanAssertionTests
{
    [Fact]
    public void ToBePositive_WhenPositive_ShouldSucceed()
    {
        (TimeSpan.FromMilliseconds(1)).Verify().ToBePositive();
    }

    [Fact]
    public void ToBePositive_WhenZero_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (TimeSpan.Zero).Verify().ToBePositive());
    }

    [Fact]
    public void ToBePositive_WhenNegative_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (TimeSpan.FromMilliseconds(-1)).Verify().ToBePositive());
    }

    [Fact]
    public void ToBeNegative_WhenNegative_ShouldSucceed()
    {
        (TimeSpan.FromMilliseconds(-1)).Verify().ToBeNegative();
    }

    [Fact]
    public void ToBeNegative_WhenZero_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (TimeSpan.Zero).Verify().ToBeNegative());
    }

    [Fact]
    public void ToBeNegative_WhenPositive_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (TimeSpan.FromMilliseconds(1)).Verify().ToBeNegative());
    }

    [Fact]
    public void ToBeGreaterThan_WhenGreater_ShouldSucceed()
    {
        (TimeSpan.FromSeconds(2)).Verify().ToBeGreaterThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ToBeGreaterThan_WhenEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (TimeSpan.FromSeconds(1)).Verify().ToBeGreaterThan(TimeSpan.FromSeconds(1)));
    }

    [Fact]
    public void ToBeGreaterThan_WhenLess_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (TimeSpan.FromSeconds(1)).Verify().ToBeGreaterThan(TimeSpan.FromSeconds(2)));
    }

    [Fact]
    public void ToBeLessThan_WhenLess_ShouldSucceed()
    {
        (TimeSpan.FromSeconds(1)).Verify().ToBeLessThan(TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void ToBeLessThan_WhenEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (TimeSpan.FromSeconds(1)).Verify().ToBeLessThan(TimeSpan.FromSeconds(1)));
    }

    [Fact]
    public void ToBeLessThan_WhenGreater_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (TimeSpan.FromSeconds(2)).Verify().ToBeLessThan(TimeSpan.FromSeconds(1)));
    }

    [Fact]
    public void ToBeOneOf_WhenTimeSpanIsInSet_ShouldSucceed()
    {
        (TimeSpan.FromSeconds(2)).Verify().ToBeOneOf(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void ToBeOneOf_WhenTimeSpanIsNotInSet_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (TimeSpan.FromSeconds(3)).Verify().ToBeOneOf(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
    }

    [Fact]
    public void ToBePositive_WithinScope_WhenNegative_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (TimeSpan.FromSeconds(-1)).Verify().ToBePositive();
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void ToBeNegative_WithinScope_MultipleFailures_ShouldThrowAggregate()
    {
        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using var scope = new AssertionScope();
            (TimeSpan.FromSeconds(1)).Verify().ToBeNegative();
            (TimeSpan.Zero).Verify().ToBeNegative();
        });
        Xunit.Assert.Equal(2, ex.InnerExceptions.Count);
    }
}
