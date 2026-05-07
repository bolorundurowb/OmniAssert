using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class CollectionAssertionTests
{
    // ── ToContain ────────────────────────────────────────────────────────────

    [Fact]
    public void ToContain_WhenItemPresent_ShouldSucceed()
    {
        Verify(new[] { 1, 2, 3 }).ToContain(2);
    }

    [Fact]
    public void ToContain_WhenItemAbsent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(new[] { 1, 2, 3 }).ToContain(4));
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

        Verify(Yield123()).ToContain(2);
    }

    // ── NotToContain ─────────────────────────────────────────────────────────

    [Fact]
    public void NotToContain_WhenItemAbsent_ShouldSucceed()
    {
        Verify(new[] { 1, 2, 3 }).NotToContain(4);
    }

    [Fact]
    public void NotToContain_WhenItemPresent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(new[] { 1, 2, 3 }).NotToContain(2));
    }

    // ── ToBeEmpty / NotToBeEmpty ─────────────────────────────────────────────

    [Fact]
    public void ToBeEmpty_WhenEmpty_ShouldSucceed()
    {
        Verify(Array.Empty<int>()).ToBeEmpty();
    }

    [Fact]
    public void ToBeEmpty_WhenNotEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(new[] { 1 }).ToBeEmpty());
    }

    [Fact]
    public void ToBeEmpty_NonICollection_WhenEmpty_ShouldSucceed()
    {
        static IEnumerable<int> YieldNone()
        {
            yield break;
        }

        Verify(YieldNone()).ToBeEmpty();
    }

    [Fact]
    public void NotToBeEmpty_WhenNotEmpty_ShouldSucceed()
    {
        Verify(new[] { 1, 2, 3 }).NotToBeEmpty();
    }

    [Fact]
    public void NotToBeEmpty_WhenEmpty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(Array.Empty<int>()).NotToBeEmpty());
    }

    // ── HasCount ─────────────────────────────────────────────────────────────

    [Fact]
    public void HasCount_WhenCountMatches_ShouldSucceed()
    {
        Verify(new[] { 1, 2, 3 }).HasCount(3);
    }

    [Fact]
    public void HasCount_WhenCountDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(new[] { 1, 2, 3 }).HasCount(2));
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

        Verify(YieldThree()).HasCount(3);
    }

    [Fact]
    public void ToHaveCount_WhenCountMatches_ShouldSucceed()
    {
        Verify(new[] { 1, 2, 3 }).ToHaveCount(3);
    }

    [Fact]
    public void ToHaveCount_WhenCountDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(new[] { 1, 2, 3 }).ToHaveCount(2));
    }

    // ── HasCountGreaterThan ──────────────────────────────────────────────────

    [Fact]
    public void HasCountGreaterThan_WhenCountIsGreater_ShouldSucceed()
    {
        Verify(new[] { 1, 2, 3 }).HasCountGreaterThan(2);
    }

    [Fact]
    public void HasCountGreaterThan_WhenCountIsEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(new[] { 1, 2 }).HasCountGreaterThan(2));
    }

    [Fact]
    public void HasCountGreaterThan_WhenCountIsLess_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(new[] { 1 }).HasCountGreaterThan(2));
    }

    // ── HasCountLessThan ─────────────────────────────────────────────────────

    [Fact]
    public void HasCountLessThan_WhenCountIsLess_ShouldSucceed()
    {
        Verify(new[] { 1, 2, 3 }).HasCountLessThan(4);
    }

    [Fact]
    public void HasCountLessThan_WhenCountIsEqual_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(new[] { 1, 2 }).HasCountLessThan(2));
    }

    // ── ToBeUnique ───────────────────────────────────────────────────────────

    [Fact]
    public void ToBeUnique_WhenAllElementsUnique_ShouldSucceed()
    {
        Verify(new[] { 1, 2, 3 }).ToBeUnique();
    }

    [Fact]
    public void ToBeUnique_WhenDuplicatesExist_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(new[] { 1, 2, 1 }).ToBeUnique());
    }

    // ── HasUniqueCount ───────────────────────────────────────────────────────

    [Fact]
    public void HasUniqueCount_WhenCorrect_ShouldSucceed()
    {
        Verify(new[] { 1, 1, 2, 3 }).HasUniqueCount(3);
    }

    [Fact]
    public void HasUniqueCount_WhenCountDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(new[] { 1, 2, 2, 3 }).HasUniqueCount(4));
    }

    // ── Ordering ─────────────────────────────────────────────────────────────

    [Fact]
    public void ToBeInAscendingOrder_WhenSorted_ShouldSucceed()
    {
        Verify(new[] { 1, 2, 2, 3 }).ToBeInAscendingOrder();
    }

    [Fact]
    public void ToBeInAscendingOrder_WhenUnsorted_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(new[] { 1, 3, 2 }).ToBeInAscendingOrder());
    }

    [Fact]
    public void ToBeInDescendingOrder_WhenSorted_ShouldSucceed()
    {
        Verify(new[] { 3, 2, 2, 1 }).ToBeInDescendingOrder();
    }

    [Fact]
    public void ToBeInDescendingOrder_WhenUnsorted_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(new[] { 3, 1, 2 }).ToBeInDescendingOrder());
    }

    // ── AllSatisfy ───────────────────────────────────────────────────────────

    [Fact]
    public void AllSatisfy_WhenAllElementsSatisfy_ShouldSucceed()
    {
        Verify(new[] { 1, 2, 3 }).AllSatisfy(x => x > 0);
    }

    [Fact]
    public void AllSatisfy_WhenOneElementFails_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(new[] { 1, 2, -1 }).AllSatisfy(x => x > 0));
    }

    // ── ToBeEquivalentTo ─────────────────────────────────────────────────────

    [Fact]
    public void ToBeEquivalentTo_WhenOrderDiffers_ShouldSucceed()
    {
        Verify(new[] { 1, 2, 3 }).ToBeEquivalentTo(new[] { 3, 2, 1 });
    }

    [Fact]
    public void ToBeEquivalentTo_WhenElementsDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(new[] { 1, 2, 3 }).ToBeEquivalentTo(new[] { 1, 2, 4 }));
    }

    [Fact]
    public void ToBeEquivalentTo_WhenCountDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new[] { 1, 2 }).ToBeEquivalentTo(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void ToBeEquivalentTo_WhenMultisetDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new[] { 1, 1, 2 }).ToBeEquivalentTo(new[] { 1, 2, 2 }));
    }

    // ── Scope ────────────────────────────────────────────────────────────────

    [Fact]
    public void ToContain_WithinScope_WhenItemAbsent_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            Verify(new[] { 1, 2 }).ToContain(9);
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void HasCount_WithinScope_MultipleFailures_ShouldThrowAggregate()
    {
        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using var scope = new AssertionScope();
            Verify(new[] { 1 }).HasCount(5);
            Verify(new[] { 2 }).HasCount(5);
        });
        Xunit.Assert.Equal(2, ex.InnerExceptions.Count);
    }
}
