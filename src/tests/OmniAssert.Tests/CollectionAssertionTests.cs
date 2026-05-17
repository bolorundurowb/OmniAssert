namespace OmniAssert.Tests;

public class CollectionAssertionTests
{
    // ── ToContain ────────────────────────────────────────────────────────────

    [Fact]
    public void ToContain_WhenItemPresent_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Verify().ToContain(2);
    }

    [Fact]
    public void ToContain_WhenItemAbsent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3 }).Verify().ToContain(4));
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

        (Yield123()).Verify().ToContain(2);
    }

    // ── NotToContain ─────────────────────────────────────────────────────────

    [Fact]
    public void NotToContain_WhenItemAbsent_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Verify().NotToContain(4);
    }

    [Fact]
    public void NotToContain_WhenItemPresent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3 }).Verify().NotToContain(2));
    }

    // ── ToBeEmpty / NotToBeEmpty ─────────────────────────────────────────────

    [Fact]
    public void ToBeEmpty_WhenEmpty_ShouldSucceed()
    {
        (Array.Empty<int>()).Verify().ToBeEmpty();
    }

    [Fact]
    public void ToBeEmpty_WhenNotEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1 }).Verify().ToBeEmpty());
    }

    [Fact]
    public void ToBeEmpty_NonICollection_WhenEmpty_ShouldSucceed()
    {
        static IEnumerable<int> YieldNone()
        {
            yield break;
        }

        (YieldNone()).Verify().ToBeEmpty();
    }

    [Fact]
    public void NotToBeEmpty_WhenNotEmpty_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Verify().NotToBeEmpty();
    }

    [Fact]
    public void NotToBeEmpty_WhenEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (Array.Empty<int>()).Verify().NotToBeEmpty());
    }

    // ── HasCount ─────────────────────────────────────────────────────────────

    [Fact]
    public void HasCount_WhenCountMatches_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Verify().HasCount(3);
    }

    [Fact]
    public void HasCount_WhenCountDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3 }).Verify().HasCount(2));
    }

    [Fact]
    public void HasCount_NonICollection_WhenCountMatches_ShouldSucceed()
    {
        static IEnumerable<int> YieldThree()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        (YieldThree()).Verify().HasCount(3);
    }

    [Fact]
    public void ToHaveCount_WhenCountMatches_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Verify().ToHaveCount(3);
    }

    [Fact]
    public void ToHaveCount_WhenCountDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3 }).Verify().ToHaveCount(2));
    }

    // ── HasCountGreaterThan ──────────────────────────────────────────────────

    [Fact]
    public void HasCountGreaterThan_WhenCountIsGreater_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Verify().HasCountGreaterThan(2);
    }

    [Fact]
    public void HasCountGreaterThan_WhenCountIsEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2 }).Verify().HasCountGreaterThan(2));
    }

    [Fact]
    public void HasCountGreaterThan_WhenCountIsLess_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1 }).Verify().HasCountGreaterThan(2));
    }

    // ── HasCountLessThan ─────────────────────────────────────────────────────

    [Fact]
    public void HasCountLessThan_WhenCountIsLess_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Verify().HasCountLessThan(4);
    }

    [Fact]
    public void HasCountLessThan_WhenCountIsEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2 }).Verify().HasCountLessThan(2));
    }

    // ── ToBeUnique ───────────────────────────────────────────────────────────

    [Fact]
    public void ToBeUnique_WhenAllElementsUnique_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Verify().ToBeUnique();
    }

    [Fact]
    public void ToBeUnique_WhenDuplicatesExist_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 1 }).Verify().ToBeUnique());
    }

    // ── HasUniqueCount ───────────────────────────────────────────────────────

    [Fact]
    public void HasUniqueCount_WhenCorrect_ShouldSucceed()
    {
        (new[] { 1, 1, 2, 3 }).Verify().HasUniqueCount(3);
    }

    [Fact]
    public void HasUniqueCount_WhenCountDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 2, 3 }).Verify().HasUniqueCount(4));
    }

    // ── Ordering ─────────────────────────────────────────────────────────────

    [Fact]
    public void ToBeInAscendingOrder_WhenSorted_ShouldSucceed()
    {
        (new[] { 1, 2, 2, 3 }).Verify().ToBeInAscendingOrder();
    }

    [Fact]
    public void ToBeInAscendingOrder_WhenUnsorted_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 3, 2 }).Verify().ToBeInAscendingOrder());
    }

    [Fact]
    public void ToBeInDescendingOrder_WhenSorted_ShouldSucceed()
    {
        (new[] { 3, 2, 2, 1 }).Verify().ToBeInDescendingOrder();
    }

    [Fact]
    public void ToBeInDescendingOrder_WhenUnsorted_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 3, 1, 2 }).Verify().ToBeInDescendingOrder());
    }

    // ── AllSatisfy ───────────────────────────────────────────────────────────

    [Fact]
    public void AllSatisfy_WhenAllElementsSatisfy_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Verify().AllSatisfy(x => x > 0);
    }

    [Fact]
    public void AllSatisfy_WhenOneElementFails_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, -1 }).Verify().AllSatisfy(x => x > 0));
    }

    // ── ToBeEquivalentTo ─────────────────────────────────────────────────────

    [Fact]
    public void ToBeEquivalentTo_WhenOrderDiffers_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Verify().ToBeEquivalentTo(new[] { 3, 2, 1 });
    }

    [Fact]
    public void ToBeEquivalentTo_WhenElementsDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3 }).Verify().ToBeEquivalentTo(new[] { 1, 2, 4 }));
    }

    [Fact]
    public void ToBeEquivalentTo_WhenCountDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 1, 2 }).Verify().ToBeEquivalentTo(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void ToBeEquivalentTo_WhenMultisetDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 1, 1, 2 }).Verify().ToBeEquivalentTo(new[] { 1, 2, 2 }));
    }

    // ── ToContain (predicate) ─────────────────────────────────────────────────

    [Fact]
    public void ToContain_WithPredicate_WhenMatchExists_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Verify().ToContain(x => x > 2);
    }

    [Fact]
    public void ToContain_WithPredicate_WhenNoMatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3 }).Verify().ToContain(x => x > 10));
    }

    // ── NotToContain (predicate) ──────────────────────────────────────────────

    [Fact]
    public void NotToContain_WithPredicate_WhenNoMatchExists_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Verify().NotToContain(x => x > 10);
    }

    [Fact]
    public void NotToContain_WithPredicate_WhenMatchExists_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3 }).Verify().NotToContain(x => x > 2));
    }

    // ── AnySatisfy ────────────────────────────────────────────────────────────

    [Fact]
    public void AnySatisfy_WhenOneElementSatisfies_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Verify().AnySatisfy(x => x == 2);
    }

    [Fact]
    public void AnySatisfy_WhenNoElementSatisfies_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3 }).Verify().AnySatisfy(x => x > 10));
    }

    [Fact]
    public void AnySatisfy_WhenAllElementsSatisfy_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Verify().AnySatisfy(x => x > 0);
    }

    // ── NoneSatisfy ───────────────────────────────────────────────────────────

    [Fact]
    public void NoneSatisfy_WhenNoElementSatisfies_ShouldSucceed()
    {
        (new[] { 1, 2, 3 }).Verify().NoneSatisfy(x => x > 10);
    }

    [Fact]
    public void NoneSatisfy_WhenOneElementSatisfies_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3 }).Verify().NoneSatisfy(x => x == 2));
    }

    [Fact]
    public void NoneSatisfy_WhenAllElementsSatisfy_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3 }).Verify().NoneSatisfy(x => x > 0));
    }

    // ── HasCountMatching ──────────────────────────────────────────────────────

    [Fact]
    public void HasCountMatching_WhenCountIsCorrect_ShouldSucceed()
    {
        (new[] { 1, 2, 3, 4, 5 }).Verify().HasCountMatching(3, x => x > 2);
    }

    [Fact]
    public void HasCountMatching_WhenCountIsWrong_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { 1, 2, 3, 4, 5 }).Verify().HasCountMatching(2, x => x > 2));
    }

    [Fact]
    public void HasCountMatching_WhenNoElementsMatch_ShouldSucceedForZero()
    {
        (new[] { 1, 2, 3 }).Verify().HasCountMatching(0, x => x > 10);
    }

    // ── ToBeInAscendingOrder (key selector) ───────────────────────────────────

    [Fact]
    public void ToBeInAscendingOrder_WithKeySelector_WhenSorted_ShouldSucceed()
    {
        var items = new[] { new { Name = "Alice", Age = 20 }, new { Name = "Bob", Age = 25 }, new { Name = "Carol", Age = 30 } };
        items.Verify().ToBeInAscendingOrder(x => x.Age);
    }

    [Fact]
    public void ToBeInAscendingOrder_WithKeySelector_WhenUnsorted_ShouldThrow()
    {
        var items = new[] { new { Name = "Bob", Age = 25 }, new { Name = "Alice", Age = 20 } };
        Xunit.Assert.Throws<OmniAssertionException>(() => items.Verify().ToBeInAscendingOrder(x => x.Age));
    }

    // ── ToBeInDescendingOrder (key selector) ──────────────────────────────────

    [Fact]
    public void ToBeInDescendingOrder_WithKeySelector_WhenSorted_ShouldSucceed()
    {
        var items = new[] { new { Name = "Carol", Age = 30 }, new { Name = "Bob", Age = 25 }, new { Name = "Alice", Age = 20 } };
        items.Verify().ToBeInDescendingOrder(x => x.Age);
    }

    [Fact]
    public void ToBeInDescendingOrder_WithKeySelector_WhenUnsorted_ShouldThrow()
    {
        var items = new[] { new { Name = "Alice", Age = 20 }, new { Name = "Bob", Age = 25 } };
        Xunit.Assert.Throws<OmniAssertionException>(() => items.Verify().ToBeInDescendingOrder(x => x.Age));
    }

    // ── Scope ────────────────────────────────────────────────────────────────

    [Fact]
    public void ToContain_WithinScope_WhenItemAbsent_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (new[] { 1, 2 }).Verify().ToContain(9);
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void HasCount_WithinScope_MultipleFailures_ShouldThrowAggregate()
    {
        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using var scope = new AssertionScope();
            (new[] { 1 }).Verify().HasCount(5);
            (new[] { 2 }).Verify().HasCount(5);
        });
        Xunit.Assert.Equal(2, ex.InnerExceptions.Count);
    }
}
