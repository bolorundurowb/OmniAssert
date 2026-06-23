namespace OmniAssert.Tests;

public class DictionaryAssertionTests
{
    [Fact]
    public void ContainKey_WhenPresent_ShouldSucceed()
    {
        (new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 }).Must().ContainKey("a");
    }

    [Fact]
    public void ContainKey_WhenMissing_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Dictionary<string, int> { ["a"] = 1 }).Must().ContainKey("b"));
    }

    [Fact]
    public void NotContainKey_WhenMissing_ShouldSucceed()
    {
        (new Dictionary<string, int> { ["a"] = 1 }).Must().NotContainKey("b");
    }

    [Fact]
    public void NotContainKey_WhenPresent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Dictionary<string, int> { ["a"] = 1 }).Must().NotContainKey("a"));
    }

    [Fact]
    public void ContainValue_WhenPresent_ShouldSucceed()
    {
        (new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 }).Must().ContainValue(2);
    }

    [Fact]
    public void ContainValue_WhenMissing_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Dictionary<string, int> { ["a"] = 1 }).Must().ContainValue(2));
    }

    [Fact]
    public void HaveValue_WhenKeyAndValueMatch_ShouldSucceed()
    {
        (new Dictionary<string, int> { ["a"] = 1 }).Must().HaveValue("a", 1);
    }

    [Fact]
    public void HaveValue_WhenKeyMissing_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Dictionary<string, int> { ["a"] = 1 }).Must().HaveValue("b", 1));
    }

    [Fact]
    public void HaveValue_WhenValueDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Dictionary<string, int> { ["a"] = 1 }).Must().HaveValue("a", 2));
    }

    [Fact]
    public void DictionaryAssertions_WhenTypedAsIReadOnlyDictionary_ShouldUseDictionaryAssertions()
    {
        IReadOnlyDictionary<string, int> actual = new Dictionary<string, int> { ["x"] = 10 };
        (actual).Must().ContainKey("x");
        (actual).Must().ContainValue(10);
    }

    [Fact]
    public void NotContainValue_WhenMissing_ShouldSucceed()
    {
        (new Dictionary<string, int> { ["a"] = 1 }).Must().NotContainValue(99);
    }

    [Fact]
    public void NotContainValue_WhenPresent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Dictionary<string, int> { ["a"] = 1 }).Must().NotContainValue(1));
    }

    [Fact]
    public void ContainKey_WithNullKey_ShouldSucceed()
    {
        var dict = new DictionaryWithNullKey();
        (dict).Must().ContainKey(null);
    }

    private class DictionaryWithNullKey : IReadOnlyDictionary<string?, int>
    {
        public int this[string? key] => key == null ? 42 : throw new KeyNotFoundException();
        public IEnumerable<string?> Keys => [null];
        public IEnumerable<int> Values => [42];
        public int Count => 1;
        public bool ContainsKey(string? key) => key == null;
        public IEnumerator<KeyValuePair<string?, int>> GetEnumerator()
        {
            yield return new KeyValuePair<string?, int>(null, 42);
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        public bool TryGetValue(string? key, out int value)
        {
            if (key == null)
            {
                value = 42;
                return true;
            }
            value = 0;
            return false;
        }
    }

    [Fact]
    public void ContainValue_WithNullValue_ShouldSucceed()
    {
        (new Dictionary<string, string?> { ["a"] = null }).Must().ContainValue(null);
    }

    [Fact]
    public void HaveValue_WithNullValue_ShouldSucceed()
    {
        (new Dictionary<string, string?> { ["a"] = null }).Must().HaveValue("a", null);
    }

    [Fact]
    public void HaveValue_WithNullValueMismatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Dictionary<string, string?> { ["a"] = null }).Must().HaveValue("a", "expected"));
    }

    [Fact]
    public void ContainKey_WhenScopeActive_ShouldCollectFailure()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (new Dictionary<string, int> { ["a"] = 1 }).Must().ContainKey("missing");
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void ContainValue_WhenScopeActive_ShouldCollectFailure()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (new Dictionary<string, int> { ["a"] = 1 }).Must().ContainValue(99);
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void HaveValue_WhenScopeActive_ShouldCollectFailure()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (new Dictionary<string, int> { ["a"] = 1 }).Must().HaveValue("a", 99);
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void ToBeNull_WhenNull_ShouldSucceed()
    {
        ((IReadOnlyDictionary<string, int>?)null).Must().BeNull();
    }

    [Fact]
    public void ToBeNull_WhenNotNull_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Dictionary<string, int> { ["a"] = 1 }).Must().BeNull());
    }

    [Fact]
    public void NotToBeNull_WhenNotNull_ShouldSucceed()
    {
        (new Dictionary<string, int> { ["a"] = 1 }).Must().NotBeNull();
    }

    [Fact]
    public void NotToBeNull_WhenNull_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            ((IReadOnlyDictionary<string, int>?)null).Must().NotBeNull());
    }

    [Fact]
    public void ToHaveCount_WhenCountMatches_ShouldSucceed()
    {
        (new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 }).Must().HaveCount(2);
    }

    [Fact]
    public void ToHaveCount_WhenCountDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Dictionary<string, int> { ["a"] = 1 }).Must().HaveCount(3));
    }

    [Fact]
    public void ToHaveCount_WhenNull_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            ((IReadOnlyDictionary<string, int>?)null).Must().HaveCount(0));
    }
}
