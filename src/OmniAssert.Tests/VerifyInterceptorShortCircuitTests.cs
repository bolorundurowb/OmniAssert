using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class VerifyInterceptorShortCircuitTests
{
    [Fact]
    public void Verify_LogicalAnd_WhenLeftFalse_ShouldNotEvaluateRight()
    {
        var left = false;
        var rightInvoked = false;
        bool Right()
        {
            rightInvoked = true;
            return true;
        }

        // `left && Right()` is false with short-circuit; negate so Verify receives true while RHS stays unevaluated.
        Verify(!(left && Right()));
        Xunit.Assert.False(rightInvoked);
    }

    [Fact]
    public void Verify_LogicalAnd_WhenTrue_ShouldEvaluateBoth()
    {
        var left = true;
        var rightInvoked = false;
        bool Right()
        {
            rightInvoked = true;
            return true;
        }

        Verify(left && Right());
        Xunit.Assert.True(rightInvoked);
    }

    [Fact]
    public void Verify_LogicalOr_WhenLeftTrue_ShouldNotEvaluateRight()
    {
        var left = true;
        var rightInvoked = false;
        bool Right()
        {
            rightInvoked = true;
            return false;
        }

        Verify(left || Right());
        Xunit.Assert.False(rightInvoked);
    }

    [Fact]
    public void Verify_LogicalOr_WhenLeftFalse_ShouldEvaluateRight()
    {
        var left = false;
        var rightInvoked = false;
        bool Right()
        {
            rightInvoked = true;
            return true;
        }

        Verify(left || Right());
        Xunit.Assert.True(rightInvoked);
    }

    [Fact]
    public void Verify_OrFailure_ShouldCaptureExpressionOnly()
    {
        var left = false;
        var right = false;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => Verify(left || right));
        Xunit.Assert.Contains("left || right", ex.SourceExpression, StringComparison.Ordinal);
        Xunit.Assert.Null(ex.CapturedValues);
    }
}
