using static OmniAssert.Assert;

namespace OmniAssert.Tests.NoInterceptors;

/// <summary>Regression tests when <c>OmniAssertEnableVerifyInterceptors</c> is false (no generated interceptors).</summary>
public class VerifyInterceptorsDisabledSmokeTests
{
    [Fact]
    public void Verify_BooleanLiteral_ShouldSucceed()
    {
        Verify(true).ToBeTrue();
    }

    [Fact]
    public void VerifyExpression_BooleanExpression_ShouldUseCallerArgumentExpressionOnly()
    {
        var x = 10;
        var y = 5;
        VerifyExpression(x > 5 && y < 10);
    }
}
