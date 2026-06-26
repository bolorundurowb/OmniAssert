namespace OmniAssert.Tests;

public class NumericSignAndExtraAssertionTests
{
    // BePositive / BeNegative
    [Fact]
    public void BePositive_WhenPositive_ShouldSucceed() => (5).Must().BePositive();

    [Fact]
    public void BePositive_WhenZero_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => (0).Must().BePositive());

    [Fact]
    public void BePositive_WhenNegative_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => (-3).Must().BePositive());

    [Fact]
    public void BePositive_WithDouble_ShouldSucceed() => (0.5).Must().BePositive();

    [Fact]
    public void BeNegative_WhenNegative_ShouldSucceed() => (-3).Must().BeNegative();

    [Fact]
    public void BeNegative_WhenZero_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => (0).Must().BeNegative());

    [Fact]
    public void BeNegative_WithDecimal_ShouldSucceed() => (-1.5m).Must().BeNegative();

    // BeEven / BeOdd
    [Fact]
    public void BeEven_WhenEven_ShouldSucceed() => (4).Must().BeEven();

    [Fact]
    public void BeEven_WhenNegativeEven_ShouldSucceed() => (-8).Must().BeEven();

    [Fact]
    public void BeEven_WhenOdd_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => (3).Must().BeEven());

    [Fact]
    public void BeEven_WhenFractionalDouble_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => (2.5).Must().BeEven());

    [Fact]
    public void BeOdd_WhenOdd_ShouldSucceed() => (7).Must().BeOdd();

    [Fact]
    public void BeOdd_WhenEven_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => (8).Must().BeOdd());

    [Fact]
    public void BeOdd_WithLong_ShouldSucceed() => (3L).Must().BeOdd();

    // BeMultipleOf
    [Fact]
    public void BeMultipleOf_WhenDivisible_ShouldSucceed() => (12).Must().BeMultipleOf(3);

    [Fact]
    public void BeMultipleOf_WhenNotDivisible_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => (13).Must().BeMultipleOf(3));

    [Fact]
    public void BeMultipleOf_WhenFactorIsZero_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => (10).Must().BeMultipleOf(0));

    // BeFinite / BeInfinite
    [Fact]
    public void BeFinite_WhenFinite_ShouldSucceed() => (1.5).Must().BeFinite();

    [Fact]
    public void BeFinite_WhenInteger_ShouldSucceed() => (42).Must().BeFinite();

    [Fact]
    public void BeFinite_WhenInfinity_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => double.PositiveInfinity.Must().BeFinite());

    [Fact]
    public void BeFinite_WhenNaN_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => double.NaN.Must().BeFinite());

    [Fact]
    public void BeInfinite_WhenInfinity_ShouldSucceed() => double.NegativeInfinity.Must().BeInfinite();

    [Fact]
    public void BeInfinite_WhenFinite_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => (1.0).Must().BeInfinite());

    // HaveDecimalPlaces
    [Fact]
    public void HaveDecimalPlaces_WhenExactDecimal_ShouldSucceed() => (1.25m).Must().HaveDecimalPlaces(2);

    [Fact]
    public void HaveDecimalPlaces_WhenTrailingZeros_ShouldIgnoreThem() => (1.50m).Must().HaveDecimalPlaces(1);

    [Fact]
    public void HaveDecimalPlaces_WhenWholeNumber_ShouldBeZero() => (10m).Must().HaveDecimalPlaces(0);

    [Fact]
    public void HaveDecimalPlaces_WithDouble_ShouldSucceed() => (0.125).Must().HaveDecimalPlaces(3);

    [Fact]
    public void HaveDecimalPlaces_WhenMismatch_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => (1.234m).Must().HaveDecimalPlaces(2));

    [Fact]
    public void HaveDecimalPlaces_WhenNegativeArg_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => (1.0m).Must().HaveDecimalPlaces(-1));
}
