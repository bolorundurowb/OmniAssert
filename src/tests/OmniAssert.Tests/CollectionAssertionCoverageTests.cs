namespace OmniAssert.Tests;

public class CollectionAssertionCoverageTests
{
    [Fact]
    public void Contain_WhenCollectionIsNull_ShouldThrow()
    {
        IEnumerable<int>? values = null;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => values.Must().Contain(1));
        Xunit.Assert.Contains("not to be null", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(nameof(CollectionAssertions<int>.BeEmpty))]
    [InlineData(nameof(CollectionAssertions<int>.NotBeEmpty))]
    [InlineData(nameof(CollectionAssertions<int>.HaveCount))]
    [InlineData(nameof(CollectionAssertions<int>.HaveCountGreaterThan))]
    [InlineData(nameof(CollectionAssertions<int>.HaveCountLessThan))]
    [InlineData(nameof(CollectionAssertions<int>.BeUnique))]
    [InlineData(nameof(CollectionAssertions<int>.HaveUniqueCount))]
    [InlineData(nameof(CollectionAssertions<int>.BeInAscendingOrder))]
    [InlineData(nameof(CollectionAssertions<int>.BeInDescendingOrder))]
    [InlineData(nameof(CollectionAssertions<int>.AllSatisfy))]
    [InlineData(nameof(CollectionAssertions<int>.AnySatisfy))]
    [InlineData(nameof(CollectionAssertions<int>.NoneSatisfy))]
    [InlineData(nameof(CollectionAssertions<int>.HaveCountMatching))]
    [InlineData(nameof(CollectionAssertions<int>.BeEquivalentTo))]
    [InlineData(nameof(CollectionAssertions<int>.BeSequenceEqual))]
    [InlineData(nameof(CollectionAssertions<int>.ContainInOrder))]
    public void Methods_WhenCollectionIsNull_ShouldThrow(string _)
    {
        IEnumerable<int>? values = null;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => InvokeNullCollectionAssertion(values!, _));
        Xunit.Assert.Contains("not to be null", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Contain_WithICollectionFastPath_WhenItemPresent_ShouldSucceed()
    {
        ICollection<int> list = new List<int> { 1, 2, 3 };
        list.Must().Contain(2);
    }

    [Fact]
    public void NotContain_WithICollectionFastPath_WhenItemPresent_ShouldThrow()
    {
        ICollection<int> list = new List<int> { 1, 2, 3 };
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => list.Must().NotContain(2));
        Xunit.Assert.Contains("not to contain", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BeEmpty_WithNonICollectionEnumerable_WhenNotEmpty_ShouldThrow()
    {
        static IEnumerable<int> YieldOne()
        {
            yield return 1;
        }

        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => YieldOne().Must().BeEmpty());
        Xunit.Assert.Contains("to be empty", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void HaveCountGreaterThan_WithNonICollectionEnumerable_WhenCountIsGreater_ShouldSucceed()
    {
        static IEnumerable<int> YieldThree()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        YieldThree().Must().HaveCountGreaterThan(2);
    }

    [Fact]
    public void HaveCountLessThan_WithNonICollectionEnumerable_WhenCountIsLess_ShouldSucceed()
    {
        static IEnumerable<int> YieldTwo()
        {
            yield return 1;
            yield return 2;
        }

        YieldTwo().Must().HaveCountLessThan(3);
    }

    [Fact]
    public void AllSatisfy_WhenOneElementFails_ShouldIncludeIndexInMessage()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 1, 2, -1 }).Must().AllSatisfy(x => x > 0));
        Xunit.Assert.Contains("index 2", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NotContain_WithPredicate_WhenMatchExists_ShouldIncludeIndexInMessage()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 1, 2, 3 }).Must().NotContain(x => x == 2));
        Xunit.Assert.Contains("index 1", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NoneSatisfy_WhenOneElementMatches_ShouldIncludeIndexInMessage()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 1, 2, 3 }).Must().NoneSatisfy(x => x == 2));
        Xunit.Assert.Contains("index 1", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BeUnique_WhenDuplicateExists_ShouldIncludeIndexInMessage()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 1, 2, 2 }).Must().BeUnique());
        Xunit.Assert.Contains("index 2", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BeEquivalentTo_WhenExpectedElementIsMissing_ShouldThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 1, 2, 3 }).Must().BeEquivalentTo(new[] { 1, 2, 9 }));
        Xunit.Assert.Contains("0 occurrences", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BeEquivalentTo_WhenEquivalentObjectsInCollection_ShouldSucceed()
    {
        var actual = new[] { new { A = 1 }, new { A = 1 } };
        var expected = new[] { new { A = 1 }, new { A = 1 } };
        actual.Must().BeEquivalentTo(expected);
    }

    [Fact]
    public void BeSequenceEqual_WhenStructurallyEqualObjectsMatchInOrder_ShouldSucceed()
    {
        var left = new[] { new { A = 1 }, new { A = 2 } };
        var right = new[] { new { A = 1 }, new { A = 2 } };
        left.Must().BeSequenceEqual(right);
    }

    [Fact]
    public void BeSequenceEqual_WhenStructurallyEqualObjectsDifferAtIndex_ShouldThrow()
    {
        var left = new[] { new { A = 1 }, new { A = 2 } };
        var right = new[] { new { A = 1 }, new { A = 3 } };
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => left.Must().BeSequenceEqual(right));
        Xunit.Assert.Contains("index 1", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ContainInOrder_WhenStructurallyEqualItemsAppearInOrder_ShouldSucceed()
    {
        var items = new[] { new { Id = 1 }, new { Id = 2 }, new { Id = 3 } };
        items.Must().ContainInOrder(new[] { new { Id = 1 }, new { Id = 3 } });
    }

    [Fact]
    public void BeInAscendingOrder_WhenComparerThrows_ShouldThrowWithComparisonMessage()
    {
        var items = new[] { new ThrowingComparable(), new ThrowingComparable() };
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => items.Must().BeInAscendingOrder());
        Xunit.Assert.Contains("could not be compared", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BeInDescendingOrder_WhenComparerThrows_ShouldThrowWithComparisonMessage()
    {
        var items = new[] { new ThrowingComparable(), new ThrowingComparable() };
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => items.Must().BeInDescendingOrder());
        Xunit.Assert.Contains("could not be compared", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BeInAscendingOrder_WithKeySelector_WhenComparerThrows_ShouldThrowWithComparisonMessage()
    {
        var items = new[] { new KeyHolder(new ThrowingComparable()), new KeyHolder(new ThrowingComparable()) };
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => items.Must().BeInAscendingOrder(x => x.Key));
        Xunit.Assert.Contains("could not be compared", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BeInDescendingOrder_WithKeySelector_WhenComparerThrows_ShouldThrowWithComparisonMessage()
    {
        var items = new[] { new KeyHolder(new ThrowingComparable()), new KeyHolder(new ThrowingComparable()) };
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => items.Must().BeInDescendingOrder(x => x.Key));
        Xunit.Assert.Contains("could not be compared", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BeInAscendingOrder_WhenOutOfOrder_ShouldIncludeIndexesInMessage()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 3, 1 }).Must().BeInAscendingOrder());
        Xunit.Assert.Contains("index 0", ex.Message, StringComparison.OrdinalIgnoreCase);
        Xunit.Assert.Contains("index 1", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BeInAscendingOrder_WithKeySelector_WhenOutOfOrder_ShouldIncludeIndexesInMessage()
    {
        var items = new[] { new { Age = 30 }, new { Age = 10 } };
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => items.Must().BeInAscendingOrder(x => x.Age));
        Xunit.Assert.Contains("index 0", ex.Message, StringComparison.OrdinalIgnoreCase);
        Xunit.Assert.Contains("index 1", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static void InvokeNullCollectionAssertion(IEnumerable<int> values, string method)
    {
        switch (method)
        {
            case nameof(CollectionAssertions<int>.BeEmpty):
                values.Must().BeEmpty();
                break;
            case nameof(CollectionAssertions<int>.NotBeEmpty):
                values.Must().NotBeEmpty();
                break;
            case nameof(CollectionAssertions<int>.HaveCount):
                values.Must().HaveCount(0);
                break;
            case nameof(CollectionAssertions<int>.HaveCountGreaterThan):
                values.Must().HaveCountGreaterThan(0);
                break;
            case nameof(CollectionAssertions<int>.HaveCountLessThan):
                values.Must().HaveCountLessThan(1);
                break;
            case nameof(CollectionAssertions<int>.BeUnique):
                values.Must().BeUnique();
                break;
            case nameof(CollectionAssertions<int>.HaveUniqueCount):
                values.Must().HaveUniqueCount(0);
                break;
            case nameof(CollectionAssertions<int>.BeInAscendingOrder):
                values.Must().BeInAscendingOrder();
                break;
            case nameof(CollectionAssertions<int>.BeInDescendingOrder):
                values.Must().BeInDescendingOrder();
                break;
            case nameof(CollectionAssertions<int>.AllSatisfy):
                values.Must().AllSatisfy(_ => true);
                break;
            case nameof(CollectionAssertions<int>.AnySatisfy):
                values.Must().AnySatisfy(_ => true);
                break;
            case nameof(CollectionAssertions<int>.NoneSatisfy):
                values.Must().NoneSatisfy(_ => false);
                break;
            case nameof(CollectionAssertions<int>.HaveCountMatching):
                values.Must().HaveCountMatching(0, _ => true);
                break;
            case nameof(CollectionAssertions<int>.BeEquivalentTo):
                values.Must().BeEquivalentTo(Array.Empty<int>());
                break;
            case nameof(CollectionAssertions<int>.BeSequenceEqual):
                values.Must().BeSequenceEqual(Array.Empty<int>());
                break;
            case nameof(CollectionAssertions<int>.ContainInOrder):
                values.Must().ContainInOrder(Array.Empty<int>());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(method), method, null);
        }
    }

    private sealed class ThrowingComparable : IComparable<ThrowingComparable>
    {
        public int CompareTo(ThrowingComparable? other) =>
            throw new InvalidOperationException("compare failed");
    }

    private sealed class KeyHolder(ThrowingComparable key)
    {
        public ThrowingComparable Key { get; } = key;
    }
}
