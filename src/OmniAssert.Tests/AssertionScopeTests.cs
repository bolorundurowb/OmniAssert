using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class AssertionScopeTests
{
    [Fact]
    public void Dispose_WhenNoFailures_ShouldNotThrow()
    {
        using (new AssertionScope())
        {
            Verify(1).ToBe(1);
        }
    }

    [Fact]
    public void Dispose_WhenSingleFailure_ShouldThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using (new AssertionScope())
            {
                Verify(1).ToBe(2);
            }
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void Dispose_WhenMultipleFailures_ShouldThrowAggregateException()
    {
        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using (new AssertionScope())
            {
                Verify(1).ToBe(2);
                Verify(3).ToBe(4);
            }
        });
        Xunit.Assert.Equal(2, ex!.InnerExceptions.Count);
    }
}
