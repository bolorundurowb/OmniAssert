using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class AssertionScopeTests
{
    // ── Basic scope behavior ─────────────────────────────────────────────────

    [Fact]
    public void Dispose_WhenNoFailures_ShouldNotThrow()
    {
        using (new AssertionScope())
        {
            Verify(1).ToBe(1);
        }
    }

    [Fact]
    public void Dispose_WhenSingleFailure_ShouldThrowOmniAssertionException()
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

    [Fact]
    public void Dispose_WhenMultipleFailures_AggregateInnerExceptionsShouldBeOmniAssertionException()
    {
        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using (new AssertionScope())
            {
                Verify(1).ToBe(2);
                Verify(3).ToBe(4);
            }
        });
        Xunit.Assert.All(ex.InnerExceptions, e => Xunit.Assert.IsType<OmniAssertionException>(e));
    }

    // ── VerifyExpression with scope ──────────────────────────────────────────

    [Fact]
    public void VerifyExpression_WithinScope_WhenFalse_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            VerifyExpression(false);
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void VerifyExpression_WithinScope_WhenTrue_ShouldNotContributeFailure()
    {
        using (new AssertionScope())
        {
            VerifyExpression(true);
        }
    }

    // ── Scope isolation ──────────────────────────────────────────────────────

    [Fact]
    public void Scope_WhenDisposed_ShouldNotAffectSubsequentAssertions()
    {
        Xunit.Assert.Throws<AggregateException>(() =>
        {
            using (new AssertionScope())
            {
                Verify(1).ToBe(2);
                Verify(3).ToBe(4);
            }
        });

        // After scope is disposed, assertions should throw immediately again
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(5).ToBe(6));
    }
}
