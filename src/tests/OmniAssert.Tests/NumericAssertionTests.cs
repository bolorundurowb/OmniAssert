using System.Numerics;

namespace OmniAssert.Tests;

public class NumericAssertionTests
{
    [Fact]
    public void ToBe_WhenIntValuesEqual_ShouldSucceed()
    {
        (42).Must().Be(42);
    }

    [Fact]
    public void ToBe_WhenIntValuesDiffer_ShouldThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => (1).Must().Be(2));
        Xunit.Assert.Contains("Expected", ex.Message, StringComparison.Ordinal);
        Xunit.Assert.Contains("Got", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ToBe_WhenLongValuesEqual_ShouldSucceed()
    {
        (100L).Must().Be(100L);
    }

    [Fact]
    public void ToBe_WhenDoubleValuesEqual_ShouldSucceed()
    {
        (3.14).Must().Be(3.14);
    }

    [Fact]
    public void ToBe_WhenFloatValuesEqual_ShouldSucceed()
    {
        (1.5f).Must().Be(1.5f);
    }

    [Fact]
    public void ToBe_WhenDecimalValuesEqual_ShouldSucceed()
    {
        (9.99m).Must().Be(9.99m);
    }

    [Fact]
    public void ToBe_WhenByteValuesEqual_ShouldSucceed()
    {
        ((byte)9).Must().Be((byte)9);
    }

    [Fact]
    public void ToBe_WhenShortValuesEqual_ShouldSucceed()
    {
        ((short)7).Must().Be((short)7);
    }

    [Fact]
    public void ToBe_WhenUIntValuesEqual_ShouldSucceed()
    {
        (42u).Must().Be(42u);
    }

    [Fact]
    public void ToBe_WhenULongValuesEqual_ShouldSucceed()
    {
        (42ul).Must().Be(42ul);
    }

    [Fact]
    public void ToBe_WhenUShortValuesEqual_ShouldSucceed()
    {
        ((ushort)5).Must().Be((ushort)5);
    }

    [Fact]
    public void ToBe_WhenSByteValuesEqual_ShouldSucceed()
    {
        ((sbyte)-3).Must().Be((sbyte)-3);
    }

    [Fact]
    public void ToBe_WhenBigIntegerValuesEqual_ShouldSucceed()
    {
        (new BigInteger(999)).Must().Be(new BigInteger(999));
    }

    [Fact]
    public void NotToBe_WhenIntValuesDiffer_ShouldSucceed()
    {
        (1).Must().NotBe(2);
    }

    [Fact]
    public void NotToBe_WhenIntValuesEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (7).Must().NotBe(7));
    }

    [Fact]
    public void ToBeOneOf_WhenValueIsInSet_ShouldSucceed()
    {
        (2).Must().BeOneOf(1, 2, 3);
    }

    [Fact]
    public void ToBeOneOf_WhenValueIsNotInSet_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (4).Must().BeOneOf(1, 2, 3));
    }

    [Fact]
    public void ToBeGreaterThan_WhenGreater_ShouldSucceed()
    {
        (10).Must().BeGreaterThan(9);
    }

    [Fact]
    public void ToBeGreaterThan_WhenNotGreater_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (1).Must().BeGreaterThan(2));
    }

    [Fact]
    public void ToBeGreaterThan_WhenEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (5).Must().BeGreaterThan(5));
    }

    [Fact]
    public void ToBeGreaterThanOrEqualTo_WhenGreater_ShouldSucceed()
    {
        (5).Must().BeGreaterThanOrEqualTo(4);
    }

    [Fact]
    public void ToBeGreaterThanOrEqualTo_WhenEqual_ShouldSucceed()
    {
        (3).Must().BeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public void ToBeGreaterThanOrEqualTo_WhenLess_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (2).Must().BeGreaterThanOrEqualTo(3));
    }

    [Fact]
    public void ToBeLessThan_WhenLess_ShouldSucceed()
    {
        (2).Must().BeLessThan(3);
    }

    [Fact]
    public void ToBeLessThan_WhenNotLess_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (5).Must().BeLessThan(1));
    }

    [Fact]
    public void ToBeLessThan_WhenEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (4).Must().BeLessThan(4));
    }

    [Fact]
    public void ToBeLessThanOrEqualTo_WhenLess_ShouldSucceed()
    {
        (2).Must().BeLessThanOrEqualTo(3);
    }

    [Fact]
    public void ToBeLessThanOrEqualTo_WhenEqual_ShouldSucceed()
    {
        (4).Must().BeLessThanOrEqualTo(4);
    }

    [Fact]
    public void ToBeLessThanOrEqualTo_WhenGreater_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (6).Must().BeLessThanOrEqualTo(5));
    }

    [Fact]
    public void ToBeInRange_WhenInRange_ShouldSucceed()
    {
        (5).Must().BeInRange(1, 10);
    }

    [Fact]
    public void ToBeInRange_WhenAtLowerBound_ShouldSucceed()
    {
        (1).Must().BeInRange(1, 10);
    }

    [Fact]
    public void ToBeInRange_WhenAtUpperBound_ShouldSucceed()
    {
        (10).Must().BeInRange(1, 10);
    }

    [Fact]
    public void ToBeInRange_WhenAboveUpperBound_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (100).Must().BeInRange(0, 10));
    }

    [Fact]
    public void ToBeInRange_WhenBelowLowerBound_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (0).Must().BeInRange(1, 10));
    }

    [Fact]
    public void ToBeApproximately_WhenWithinTolerance_ShouldSucceed()
    {
        (1.0).Must().BeApproximately(1.001, 0.01);
    }

    [Fact]
    public void ToBeApproximately_WhenExact_ShouldSucceed()
    {
        (5.0).Must().BeApproximately(5.0, 0.0001);
    }

    [Fact]
    public void ToBeApproximately_WhenOutsideTolerance_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (1.0).Must().BeApproximately(2.0, 0.5));
    }

    [Fact]
    public void ToBeApproximately_WhenPrecisionIsNegative_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (1.0).Must().BeApproximately(1.0, -0.1));
    }

    [Fact]
    public void ToBeInRange_WhenDoubleInBounds_ShouldSucceed()
    {
        (5.0).Must().BeInRange(1.0, 10.0);
    }

    [Fact]
    public void ToBeApproximately_WithFloatingPointNaN_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            double.NaN.Must().BeApproximately(1.0, 0.1));
    }

    [Fact]
    public void ToBeApproximately_WhenBothNaN_ShouldFail()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            double.NaN.Must().BeApproximately(double.NaN, 0.1));
    }

    [Fact]
    public void ToBeApproximately_WithFloatingPointPositiveInfinity_ShouldSucceed()
    {
        double.PositiveInfinity.Must().BeApproximately(double.PositiveInfinity, 0.1);
    }

    [Fact]
    public void ToBeApproximately_WithFloatingPointNegativeInfinity_ShouldSucceed()
    {
        double.NegativeInfinity.Must().BeApproximately(double.NegativeInfinity, 0.1);
    }

    [Fact]
    public void ToBeInRange_WithVeryLargeNumbers_ShouldSucceed()
    {
        long.MaxValue.Must().BeInRange(long.MinValue, long.MaxValue);
    }

    [Fact]
    public void ToBeInRange_WithVerySmallNumbers_ShouldSucceed()
    {
        long.MinValue.Must().BeInRange(long.MinValue, long.MaxValue);
    }

    [Fact]
    public void ToBeGreaterThan_WithZero_ShouldSucceed()
    {
        (1).Must().BeGreaterThan(0);
    }

    [Fact]
    public void ToBeGreaterThan_WithNegative_ShouldSucceed()
    {
        (0).Must().BeGreaterThan(-1);
    }

    [Fact]
    public void ToBeLessThan_WithZero_ShouldSucceed()
    {
        (-1).Must().BeLessThan(0);
    }

    [Fact]
    public void ToBeLessThan_WithNegative_ShouldSucceed()
    {
        (-100).Must().BeLessThan(-1);
    }

    [Fact]
    public void ToBeApproximately_WithZeroPrecision_ShouldSucceed()
    {
        (5.0).Must().BeApproximately(5.0, 0.0);
    }

    [Fact]
    public void ToBeApproximately_WithZeroPrecision_AndMismatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (5.0).Must().BeApproximately(5.1, 0.0));
    }

    [Fact]
    public void ToBeInRange_WhenMinEqualsMax_ShouldSucceed()
    {
        (5).Must().BeInRange(5, 5);
    }

    [Fact]
    public void ToBeInRange_WithInvertedRange_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (5).Must().BeInRange(10, 1));
    }

    [Fact]
    public void ToBe_WithinScope_WhenValuesDiffer_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (1).Must().Be(2);
        });
        Xunit.Assert.Contains("Got", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ToBe_WithinScope_MultipleFailures_ShouldThrowAggregate()
    {
        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using var scope = new AssertionScope();
            (1).Must().Be(2);
            (3).Must().Be(4);
        });
        Xunit.Assert.Equal(2, ex.InnerExceptions.Count);
    }

    [Fact]
    public void BeGreaterThanOrEqualTo_WithDouble_WhenLess_ShouldThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => (1.5).Must().BeGreaterThanOrEqualTo(2.5));
        Xunit.Assert.Contains("greater than or equal to", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeLessThanOrEqualTo_WithDouble_WhenGreater_ShouldThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => (5.0).Must().BeLessThanOrEqualTo(3.0));
        Xunit.Assert.Contains("less than or equal to", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeApproximately_WithDouble_WhenOutsideTolerance_ShouldThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => (10.0).Must().BeApproximately(20.0, 0.5));
        Xunit.Assert.Contains("approximately", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeApproximately_WithFloat_WhenWithinTolerance_ShouldSucceed()
    {
        (1.0f).Must().BeApproximately(1.001f, 0.01f);
    }

    [Fact]
    public void BeApproximately_WithFloat_WhenOutsideTolerance_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (1.0f).Must().BeApproximately(2.0f, 0.5f));
    }

    [Fact]
    public void BeApproximately_WithDecimal_WhenWithinTolerance_ShouldSucceed()
    {
        (1.0m).Must().BeApproximately(1.001m, 0.01m);
    }

    [Fact]
    public void BeApproximately_WithDecimal_WhenOutsideTolerance_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (1.0m).Must().BeApproximately(2.0m, 0.5m));
    }

    [Fact]
    public void BeApproximately_WithLong_WhenWithinTolerance_ShouldSucceed()
    {
        (100L).Must().BeApproximately(102L, 5L);
    }

    [Fact]
    public void BeApproximately_WithLong_WhenOutsideTolerance_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (100L).Must().BeApproximately(200L, 5L));
    }

    [Fact]
    public void BeOneOf_WithEmptyArray_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (1).Must().BeOneOf(Array.Empty<int>()));
    }

    [Fact]
    public void BeGreaterThanOrEqualTo_WithFloat_WhenGreater_ShouldSucceed()
    {
        (5.0f).Must().BeGreaterThanOrEqualTo(4.0f);
    }

    [Fact]
    public void BeGreaterThanOrEqualTo_WithFloat_WhenLess_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (1.0f).Must().BeGreaterThanOrEqualTo(2.0f));
    }

    [Fact]
    public void BeLessThanOrEqualTo_WithFloat_WhenLess_ShouldSucceed()
    {
        (1.0f).Must().BeLessThanOrEqualTo(2.0f);
    }

    [Fact]
    public void BeLessThanOrEqualTo_WithFloat_WhenGreater_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (5.0f).Must().BeLessThanOrEqualTo(3.0f));
    }

    [Fact]
    public void BeGreaterThanOrEqualTo_WithDecimal_WhenGreater_ShouldSucceed()
    {
        (5.0m).Must().BeGreaterThanOrEqualTo(4.0m);
    }

    [Fact]
    public void BeGreaterThanOrEqualTo_WithDecimal_WhenLess_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (1.0m).Must().BeGreaterThanOrEqualTo(2.0m));
    }

    [Fact]
    public void BeLessThanOrEqualTo_WithDecimal_WhenLess_ShouldSucceed()
    {
        (1.0m).Must().BeLessThanOrEqualTo(2.0m);
    }

    [Fact]
    public void BeLessThanOrEqualTo_WithDecimal_WhenGreater_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (5.0m).Must().BeLessThanOrEqualTo(3.0m));
    }

    [Fact]
    public void BeInRange_WithDouble_WhenOutOfRange_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (100.0).Must().BeInRange(0.0, 10.0));
    }

    [Fact]
    public void BeInRange_WithDouble_WhenBelowLowerBound_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (-1.0).Must().BeInRange(0.0, 10.0));
    }

    [Fact]
    public void NotBe_WithLong_WhenEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (7L).Must().NotBe(7L));
    }

    [Fact]
    public void BeGreaterThan_WithLong_WhenNotGreater_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (1L).Must().BeGreaterThan(2L));
    }

    [Fact]
    public void BeLessThan_WithLong_WhenNotLess_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (5L).Must().BeLessThan(1L));
    }
}
