using System.Numerics;
using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class NumericAssertionTests
{
    // ── ToBe ────────────────────────────────────────────────────────────────

    [Fact]
    public void ToBe_WhenIntValuesEqual_ShouldSucceed()
    {
        Verify(42).ToBe(42);
    }

    [Fact]
    public void ToBe_WhenIntValuesDiffer_ShouldThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => Verify(1).ToBe(2));
        Xunit.Assert.Contains("Expected", ex.Message, StringComparison.Ordinal);
        Xunit.Assert.Contains("Got", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ToBe_WhenLongValuesEqual_ShouldSucceed()
    {
        Verify(100L).ToBe(100L);
    }

    [Fact]
    public void ToBe_WhenDoubleValuesEqual_ShouldSucceed()
    {
        Verify(3.14).ToBe(3.14);
    }

    [Fact]
    public void ToBe_WhenFloatValuesEqual_ShouldSucceed()
    {
        Verify(1.5f).ToBe(1.5f);
    }

    [Fact]
    public void ToBe_WhenDecimalValuesEqual_ShouldSucceed()
    {
        Verify(9.99m).ToBe(9.99m);
    }

    [Fact]
    public void ToBe_WhenByteValuesEqual_ShouldSucceed()
    {
        Verify((byte)9).ToBe((byte)9);
    }

    [Fact]
    public void ToBe_WhenShortValuesEqual_ShouldSucceed()
    {
        Verify((short)7).ToBe((short)7);
    }

    [Fact]
    public void ToBe_WhenUIntValuesEqual_ShouldSucceed()
    {
        Verify(42u).ToBe(42u);
    }

    [Fact]
    public void ToBe_WhenULongValuesEqual_ShouldSucceed()
    {
        Verify(42ul).ToBe(42ul);
    }

    [Fact]
    public void ToBe_WhenUShortValuesEqual_ShouldSucceed()
    {
        Verify((ushort)5).ToBe((ushort)5);
    }

    [Fact]
    public void ToBe_WhenSByteValuesEqual_ShouldSucceed()
    {
        Verify((sbyte)-3).ToBe((sbyte)-3);
    }

    [Fact]
    public void ToBe_WhenBigIntegerValuesEqual_ShouldSucceed()
    {
        Verify(new BigInteger(999)).ToBe(new BigInteger(999));
    }

    // ── NotToBe ─────────────────────────────────────────────────────────────

    [Fact]
    public void NotToBe_WhenIntValuesDiffer_ShouldSucceed()
    {
        Verify(1).NotToBe(2);
    }

    [Fact]
    public void NotToBe_WhenIntValuesEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(7).NotToBe(7));
    }

    // ── ToBeGreaterThan ──────────────────────────────────────────────────────

    [Fact]
    public void ToBeGreaterThan_WhenGreater_ShouldSucceed()
    {
        Verify(10).ToBeGreaterThan(9);
    }

    [Fact]
    public void ToBeGreaterThan_WhenNotGreater_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(1).ToBeGreaterThan(2));
    }

    [Fact]
    public void ToBeGreaterThan_WhenEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(5).ToBeGreaterThan(5));
    }

    // ── ToBeGreaterThanOrEqualTo ─────────────────────────────────────────────

    [Fact]
    public void ToBeGreaterThanOrEqualTo_WhenGreater_ShouldSucceed()
    {
        Verify(5).ToBeGreaterThanOrEqualTo(4);
    }

    [Fact]
    public void ToBeGreaterThanOrEqualTo_WhenEqual_ShouldSucceed()
    {
        Verify(3).ToBeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public void ToBeGreaterThanOrEqualTo_WhenLess_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(2).ToBeGreaterThanOrEqualTo(3));
    }

    // ── ToBeLessThan ─────────────────────────────────────────────────────────

    [Fact]
    public void ToBeLessThan_WhenLess_ShouldSucceed()
    {
        Verify(2).ToBeLessThan(3);
    }

    [Fact]
    public void ToBeLessThan_WhenNotLess_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(5).ToBeLessThan(1));
    }

    [Fact]
    public void ToBeLessThan_WhenEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(4).ToBeLessThan(4));
    }

    // ── ToBeLessThanOrEqualTo ────────────────────────────────────────────────

    [Fact]
    public void ToBeLessThanOrEqualTo_WhenLess_ShouldSucceed()
    {
        Verify(2).ToBeLessThanOrEqualTo(3);
    }

    [Fact]
    public void ToBeLessThanOrEqualTo_WhenEqual_ShouldSucceed()
    {
        Verify(4).ToBeLessThanOrEqualTo(4);
    }

    [Fact]
    public void ToBeLessThanOrEqualTo_WhenGreater_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(6).ToBeLessThanOrEqualTo(5));
    }

    // ── ToBeInRange ──────────────────────────────────────────────────────────

    [Fact]
    public void ToBeInRange_WhenInRange_ShouldSucceed()
    {
        Verify(5).ToBeInRange(1, 10);
    }

    [Fact]
    public void ToBeInRange_WhenAtLowerBound_ShouldSucceed()
    {
        Verify(1).ToBeInRange(1, 10);
    }

    [Fact]
    public void ToBeInRange_WhenAtUpperBound_ShouldSucceed()
    {
        Verify(10).ToBeInRange(1, 10);
    }

    [Fact]
    public void ToBeInRange_WhenAboveUpperBound_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(100).ToBeInRange(0, 10));
    }

    [Fact]
    public void ToBeInRange_WhenBelowLowerBound_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(0).ToBeInRange(1, 10));
    }

    // ── ToBeApproximately ────────────────────────────────────────────────────

    [Fact]
    public void ToBeApproximately_WhenWithinTolerance_ShouldSucceed()
    {
        Verify(1.0).ToBeApproximately(1.001, 0.01);
    }

    [Fact]
    public void ToBeApproximately_WhenExact_ShouldSucceed()
    {
        Verify(5.0).ToBeApproximately(5.0, 0.0001);
    }

    [Fact]
    public void ToBeApproximately_WhenOutsideTolerance_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(1.0).ToBeApproximately(2.0, 0.5));
    }

    [Fact]
    public void ToBeInRange_WhenDoubleInBounds_ShouldSucceed()
    {
        Verify(5.0).ToBeInRange(1.0, 10.0);
    }

    // ── Scope ────────────────────────────────────────────────────────────────

    [Fact]
    public void ToBe_WithinScope_WhenValuesDiffer_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            Verify(1).ToBe(2);
        });
        Xunit.Assert.Contains("Got", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ToBe_WithinScope_MultipleFailures_ShouldThrowAggregate()
    {
        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using var scope = new AssertionScope();
            Verify(1).ToBe(2);
            Verify(3).ToBe(4);
        });
        Xunit.Assert.Equal(2, ex.InnerExceptions.Count);
    }
}
