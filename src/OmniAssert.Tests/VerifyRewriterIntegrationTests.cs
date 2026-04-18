using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class VerifyRewriterIntegrationTests
{
    [Fact]
    public void VerifyBoolean_WhenFalse_ShouldIncludeCapturedValuesInException()
    {
        var capture = new AssertionCapture(
            "x > 5 && y < 10",
            new Dictionary<string, object?> { ["x"] = 3, ["y"] = 12 });

        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => VerifyBoolean(false, in capture));
        Xunit.Assert.Contains("x > 5 && y < 10", ex.SourceExpression, StringComparison.Ordinal);
        Xunit.Assert.NotNull(ex.CapturedValues);
        Xunit.Assert.Equal(3, ex.CapturedValues!["x"]);
        Xunit.Assert.Equal(12, ex.CapturedValues["y"]);
    }

    [Fact]
    public void Verify_BooleanExpression_WhenTrue_ShouldSucceed()
    {
        var x = 10;
        var y = 5;
        Verify(x > 5 && y < 10);
    }

}
