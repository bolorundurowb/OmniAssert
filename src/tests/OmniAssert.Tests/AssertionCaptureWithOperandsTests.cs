namespace OmniAssert.Tests;

#pragma warning disable CS0618
#pragma warning disable OA004

public class AssertionCaptureWithOperandsTests
{
    [Fact]
    public void VerifyExpression_WithAssertionCapture_ShouldIncludeCapturedValuesInMessage()
    {
        var x = 2;
        var y = 3;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            (x > y).VerifyExpression(AssertionCapture.WithOperands("x > y", ("x", x), ("y", y))));

        Xunit.Assert.Contains("x > y", ex.Message, StringComparison.Ordinal);
        Xunit.Assert.Contains("Captured values:", ex.Message, StringComparison.Ordinal);
        Xunit.Assert.Contains("x: 2", ex.Message, StringComparison.Ordinal);
        Xunit.Assert.Contains("y: 3", ex.Message, StringComparison.Ordinal);
        Xunit.Assert.NotNull(ex.CapturedValues);
        Xunit.Assert.Equal(2, ex.CapturedValues!["x"]);
        Xunit.Assert.Equal(3, ex.CapturedValues["y"]);
    }
}
