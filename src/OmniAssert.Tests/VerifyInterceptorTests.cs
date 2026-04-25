using static OmniAssert.Assert;

namespace OmniAssert.Tests;

/// <summary>Covers <see cref="Assert.VerifyExpression"/> with interceptors enabled and related <see cref="Assert.VerifyBoolean"/> behavior.</summary>
public class VerifyInterceptorTests
{
    [Fact]
    public void CompoundFailure_ShouldIncludeSourceExpression_AndNoOperandDictionary()
    {
        var x = 3;
        var y = 12;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => VerifyExpression(x > 5 && y < 10));
        Xunit.Assert.Contains("x > 5 && y < 10", ex.SourceExpression, StringComparison.Ordinal);
        Xunit.Assert.Null(ex.CapturedValues);
    }

    [Fact]
    public void CompoundSuccess_ShouldNotThrow()
    {
        var x = 10;
        var y = 5;
        VerifyExpression(x > 5 && y < 10);
    }

    [Fact]
    public void SimpleIdentifier_False_ShouldUseVerifyFluentPath()
    {
        var flag = false;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => VerifyExpression(flag));
        Xunit.Assert.Contains("flag", ex.SourceExpression, StringComparison.Ordinal);
    }

    [Fact]
    public void ParenthesizedIdentifier_ShouldUseVerifyFluentPath()
    {
        var flag = false;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => VerifyExpression((flag)));
        Xunit.Assert.Contains("flag", ex.SourceExpression, StringComparison.Ordinal);
    }

    [Fact]
    public void UnaryNot_OnIdentifier_ShouldFailWithExpressionText()
    {
        var flag = true;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => VerifyExpression(!flag));
        Xunit.Assert.Contains("!flag", ex.SourceExpression, StringComparison.Ordinal);
        Xunit.Assert.Null(ex.CapturedValues);
    }

    [Fact]
    public void VerifyBoolean_Direct_ShouldNotBeIntercepted()
    {
        var capture = new AssertionCapture("manual", new Dictionary<string, object?> { ["a"] = 1 });
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => VerifyBoolean(false, in capture));
        Xunit.Assert.Equal("manual", ex.SourceExpression);
        Xunit.Assert.NotNull(ex.CapturedValues);
        Xunit.Assert.Equal(1, ex.CapturedValues!["a"]);
    }

    [Fact]
    public void LiteralTrue_ShouldSucceed()
    {
        Verify(true).ToBeTrue();
    }

    [Fact]
    public void LiteralFalse_ShouldThrowWithExpression()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => VerifyExpression(false));
        Xunit.Assert.Contains("false", ex.SourceExpression, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EqualityLiteral_ShouldThrowWithExpression()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => VerifyExpression(1 == 2));
        Xunit.Assert.Contains("1 == 2", ex.SourceExpression, StringComparison.Ordinal);
        Xunit.Assert.Null(ex.CapturedValues);
    }

    [Fact]
    public void AssertionScope_BooleanVerifyFailure_ShouldThrowOnDispose()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using (new AssertionScope())
            {
                VerifyExpression(1 > 2);
            }
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void Verify_BooleanFalse_ToBeTrue_ShouldUseBoolAssertionsMessage()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => Verify(false).ToBeTrue());
        Xunit.Assert.Contains("expected expression to be true", ex.Message, StringComparison.OrdinalIgnoreCase);
        Xunit.Assert.Contains("false", ex.SourceExpression, StringComparison.OrdinalIgnoreCase);
    }
}
