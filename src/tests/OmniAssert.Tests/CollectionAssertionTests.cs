namespace OmniAssert.Tests;

public class CollectionAssertionTests
{
    [Fact]
    public void ToContain_WhenItemPresent_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Must().Contain(2);
    }

    [Fact]
    public void ToContain_WhenItemAbsent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3 }).Must().Contain(4));
    }

    [Fact]
    public void ToContain_NonICollectionEnumerable_WhenItemPresent_ShouldSucceed()
    {
        static IEnumerable<int> Yield123()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        (Yield123()).Must().Contain(2);
    }

    [Fact]
    public void NotToContain_WhenItemAbsent_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Must().NotContain(4);
    }

    [Fact]
    public void NotToContain_WhenItemPresent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3 }).Must().NotContain(2));
    }

    [Fact]
    public void ToBeEmpty_WhenEmpty_ShouldSucceed()
    {
        (Array.Empty<int>()).Must().BeEmpty();
    }

    [Fact]
    public void ToBeEmpty_WhenNotEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1 }).Must().BeEmpty());
    }

    [Fact]
    public void ToBeEmpty_NonICollection_WhenEmpty_ShouldSucceed()
    {
        static IEnumerable<int> YieldNone()
        {
            yield break;
        }

        (YieldNone()).Must().BeEmpty();
    }

    [Fact]
    public void NotToBeEmpty_WhenNotEmpty_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Must().NotBeEmpty();
    }

    [Fact]
    public void NotToBeEmpty_WhenEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (Array.Empty<int>()).Must().NotBeEmpty());
    }

    [Fact]
    public void ToHaveCount_WhenCountMatches_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Must().HaveCount(3);
    }

    [Fact]
    public void ToHaveCount_WhenCountDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3 }).Must().HaveCount(2));
    }

    [Fact]
    public void ToHaveCount_NonICollection_WhenCountMatches_ShouldSucceed()
    {
        static IEnumerable<int> YieldThree()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        (YieldThree()).Must().HaveCount(3);
    }

    [Fact]
    public void ToHaveCountGreaterThan_WhenCountIsGreater_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Must().HaveCountGreaterThan(2);
    }

    [Fact]
    public void ToHaveCountGreaterThan_WhenCountIsEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2 }).Must().HaveCountGreaterThan(2));
    }

    [Fact]
    public void ToHaveCountGreaterThan_WhenCountIsLess_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1 }).Must().HaveCountGreaterThan(2));
    }

    [Fact]
    public void ToHaveCountLessThan_WhenCountIsLess_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Must().HaveCountLessThan(4);
    }

    [Fact]
    public void ToHaveCountLessThan_WhenCountIsEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2 }).Must().HaveCountLessThan(2));
    }

    [Fact]
    public void ToBeUnique_WhenAllElementsUnique_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Must().BeUnique();
    }

    [Fact]
    public void ToBeUnique_WhenDuplicatesExist_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 1 }).Must().BeUnique());
    }

    [Fact]
    public void ToHaveUniqueCount_WhenCorrect_ShouldSucceed()
    {
        (new[] { 1, 1, 2, 3 }).Must().HaveUniqueCount(3);
    }

    [Fact]
    public void ToHaveUniqueCount_WhenCountDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 2, 3 }).Must().HaveUniqueCount(4));
    }

    [Fact]
    public void ToBeInAscendingOrder_WhenSorted_ShouldSucceed()
    {
        (new[] { 1, 2, 2, 3 }).Must().BeInAscendingOrder();
    }

    [Fact]
    public void ToBeInAscendingOrder_WhenUnsorted_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 3, 2 }).Must().BeInAscendingOrder());
    }

    [Fact]
    public void ToBeInDescendingOrder_WhenSorted_ShouldSucceed()
    {
        (new[] { 3, 2, 2, 1 }).Must().BeInDescendingOrder();
    }

    [Fact]
    public void ToBeInDescendingOrder_WhenUnsorted_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 3, 1, 2 }).Must().BeInDescendingOrder());
    }

    [Fact]
    public void AllSatisfy_WhenAllElementsSatisfy_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Must().AllSatisfy(x => x > 0);
    }

    [Fact]
    public void AllSatisfy_WhenOneElementFails_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, -1 }).Must().AllSatisfy(x => x > 0));
    }

    [Fact]
    public void ToBeEquivalentTo_WhenOrderDiffers_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Must().BeEquivalentTo(new[] { 3, 2, 1 });
    }

    [Fact]
    public void ToBeEquivalentTo_WhenElementsDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3 }).Must().BeEquivalentTo(new[] { 1, 2, 4 }));
    }

    [Fact]
    public void ToBeEquivalentTo_WhenCountDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 1, 2 }).Must().BeEquivalentTo(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void ToBeEquivalentTo_WhenMultisetDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 1, 1, 2 }).Must().BeEquivalentTo(new[] { 1, 2, 2 }));
    }

    [Fact]
    public void ToContain_WithPredicate_WhenMatchExists_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Must().Contain(x => x > 2);
    }

    [Fact]
    public void NotToContain_WithPredicate_WhenNoMatchExists_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Must().NotContain(x => x > 10);
    }

    [Fact]
    public void NotToContain_WithPredicate_WhenMatchExists_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3 }).Must().NotContain(x => x > 2));
    }

    [Fact]
    public void AnySatisfy_WhenOneElementSatisfies_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Must().AnySatisfy(x => x == 2);
    }

    [Fact]
    public void AnySatisfy_WhenNoElementSatisfies_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3 }).Must().AnySatisfy(x => x > 10));
    }

    [Fact]
    public void AnySatisfy_WhenAllElementsSatisfy_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Must().AnySatisfy(x => x > 0);
    }

    [Fact]
    public void NoneSatisfy_WhenNoElementSatisfies_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Must().NoneSatisfy(x => x > 10);
    }

    [Fact]
    public void NoneSatisfy_WhenOneElementSatisfies_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3 }).Must().NoneSatisfy(x => x == 2));
    }

    [Fact]
    public void NoneSatisfy_WhenAllElementsSatisfy_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3 }).Must().NoneSatisfy(x => x > 0));
    }

    [Fact]
    public void ToHaveCountMatching_WhenCountIsCorrect_ShouldSucceed()
    {
        (new[] { 1, 2, 3, 4, 5 }).Must().HaveCountMatching(3, x => x > 2);
    }

    [Fact]
    public void ToHaveCountMatching_WhenCountIsWrong_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 1, 2, 3, 4, 5 }).Must().HaveCountMatching(2, x => x > 2));
    }

    [Fact]
    public void ToHaveCountMatching_WhenNoElementsMatch_ShouldSucceedForZero()
    {
        (new[] { 1, 2, 3 }).Must().HaveCountMatching(0, x => x > 10);
    }

    [Fact]
    public void ToBeInAscendingOrder_WithKeySelector_WhenSorted_ShouldSucceed()
    {
        var items = new[] { new { Name = "Alice", Age = 20 }, new { Name = "Bob", Age = 25 }, new { Name = "Carol", Age = 30 } };
        items.Must().BeInAscendingOrder(x => x.Age);
    }

    [Fact]
    public void ToBeInAscendingOrder_WithKeySelector_WhenUnsorted_ShouldThrow()
    {
        var items = new[] { new { Name = "Bob", Age = 25 }, new { Name = "Alice", Age = 20 } };
        Xunit.Assert.Throws<OmniAssertionException>(() => items.Must().BeInAscendingOrder(x => x.Age));
    }

    [Fact]
    public void ToBeInDescendingOrder_WithKeySelector_WhenSorted_ShouldSucceed()
    {
        var items = new[] { new { Name = "Carol", Age = 30 }, new { Name = "Bob", Age = 25 }, new { Name = "Alice", Age = 20 } };
        items.Must().BeInDescendingOrder(x => x.Age);
    }

    [Fact]
    public void ToBeInDescendingOrder_WithKeySelector_WhenUnsorted_ShouldThrow()
    {
        var items = new[] { new { Name = "Alice", Age = 20 }, new { Name = "Bob", Age = 25 } };
        Xunit.Assert.Throws<OmniAssertionException>(() => items.Must().BeInDescendingOrder(x => x.Age));
    }

    [Fact]
    public void ToBeEquivalentTo_WithNullElements_ShouldSucceed()
    {
        (new string?[] { null, "a", null }).Must().BeEquivalentTo(new string?[] { "a", null, null });
    }

    [Fact]
    public void ToBeEquivalentTo_WithNestedCollections_ShouldSucceed()
    {
        var actual = new[] { new[] { 1, 2 }, new[] { 3, 4 } };
        var expected = new[] { new[] { 3, 4 }, new[] { 1, 2 } };
        actual.Must().BeEquivalentTo(expected);
    }

    [Fact]
    public void ToContain_WithNullElement_ShouldSucceed()
    {
        (new string?[] { "a", null, "b" }).Must().Contain((string?)null);
    }

    [Fact]
    public void ToContain_WithNullElement_WhenNotPresent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new string?[] { "a", "b" }).Must().Contain((string?)null));
    }

    [Fact]
    public void AllSatisfy_WithEmptyCollection_ShouldSucceed()
    {
        Array.Empty<int>().Must().AllSatisfy(x => x > 0);
    }

    [Fact]
    public void AnySatisfy_WithEmptyCollection_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Array.Empty<int>().Must().AnySatisfy(x => x > 0));
    }

    [Fact]
    public void NoneSatisfy_WithEmptyCollection_ShouldSucceed()
    {
        Array.Empty<int>().Must().NoneSatisfy(x => x > 0);
    }

    [Fact]
    public void ToContain_WithPredicate_WhenNoMatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 1, 2, 3 }).Must().Contain(x => x > 10));
    }

    [Fact]
    public void ToBeInAscendingOrder_WithDuplicates_ShouldSucceed()
    {
        (new[] { 1, 2, 2, 3, 3, 3 }).Must().BeInAscendingOrder();
    }

    [Fact]
    public void ToBeInDescendingOrder_WithDuplicates_ShouldSucceed()
    {
        (new[] { 3, 3, 3, 2, 2, 1 }).Must().BeInDescendingOrder();
    }

    [Fact]
    public void ToHaveCountMatching_WithEmptyCollectionAndZero_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Must().HaveCountMatching(0, x => x > 100);
    }

    [Fact]
    public void ToBeUnique_WithAllUniqueElements_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Must().BeUnique();
    }

    [Fact]
    public void ToBeUnique_WithDuplicateElements_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 1, 2, 2, 3 }).Must().BeUnique());
    }

    [Fact]
    public void ToBeUnique_WithEmptyCollection_ShouldSucceed()
    {
        Array.Empty<int>().Must().BeUnique();
    }

    [Fact]
    public void ToHaveUniqueCount_WhenCountMatches_ShouldSucceed()
    {
        (new[] { 1, 1, 2, 3, 3 }).Must().HaveUniqueCount(3);
    }

    [Fact]
    public void ToContain_WithinScope_WhenItemAbsent_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (new[] { 1, 2 }).Must().Contain(9);
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]      
    public void ToHaveCount_WithinScope_MultipleFailures_ShouldThrowAggregate()
    {
        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using var scope = new AssertionScope();
            (new[] { 1 }).Must().HaveCount(5);
            (new[] { 2 }).Must().HaveCount(5);
        });
        Xunit.Assert.Equal(2, ex.InnerExceptions.Count);
    }

    [Fact]
    public void ToBeSequenceEqual_WhenSequencesMatchInOrder_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Must().BeSequenceEqual(new[] { 1, 2, 3 });
    }

    [Fact]
    public void ToBeSequenceEqual_WhenOrderDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 1, 2, 3 }).Must().BeSequenceEqual(new[] { 3, 2, 1 }));
    }

    [Fact]
    public void ToBeSequenceEqual_WhenElementsDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 1, 2, 3 }).Must().BeSequenceEqual(new[] { 1, 2, 4 }));
    }

    [Fact]
    public void ToBeSequenceEqual_WhenCountDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 1, 2 }).Must().BeSequenceEqual(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void ToContainInOrder_WhenItemsAppearInOrder_ShouldSucceed()
    {
        (new[] { 1, 2, 3, 4, 5 }).Must().ContainInOrder(new[] { 2, 4 });
    }

    [Fact]
    public void ToContainInOrder_WhenItemsAppearOutOfOrder_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 5, 4, 3, 2, 1 }).Must().ContainInOrder(new[] { 2, 4 }));
    }

    [Fact]
    public void ToContainInOrder_WhenItemMissing_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 1, 2, 3 }).Must().ContainInOrder(new[] { 1, 4 }));
    }

    [Fact]
    public void ToContainInOrder_WithEmptyExpected_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Must().ContainInOrder(Array.Empty<int>());
    }

    [Fact]
    public void ToContainInOrder_WhenItemsAreConsecutive_ShouldSucceed()
    {
        (new[] { 1, 2, 3, 4, 5 }).Must().ContainInOrder(new[] { 2, 3, 4 });
    }

    [Fact]
    public void ToBeSequenceEqual_WithEmptyCollections_ShouldSucceed()
    {
        Array.Empty<int>().Must().BeSequenceEqual(Array.Empty<int>());
    }

    [Fact]
    public void NotContain_WithNonICollectionEnumerable_WhenItemAbsent_ShouldSucceed()
    {
        static IEnumerable<int> Yield123()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        (Yield123()).Must().NotContain(4);
    }

    [Fact]
    public void NotContain_WithNonICollectionEnumerable_WhenItemPresent_ShouldThrow()
    {
        static IEnumerable<int> Yield123()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        Xunit.Assert.Throws<OmniAssertionException>(() => (Yield123()).Must().NotContain(2));
    }

    [Fact]
    public void Be_WhenSameReference_ShouldSucceed()
    {
        IEnumerable<int> arr = new[] { 1, 2, 3 };
        arr.Must().Be(arr);
    }

    [Fact]
    public void Be_WhenDifferentReference_ShouldThrow()
    {
        IEnumerable<int> arr1 = new[] { 1, 2, 3 };
        IEnumerable<int> arr2 = new[] { 1, 2, 3 };
        Xunit.Assert.Throws<OmniAssertionException>(() => arr1.Must().Be(arr2));
    }

    [Fact]
    public void BeEquivalentTo_WhenElementCountDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 1, 1, 2 }).Must().BeEquivalentTo(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void NotBeEmpty_NonICollection_WhenEmpty_ShouldThrow()
    {
        static IEnumerable<int> YieldNone()
        {
            yield break;
        }

        Xunit.Assert.Throws<OmniAssertionException>(() => (YieldNone()).Must().NotBeEmpty());
    }

    [Fact]
    public void NotBeEmpty_NonICollection_WhenNotEmpty_ShouldSucceed()
    {
        static IEnumerable<int> Yield1()
        {
            yield return 1;
        }

        (Yield1()).Must().NotBeEmpty();
    }

    [Fact]
    public void HaveCountLessThan_WhenCountIsGreater_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3 }).Must().HaveCountLessThan(2));
    }

    [Fact]
    public void BeInAscendingOrder_WithEmptyCollection_ShouldSucceed()
    {
        Array.Empty<int>().Must().BeInAscendingOrder();
    }

    [Fact]
    public void BeInDescendingOrder_WithEmptyCollection_ShouldSucceed()
    {
        Array.Empty<int>().Must().BeInDescendingOrder();
    }

    [Fact]
    public void BeInAscendingOrder_WithKeySelector_WithEmptyCollection_ShouldSucceed()
    {
        Array.Empty<(int Age, string Name)>().Must().BeInAscendingOrder(x => x.Age);
    }

    [Fact]
    public void BeInDescendingOrder_WithKeySelector_WithEmptyCollection_ShouldSucceed()
    {
        Array.Empty<(int Age, string Name)>().Must().BeInDescendingOrder(x => x.Age);
    }
}
