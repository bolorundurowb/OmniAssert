namespace OmniAssert.Tests;

public class GuidAssertionTests
{
    [Fact]
    public void ToBeEmpty_WhenGuidIsEmpty_ShouldSucceed()
    {
        (Guid.Empty).Must().BeEmpty();
    }

    [Fact]
    public void ToBeEmpty_WhenGuidIsNotEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (Guid.NewGuid()).Must().BeEmpty());
    }

    [Fact]
    public void NotToBeEmpty_WhenGuidIsNotEmpty_ShouldSucceed()
    {
        (Guid.NewGuid()).Must().NotBeEmpty();
    }

    [Fact]
    public void NotToBeEmpty_WhenGuidIsEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (Guid.Empty).Must().NotBeEmpty());
    }

    [Fact]
    public void ToBeOneOf_WhenGuidIsInSet_ShouldSucceed()
    {
        var expected = Guid.NewGuid();
        (expected).Must().BeOneOf(Guid.NewGuid(), expected);
    }

    [Fact]
    public void ToBeOneOf_WhenGuidIsNotInSet_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (Guid.NewGuid()).Must().BeOneOf(Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public void ToBeEmpty_WithinScope_WhenGuidIsNotEmpty_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (Guid.NewGuid()).Must().BeEmpty();
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void NotToBeEmpty_WithinScope_MultipleFailures_ShouldThrowAggregate()
    {
        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using var scope = new AssertionScope();
            (Guid.Empty).Must().NotBeEmpty();
            (Guid.Empty).Must().NotBeEmpty();
        });
        Xunit.Assert.Equal(2, ex.InnerExceptions.Count);
    }
}
