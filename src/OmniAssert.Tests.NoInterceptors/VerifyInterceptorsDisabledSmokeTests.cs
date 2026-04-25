using static OmniAssert.Assert;

namespace OmniAssert.Tests.NoInterceptors;

public class VerifyInterceptorsDisabledSmokeTests
{
    [Fact]
    public void Verify_BooleanLiteral_ShouldSucceed()
    {
        Verify(true);
    }

    [Fact]
    public void Verify_BooleanExpression_ShouldUseCallerArgumentExpressionOnly()
    {
        var x = 10;
        var y = 5;
        Verify(x > 5 && y < 10);
    }
}
