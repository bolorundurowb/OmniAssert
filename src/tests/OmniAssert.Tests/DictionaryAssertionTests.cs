using OmniAssert;
namespace OmniAssert.Tests;

public class DictionaryAssertionTests
{
    [Fact]
    public void ContainKey_WhenPresent_ShouldSucceed()
    {
        (new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 }).Verify().ContainKey("a");
    }

    [Fact]
    public void ContainKey_WhenMissing_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Dictionary<string, int> { ["a"] = 1 }).Verify().ContainKey("b"));
    }

    [Fact]
    public void NotContainKey_WhenMissing_ShouldSucceed()
    {
        (new Dictionary<string, int> { ["a"] = 1 }).Verify().NotContainKey("b");
    }

    [Fact]
    public void NotContainKey_WhenPresent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Dictionary<string, int> { ["a"] = 1 }).Verify().NotContainKey("a"));
    }

    [Fact]
    public void ContainValue_WhenPresent_ShouldSucceed()
    {
        (new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 }).Verify().ContainValue(2);
    }

    [Fact]
    public void ContainValue_WhenMissing_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Dictionary<string, int> { ["a"] = 1 }).Verify().ContainValue(2));
    }

    [Fact]
    public void HaveValue_WhenKeyAndValueMatch_ShouldSucceed()
    {
        (new Dictionary<string, int> { ["a"] = 1 }).Verify().HaveValue("a", 1);
    }

    [Fact]
    public void HaveValue_WhenKeyMissing_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Dictionary<string, int> { ["a"] = 1 }).Verify().HaveValue("b", 1));
    }

    [Fact]
    public void HaveValue_WhenValueDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Dictionary<string, int> { ["a"] = 1 }).Verify().HaveValue("a", 2));
    }

    [Fact]
    public void DictionaryAssertions_WhenTypedAsIReadOnlyDictionary_ShouldUseDictionaryAssertions()
    {
        IReadOnlyDictionary<string, int> actual = new Dictionary<string, int> { ["x"] = 10 };
        (actual).Verify().ContainKey("x");
        (actual).Verify().ContainValue(10);
    }
}
