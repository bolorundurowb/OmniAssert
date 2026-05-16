using OmniAssert;
namespace OmniAssert.Tests;

public class BoolAssertionTests
{

    [Fact]
    public void ToBeTrue_WhenTrue_ShouldNotThrow()
    {
        (true).Verify().ToBeTrue();
    }

    [Fact]
    public void ToBeTrue_WhenFalse_ShouldThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => (false).Verify().ToBeTrue());
        Xunit.Assert.Contains("expected expression to be true", ex.Message, StringComparison.OrdinalIgnoreCase);
        Xunit.Assert.Contains("false", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ToBeTrue_WhenFalse_MessageContainsSubjectExpression()
    {
        bool myFlag = false;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => (myFlag).Verify().ToBeTrue());
        Xunit.Assert.Contains("myFlag", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ToBeTrue_WithCapturedValues_WhenFalse_ShouldIncludeContextInMessage()
    {
        var capturedValues = new Dictionary<string, object?> { ["x"] = 1, ["y"] = 2 };
        var assertions = new BoolAssertions(false, "x > y", capturedValues);

        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => assertions.ToBeTrue());
        Xunit.Assert.Contains("Context:", ex.Message, StringComparison.Ordinal);
        Xunit.Assert.Contains("x = 1", ex.Message, StringComparison.Ordinal);
        Xunit.Assert.Contains("y = 2", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ToBeTrue_WithNullCapturedValues_WhenFalse_ShouldNotIncludeContext()
    {
        var assertions = new BoolAssertions(false, "expr", null);

        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => assertions.ToBeTrue());
        Xunit.Assert.DoesNotContain("Context:", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ToBeTrue_WithEmptyCapturedValues_WhenFalse_ShouldNotIncludeContext()
    {
        var assertions = new BoolAssertions(false, "expr", new Dictionary<string, object?>());

        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => assertions.ToBeTrue());
        Xunit.Assert.DoesNotContain("Context:", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ToBeTrue_WithinScope_WhenFalse_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (false).Verify().ToBeTrue();
        });
        Xunit.Assert.Contains("expected expression to be true", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToBeFalse_WhenFalse_ShouldNotThrow()
    {
        (false).Verify().ToBeFalse();
    }

    [Fact]
    public void ToBeFalse_WhenTrue_ShouldThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => (true).Verify().ToBeFalse());
        Xunit.Assert.Contains("expected expression to be false", ex.Message, StringComparison.OrdinalIgnoreCase);
        Xunit.Assert.Contains("true", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ToBeFalse_WhenTrue_MessageContainsSubjectExpression()
    {
        bool isReady = true;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => (isReady).Verify().ToBeFalse());
        Xunit.Assert.Contains("isReady", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ToBeFalse_WithCapturedValues_WhenTrue_ShouldIncludeContextInMessage()
    {
        var capturedValues = new Dictionary<string, object?> { ["a"] = "hello", ["b"] = null };
        var assertions = new BoolAssertions(true, "a != null", capturedValues);

        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => assertions.ToBeFalse());
        Xunit.Assert.Contains("Context:", ex.Message, StringComparison.Ordinal);
        Xunit.Assert.Contains("a =", ex.Message, StringComparison.Ordinal);
        Xunit.Assert.Contains("b =", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ToBeFalse_WithinScope_WhenTrue_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (true).Verify().ToBeFalse();
        });
        Xunit.Assert.Contains("expected expression to be false", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToBeTrue_WithinScope_MultipleFailures_ShouldThrowAggregate()
    {
        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using var scope = new AssertionScope();
            (false).Verify().ToBeTrue();
            (false).Verify().ToBeTrue();
        });
        Xunit.Assert.Equal(2, ex.InnerExceptions.Count);
    }
}
