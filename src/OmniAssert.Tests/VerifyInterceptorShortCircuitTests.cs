using static OmniAssert.Assert;

namespace OmniAssert.Tests;

/// <summary>Boolean short-circuit evaluation with fluent <see cref="Assert.Verify(bool)"/> and expression failures via <see cref="Assert.VerifyExpression"/>.</summary>
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

        // `left && Right()` is false with short-circuit; negate so Verify(...).ToBeTrue() sees true while RHS stays unevaluated.
        Verify(!(left && Right())).ToBeTrue();
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

        Verify(left && Right()).ToBeTrue();
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

        Verify(left || Right()).ToBeTrue();
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

        Verify(left || Right()).ToBeTrue();
        Xunit.Assert.True(rightInvoked);
    }

    [Fact]
    public void Verify_OrFailure_ShouldCaptureExpressionOnly()
    {
        var left = false;
        var right = false;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => VerifyExpression(left || right));
        Xunit.Assert.Contains("left || right", ex.SourceExpression, StringComparison.Ordinal);
        Xunit.Assert.Null(ex.CapturedValues);
    }
}
