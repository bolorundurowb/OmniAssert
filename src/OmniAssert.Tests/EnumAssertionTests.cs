using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class EnumAssertionTests
{
    public enum TestEnum
    {
        First,
        Second
    }

    [Fact]
    public void Verify_EnumToBe_WhenValuesEqual_ShouldSucceed()
    {
        Verify(TestEnum.First).ToBe(TestEnum.First);
    }

    [Fact]
    public void Verify_EnumToBe_WhenValuesDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(TestEnum.First).ToBe(TestEnum.Second));
    }

    [Fact]
    public void Verify_EnumNotToBe_WhenValuesDiffer_ShouldSucceed()
    {
        Verify(TestEnum.First).NotToBe(TestEnum.Second);
    }

    [Fact]
    public void Verify_EnumNotToBe_WhenValuesEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(TestEnum.First).NotToBe(TestEnum.First));
    }
}
