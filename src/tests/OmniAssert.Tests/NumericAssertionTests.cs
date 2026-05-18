using System.Numerics;

namespace OmniAssert.Tests;

public class NumericAssertionTests
{
    [Fact]
    public void ToBe_WhenIntValuesEqual_ShouldSucceed()
    {
        (42).Verify().ToBe(42);
    }

    [Fact]
    public void ToBe_WhenIntValuesDiffer_ShouldThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => (1).Verify().ToBe(2));
        Xunit.Assert.Contains("Expected", ex.Message, StringComparison.Ordinal);
        Xunit.Assert.Contains("Got", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ToBe_WhenLongValuesEqual_ShouldSucceed()
    {
        (100L).Verify().ToBe(100L);
    }

    [Fact]
    public void ToBe_WhenDoubleValuesEqual_ShouldSucceed()
    {
        (3.14).Verify().ToBe(3.14);
    }

    [Fact]
    public void ToBe_WhenFloatValuesEqual_ShouldSucceed()
    {
        (1.5f).Verify().ToBe(1.5f);
    }

    [Fact]
    public void ToBe_WhenDecimalValuesEqual_ShouldSucceed()
    {
        (9.99m).Verify().ToBe(9.99m);
    }

    [Fact]
    public void ToBe_WhenByteValuesEqual_ShouldSucceed()
    {
        ((byte)9).Verify().ToBe((byte)9);
    }

    [Fact]
    public void ToBe_WhenShortValuesEqual_ShouldSucceed()
    {
        ((short)7).Verify().ToBe((short)7);
    }

    [Fact]
    public void ToBe_WhenUIntValuesEqual_ShouldSucceed()
    {
        (42u).Verify().ToBe(42u);
    }

    [Fact]
    public void ToBe_WhenULongValuesEqual_ShouldSucceed()
    {
        (42ul).Verify().ToBe(42ul);
    }

    [Fact]
    public void ToBe_WhenUShortValuesEqual_ShouldSucceed()
    {
        ((ushort)5).Verify().ToBe((ushort)5);
    }

    [Fact]
    public void ToBe_WhenSByteValuesEqual_ShouldSucceed()
    {
        ((sbyte)-3).Verify().ToBe((sbyte)-3);
    }

    [Fact]
    public void ToBe_WhenBigIntegerValuesEqual_ShouldSucceed()
    {
        (new BigInteger(999)).Verify().ToBe(new BigInteger(999));
    }

    [Fact]
    public void NotToBe_WhenIntValuesDiffer_ShouldSucceed()
    {
        (1).Verify().NotToBe(2);
    }

    [Fact]
    public void NotToBe_WhenIntValuesEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (7).Verify().NotToBe(7));
    }

    [Fact]
    public void ToBeOneOf_WhenValueIsInSet_ShouldSucceed()
    {
        (2).Verify().ToBeOneOf(1, 2, 3);
    }

    [Fact]
    public void ToBeOneOf_WhenValueIsNotInSet_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (4).Verify().ToBeOneOf(1, 2, 3));
    }

    [Fact]
    public void ToBeGreaterThan_WhenGreater_ShouldSucceed()
    {
        (10).Verify().ToBeGreaterThan(9);
    }

    [Fact]
    public void ToBeGreaterThan_WhenNotGreater_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (1).Verify().ToBeGreaterThan(2));
    }

    [Fact]
    public void ToBeGreaterThan_WhenEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (5).Verify().ToBeGreaterThan(5));
    }

    [Fact]
    public void ToBeGreaterThanOrEqualTo_WhenGreater_ShouldSucceed()
    {
        (5).Verify().ToBeGreaterThanOrEqualTo(4);
    }

    [Fact]
    public void ToBeGreaterThanOrEqualTo_WhenEqual_ShouldSucceed()
    {
        (3).Verify().ToBeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public void ToBeGreaterThanOrEqualTo_WhenLess_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (2).Verify().ToBeGreaterThanOrEqualTo(3));
    }

    [Fact]
    public void ToBeLessThan_WhenLess_ShouldSucceed()
    {
        (2).Verify().ToBeLessThan(3);
    }

    [Fact]
    public void ToBeLessThan_WhenNotLess_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (5).Verify().ToBeLessThan(1));
    }

    [Fact]
    public void ToBeLessThan_WhenEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (4).Verify().ToBeLessThan(4));
    }

    [Fact]
    public void ToBeLessThanOrEqualTo_WhenLess_ShouldSucceed()
    {
        (2).Verify().ToBeLessThanOrEqualTo(3);
    }

    [Fact]
    public void ToBeLessThanOrEqualTo_WhenEqual_ShouldSucceed()
    {
        (4).Verify().ToBeLessThanOrEqualTo(4);
    }

    [Fact]
    public void ToBeLessThanOrEqualTo_WhenGreater_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (6).Verify().ToBeLessThanOrEqualTo(5));
    }

    [Fact]
    public void ToBeInRange_WhenInRange_ShouldSucceed()
    {
        (5).Verify().ToBeInRange(1, 10);
    }

    [Fact]
    public void ToBeInRange_WhenAtLowerBound_ShouldSucceed()
    {
        (1).Verify().ToBeInRange(1, 10);
    }

    [Fact]
    public void ToBeInRange_WhenAtUpperBound_ShouldSucceed()
    {
        (10).Verify().ToBeInRange(1, 10);
    }

    [Fact]
    public void ToBeInRange_WhenAboveUpperBound_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (100).Verify().ToBeInRange(0, 10));
    }

    [Fact]
    public void ToBeInRange_WhenBelowLowerBound_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (0).Verify().ToBeInRange(1, 10));
    }

    [Fact]
    public void ToBeApproximately_WhenWithinTolerance_ShouldSucceed()
    {
        (1.0).Verify().ToBeApproximately(1.001, 0.01);
    }

    [Fact]
    public void ToBeApproximately_WhenExact_ShouldSucceed()
    {
        (5.0).Verify().ToBeApproximately(5.0, 0.0001);
    }

    [Fact]
    public void ToBeApproximately_WhenOutsideTolerance_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (1.0).Verify().ToBeApproximately(2.0, 0.5));
    }

    [Fact]
    public void ToBeApproximately_WhenPrecisionIsNegative_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (1.0).Verify().ToBeApproximately(1.0, -0.1));
    }

    [Fact]
    public void ToBeInRange_WhenDoubleInBounds_ShouldSucceed()
    {
        (5.0).Verify().ToBeInRange(1.0, 10.0);
    }

    [Fact]
    public void ToBeApproximately_WithFloatingPointNaN_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            double.NaN.Verify().ToBeApproximately(1.0, 0.1));
    }

    [Fact]
    public void ToBeApproximately_WhenBothNaN_ShouldFail()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            double.NaN.Verify().ToBeApproximately(double.NaN, 0.1));
    }

    [Fact]
    public void ToBeApproximately_WithFloatingPointPositiveInfinity_ShouldSucceed()
    {
        double.PositiveInfinity.Verify().ToBeApproximately(double.PositiveInfinity, 0.1);
    }

    [Fact]
    public void ToBeApproximately_WithFloatingPointNegativeInfinity_ShouldSucceed()
    {
        double.NegativeInfinity.Verify().ToBeApproximately(double.NegativeInfinity, 0.1);
    }

    [Fact]
    public void ToBeInRange_WithVeryLargeNumbers_ShouldSucceed()
    {
        long.MaxValue.Verify().ToBeInRange(long.MinValue, long.MaxValue);
    }

    [Fact]
    public void ToBeInRange_WithVerySmallNumbers_ShouldSucceed()
    {
        long.MinValue.Verify().ToBeInRange(long.MinValue, long.MaxValue);
    }

    [Fact]
    public void ToBeGreaterThan_WithZero_ShouldSucceed()
    {
        (1).Verify().ToBeGreaterThan(0);
    }

    [Fact]
    public void ToBeGreaterThan_WithNegative_ShouldSucceed()
    {
        (0).Verify().ToBeGreaterThan(-1);
    }

    [Fact]
    public void ToBeLessThan_WithZero_ShouldSucceed()
    {
        (-1).Verify().ToBeLessThan(0);
    }

    [Fact]
    public void ToBeLessThan_WithNegative_ShouldSucceed()
    {
        (-100).Verify().ToBeLessThan(-1);
    }

    [Fact]
    public void ToBeApproximately_WithZeroPrecision_ShouldSucceed()
    {
        (5.0).Verify().ToBeApproximately(5.0, 0.0);
    }

    [Fact]
    public void ToBeApproximately_WithZeroPrecision_AndMismatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (5.0).Verify().ToBeApproximately(5.1, 0.0));
    }

    [Fact]
    public void ToBeInRange_WhenMinEqualsMax_ShouldSucceed()
    {
        (5).Verify().ToBeInRange(5, 5);
    }

    [Fact]
    public void ToBeInRange_WithInvertedRange_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (5).Verify().ToBeInRange(10, 1));
    }

    [Fact]
    public void ToBe_WithinScope_WhenValuesDiffer_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (1).Verify().ToBe(2);
        });
        Xunit.Assert.Contains("Got", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ToBe_WithinScope_MultipleFailures_ShouldThrowAggregate()
    {
        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using var scope = new AssertionScope();
            (1).Verify().ToBe(2);
            (3).Verify().ToBe(4);
        });
        Xunit.Assert.Equal(2, ex.InnerExceptions.Count);
    }
}
