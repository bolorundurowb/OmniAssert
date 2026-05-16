using OmniAssert;
namespace OmniAssert.Tests;

public class GuidAssertionTests
{
    [Fact]
    public void ToBeEmpty_WhenGuidIsEmpty_ShouldSucceed()
    {
        (Guid.Empty).Verify().ToBeEmpty();
    }

    [Fact]
    public void ToBeEmpty_WhenGuidIsNotEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (Guid.NewGuid()).Verify().ToBeEmpty());
    }

    [Fact]
    public void NotToBeEmpty_WhenGuidIsNotEmpty_ShouldSucceed()
    {
        (Guid.NewGuid()).Verify().NotToBeEmpty();
    }

    [Fact]
    public void NotToBeEmpty_WhenGuidIsEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (Guid.Empty).Verify().NotToBeEmpty());
    }

    [Fact]
    public void ToBeOneOf_WhenGuidIsInSet_ShouldSucceed()
    {
        var expected = Guid.NewGuid();
        (expected).Verify().ToBeOneOf(Guid.NewGuid(), expected);
    }

    [Fact]
    public void ToBeOneOf_WhenGuidIsNotInSet_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (Guid.NewGuid()).Verify().ToBeOneOf(Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public void ToBeEmpty_WithinScope_WhenGuidIsNotEmpty_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (Guid.NewGuid()).Verify().ToBeEmpty();
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void NotToBeEmpty_WithinScope_MultipleFailures_ShouldThrowAggregate()
    {
        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using var scope = new AssertionScope();
            (Guid.Empty).Verify().NotToBeEmpty();
            (Guid.Empty).Verify().NotToBeEmpty();
        });
        Xunit.Assert.Equal(2, ex.InnerExceptions.Count);
    }
}
