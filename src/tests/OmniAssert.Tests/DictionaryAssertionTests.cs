using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class DictionaryAssertionTests
{
    [Fact]
    public void ContainKey_WhenPresent_ShouldSucceed()
    {
        Verify(new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 }).ContainKey("a");
    }

    [Fact]
    public void ContainKey_WhenMissing_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new Dictionary<string, int> { ["a"] = 1 }).ContainKey("b"));
    }

    [Fact]
    public void NotContainKey_WhenMissing_ShouldSucceed()
    {
        Verify(new Dictionary<string, int> { ["a"] = 1 }).NotContainKey("b");
    }

    [Fact]
    public void NotContainKey_WhenPresent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new Dictionary<string, int> { ["a"] = 1 }).NotContainKey("a"));
    }

    [Fact]
    public void ContainValue_WhenPresent_ShouldSucceed()
    {
        Verify(new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 }).ContainValue(2);
    }

    [Fact]
    public void ContainValue_WhenMissing_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new Dictionary<string, int> { ["a"] = 1 }).ContainValue(2));
    }

    [Fact]
    public void HaveValue_WhenKeyAndValueMatch_ShouldSucceed()
    {
        Verify(new Dictionary<string, int> { ["a"] = 1 }).HaveValue("a", 1);
    }

    [Fact]
    public void HaveValue_WhenKeyMissing_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new Dictionary<string, int> { ["a"] = 1 }).HaveValue("b", 1));
    }

    [Fact]
    public void HaveValue_WhenValueDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new Dictionary<string, int> { ["a"] = 1 }).HaveValue("a", 2));
    }

    [Fact]
    public void DictionaryAssertions_WhenTypedAsIReadOnlyDictionary_ShouldUseDictionaryAssertions()
    {
        IReadOnlyDictionary<string, int> actual = new Dictionary<string, int> { ["x"] = 10 };
        Verify(actual).ContainKey("x");
        Verify(actual).ContainValue(10);
    }
}
