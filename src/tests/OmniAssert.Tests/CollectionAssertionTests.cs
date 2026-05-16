using OmniAssert;
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
