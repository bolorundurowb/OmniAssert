namespace OmniAssert.Tests;

public class CollectionSetAssertionTests
{
    // HaveCountBetween
    [Fact]
    public void HaveCountBetween_WhenWithinRange_ShouldSucceed() =>
        (new[] { 1, 2, 3 }).Must().HaveCountBetween(1, 5);

    [Fact]
    public void HaveCountBetween_WhenAtBounds_ShouldSucceed() =>
        (new[] { 1, 2, 3 }).Must().HaveCountBetween(3, 3);

    [Fact]
    public void HaveCountBetween_WhenBelow_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1 }).Must().HaveCountBetween(2, 4));

    [Fact]
    public void HaveCountBetween_WhenAbove_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2, 3, 4, 5 }).Must().HaveCountBetween(1, 4));

    // ContainOnly (allowed set)
    [Fact]
    public void ContainOnly_Set_WhenAllAllowed_ShouldSucceed() =>
        (new[] { 1, 2, 2, 3 }).Must().ContainOnly(new[] { 1, 2, 3, 4 });

    [Fact]
    public void ContainOnly_Set_WhenEmptyCollection_ShouldSucceed() =>
        (Array.Empty<int>()).Must().ContainOnly(new[] { 1, 2 });

    [Fact]
    public void ContainOnly_Set_WhenContainsDisallowed_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 9 }).Must().ContainOnly(new[] { 1, 2, 3 }));

    // ContainOnly (predicate)
    [Fact]
    public void ContainOnly_Predicate_WhenAllMatch_ShouldSucceed() =>
        (new[] { 2, 4, 6 }).Must().ContainOnly(x => x % 2 == 0);

    [Fact]
    public void ContainOnly_Predicate_WhenOneFails_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 2, 3, 6 }).Must().ContainOnly(x => x % 2 == 0));

    // BeSubsetOf
    [Fact]
    public void BeSubsetOf_WhenSubset_ShouldSucceed() =>
        (new[] { 1, 2 }).Must().BeSubsetOf(new[] { 1, 2, 3 });

    [Fact]
    public void BeSubsetOf_WhenEqualSets_ShouldSucceed() =>
        (new[] { 1, 2, 3 }).Must().BeSubsetOf(new[] { 3, 2, 1 });

    [Fact]
    public void BeSubsetOf_WhenNotSubset_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 4 }).Must().BeSubsetOf(new[] { 1, 2, 3 }));

    // BeSupersetOf
    [Fact]
    public void BeSupersetOf_WhenSuperset_ShouldSucceed() =>
        (new[] { 1, 2, 3, 4 }).Must().BeSupersetOf(new[] { 2, 3 });

    [Fact]
    public void BeSupersetOf_WhenMissingElement_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => (new[] { 1, 2 }).Must().BeSupersetOf(new[] { 2, 5 }));

    // Span parity: HaveCountBetween
    [Fact]
    public void Span_HaveCountBetween_WhenWithinRange_ShouldSucceed()
    {
        ReadOnlySpan<int> span = new[] { 1, 2, 3 };
        span.Must().HaveCountBetween(2, 4);
    }

    [Fact]
    public void Span_HaveCountBetween_WhenOutOfRange_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            ReadOnlySpan<int> span = new[] { 1, 2, 3 };
            span.Must().HaveCountBetween(4, 6);
        });
    }
}
