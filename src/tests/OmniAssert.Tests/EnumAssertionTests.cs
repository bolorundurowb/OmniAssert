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
        (TestEnum.First).Verify().ToBe(TestEnum.First);
    }

    [Fact]
    public void ToBe_WhenValuesDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (TestEnum.First).Verify().ToBe(TestEnum.Second));
    }

    [Fact]
    public void ToBe_WhenValuesDiffer_MessageContainsExpectedAndActual()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            (TestEnum.First).Verify().ToBe(TestEnum.Second));
        Xunit.Assert.Contains("Second", ex.Message, StringComparison.Ordinal);
        Xunit.Assert.Contains("First", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ToBe_WithinScope_WhenValuesDiffer_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (TestEnum.First).Verify().ToBe(TestEnum.Second);
        });
        Xunit.Assert.Contains("Second", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NotToBe_WhenValuesDiffer_ShouldSucceed()
    {
        (TestEnum.First).Verify().NotToBe(TestEnum.Second);
    }

    [Fact]
    public void NotToBe_WhenValuesEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (TestEnum.First).Verify().NotToBe(TestEnum.First));
    }

    [Fact]
    public void NotToBe_WhenValuesEqual_MessageContainsValue()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            (TestEnum.Third).Verify().NotToBe(TestEnum.Third));
        Xunit.Assert.Contains("Third", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NotToBe_WithinScope_WhenValuesEqual_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (TestEnum.First).Verify().NotToBe(TestEnum.First);
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void ToBeOneOf_WhenValueIsInSet_ShouldSucceed()
    {
        (TestEnum.Second).Verify().ToBeOneOf(TestEnum.First, TestEnum.Second);
    }

    [Fact]
    public void ToBeOneOf_WhenValueIsNotInSet_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (TestEnum.Third).Verify().ToBeOneOf(TestEnum.First, TestEnum.Second));
    }
}
