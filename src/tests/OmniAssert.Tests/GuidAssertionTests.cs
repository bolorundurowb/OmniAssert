using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class GuidAssertionTests
{
    [Fact]
    public void ToBeEmpty_WhenGuidIsEmpty_ShouldSucceed()
    {
        Verify(Guid.Empty).ToBeEmpty();
    }

    [Fact]
    public void ToBeEmpty_WhenGuidIsNotEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(Guid.NewGuid()).ToBeEmpty());
    }

    [Fact]
    public void NotToBeEmpty_WhenGuidIsNotEmpty_ShouldSucceed()
    {
        Verify(Guid.NewGuid()).NotToBeEmpty();
    }

    [Fact]
    public void NotToBeEmpty_WhenGuidIsEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(Guid.Empty).NotToBeEmpty());
    }

    [Fact]
    public void ToBeOneOf_WhenGuidIsInSet_ShouldSucceed()
    {
        var expected = Guid.NewGuid();
        Verify(expected).ToBeOneOf(Guid.NewGuid(), expected);
    }

    [Fact]
    public void ToBeOneOf_WhenGuidIsNotInSet_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(Guid.NewGuid()).ToBeOneOf(Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public void ToBeEmpty_WithinScope_WhenGuidIsNotEmpty_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            Verify(Guid.NewGuid()).ToBeEmpty();
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void NotToBeEmpty_WithinScope_MultipleFailures_ShouldThrowAggregate()
    {
        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using var scope = new AssertionScope();
            Verify(Guid.Empty).NotToBeEmpty();
            Verify(Guid.Empty).NotToBeEmpty();
        });
        Xunit.Assert.Equal(2, ex.InnerExceptions.Count);
    }
}
