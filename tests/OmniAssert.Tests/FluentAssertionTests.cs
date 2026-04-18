using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class FluentAssertionTests
{
    [Fact]
    public void Verify_IntToBe_WhenValuesEqual_ShouldSucceed()
    {
        Verify(42).ToBe(42);
    }

    [Fact]
    public void Verify_IntToBe_WhenValuesDiffer_ShouldThrowOmniAssertionException()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => Verify(1).ToBe(2));
        Xunit.Assert.Contains("Expected", ex.Message, StringComparison.Ordinal);
        Xunit.Assert.Contains("Actual", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Verify_String_WhenMatching_ShouldSucceed()
    {
        Verify("hello").ToBe("hello");
        Verify("abc").ToContain("b");
    }

    [Fact]
    public void Verify_StringToBeEmpty_WhenNotEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("x").ToBeEmpty());
    }

    [Fact]
    public void Verify_Collection_WhenValid_ShouldSucceed()
    {
        Verify([1, 2, 3]).ToContain(2);
        Verify(Array.Empty<int>()).ToBeEmpty();
    }

    [Fact]
    public void VerifyBool_WhenTrueOrFalse_ShouldSucceed()
    {
        VerifyBool(true).ToBeTrue();
        VerifyBool(false).ToBeFalse();
    }
}
