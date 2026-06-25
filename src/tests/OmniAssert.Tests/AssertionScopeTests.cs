namespace OmniAssert.Tests;

#pragma warning disable OA004 // Legacy VerifyExpression() under test

public class AssertionScopeTests
{
    [Fact]
    public void Dispose_WhenNoFailures_ShouldNotThrow()
    {
        using (new AssertionScope())
        {
            (1).Must().Be(1);
        }
    }

    [Fact]
    public void Dispose_WhenSingleFailure_ShouldThrowOmniAssertionException()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using (new AssertionScope())
            {
                (1).Must().Be(2);
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
                (1).Must().Be(2);
                (3).Must().Be(4);
            }
        });
        Xunit.Assert.Equal(2, ex!.InnerExceptions.Count);
    }

    [Fact]
    public void Dispose_WhenMultipleFailures_AggregateInnerExceptionsShouldBeOmniAssertionException()
    {
        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using (new AssertionScope())
            {
                (1).Must().Be(2);
                (3).Must().Be(4);
            }
        });
        Xunit.Assert.All(ex.InnerExceptions, e => Xunit.Assert.IsType<OmniAssertionException>(e));
    }

    [Fact]
    public void VerifyExpression_WithinScope_WhenFalse_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (false).VerifyExpression();
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void VerifyExpression_WithinScope_WhenTrue_ShouldNotContributeFailure()
    {
        using (new AssertionScope())
        {
            (true).VerifyExpression();
        }
    }

    [Fact]
    public void Scope_WhenDisposed_ShouldNotAffectSubsequentAssertions()
    {
        Xunit.Assert.Throws<AggregateException>(() =>
        {
            using (new AssertionScope())
            {
                (1).Must().Be(2);
                (3).Must().Be(4);
            }
        });

        // After scope is disposed, assertions should throw immediately again
        Xunit.Assert.Throws<OmniAssertionException>(() => (5).Must().Be(6));
    }
}
