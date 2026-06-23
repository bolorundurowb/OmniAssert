namespace OmniAssert.Tests;

public class EnumAssertionTests
{
    public enum TestEnum
    {
        First,
        Second,
        Third
    }

    [Fact]
    public void ToBe_WhenValuesEqual_ShouldSucceed()
    {
        (TestEnum.First).Must().Be(TestEnum.First);
    }

    [Fact]
    public void ToBe_WhenValuesDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (TestEnum.First).Must().Be(TestEnum.Second));
    }

    [Fact]
    public void ToBe_WhenValuesDiffer_MessageContainsExpectedAndActual()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            (TestEnum.First).Must().Be(TestEnum.Second));
        Xunit.Assert.Contains("Second", ex.Message, StringComparison.Ordinal);
        Xunit.Assert.Contains("First", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ToBe_WithinScope_WhenValuesDiffer_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (TestEnum.First).Must().Be(TestEnum.Second);
        });
        Xunit.Assert.Contains("Second", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NotToBe_WhenValuesDiffer_ShouldSucceed()
    {
        (TestEnum.First).Must().NotBe(TestEnum.Second);
    }

    [Fact]
    public void NotToBe_WhenValuesEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (TestEnum.First).Must().NotBe(TestEnum.First));
    }

    [Fact]
    public void NotToBe_WhenValuesEqual_MessageContainsValue()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            (TestEnum.Third).Must().NotBe(TestEnum.Third));
        Xunit.Assert.Contains("Third", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NotToBe_WithinScope_WhenValuesEqual_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (TestEnum.First).Must().NotBe(TestEnum.First);
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void ToBeOneOf_WhenValueIsInSet_ShouldSucceed()
    {
        (TestEnum.Second).Must().BeOneOf(TestEnum.First, TestEnum.Second);
    }

    [Fact]
    public void ToBeOneOf_WhenValueIsNotInSet_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (TestEnum.Third).Must().BeOneOf(TestEnum.First, TestEnum.Second));
    }
}
