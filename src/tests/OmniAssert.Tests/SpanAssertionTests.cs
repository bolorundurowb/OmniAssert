namespace OmniAssert.Tests;

/// <summary>
/// Tests for <see cref="SpanAssertions{T}"/> via Span&lt;T&gt;, ReadOnlySpan&lt;T&gt;,
/// Memory&lt;T&gt;, and ReadOnlyMemory&lt;T&gt; Verify() entry points.
/// </summary>
/// <remarks>
/// Because <see cref="SpanAssertions{T}"/> is a ref struct it cannot be captured inside
/// lambda closures, so expected-failure tests use inline try/catch instead of
/// <c>Xunit.Assert.Throws</c>.
/// </remarks>
public class SpanAssertionTests
{
    [Fact]
    public void Verify_ReadOnlySpan_CanBeChained()
    {
        ReadOnlySpan<int> span = new int[] { 1, 2, 3 }.AsSpan();
        span.Must().HaveLength(3);
    }

    [Fact]
    public void Verify_Span_CanBeChained()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().HaveLength(3);
    }

    [Fact]
    public void Verify_Memory_CanBeChained()
    {
        new Memory<int>([1, 2, 3]).Must().HaveLength(3);
    }

    [Fact]
    public void Verify_ReadOnlyMemory_CanBeChained()
    {
        new ReadOnlyMemory<int>([1, 2, 3]).Must().HaveLength(3);
    }

    [Fact]
    public void ToEqual_WhenSequencesMatch_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().Equal(new int[] { 1, 2, 3 }.AsSpan());
    }

    [Fact]
    public void ToEqual_WhenSequencesDiffer_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 3 }.AsSpan().Must().Equal(new int[] { 1, 2, 4 }.AsSpan()));
    }

    [Fact]
    public void ToEqual_WhenLengthsDiffer_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 3 }.AsSpan().Must().Equal(new int[] { 1, 2 }.AsSpan()));
    }

    [Fact]
    public void ToEqual_Memory_WhenSequencesMatch_ShouldSucceed()
    {
        new ReadOnlyMemory<byte>([0x01, 0x02, 0x03]).Must()
            .Equal(new byte[] { 0x01, 0x02, 0x03 }.AsSpan());
    }

    [Fact]
    public void NotToEqual_WhenSequencesDiffer_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().NotEqual(new int[] { 4, 5, 6 }.AsSpan());
    }

    [Fact]
    public void NotToEqual_WhenSequencesMatch_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 3 }.AsSpan().Must().NotEqual(new int[] { 1, 2, 3 }.AsSpan()));
    }

    [Fact]
    public void ToBeEmpty_WhenEmpty_ShouldSucceed()
    {
        Array.Empty<int>().AsSpan().Must().BeEmpty();
    }

    [Fact]
    public void ToBeEmpty_WhenNotEmpty_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1 }.AsSpan().Must().BeEmpty());
    }

    [Fact]
    public void NotToBeEmpty_WhenNotEmpty_ShouldSucceed()
    {
        new int[] { 1, 2 }.AsSpan().Must().NotBeEmpty();
    }

    [Fact]
    public void NotToBeEmpty_WhenEmpty_ShouldThrow()
    {
        AssertSpanThrows(() => Array.Empty<int>().AsSpan().Must().NotBeEmpty());
    }

    [Fact]
    public void ToHaveLength_WhenCorrect_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().HaveLength(3);
    }

    [Fact]
    public void ToHaveLength_WhenIncorrect_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 3 }.AsSpan().Must().HaveLength(5));
    }

    [Fact]
    public void ToContain_WhenItemPresent_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().Contain(2);
    }

    [Fact]
    public void ToContain_WhenItemAbsent_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 3 }.AsSpan().Must().Contain(9));
    }

    [Fact]
    public void ToContain_ByteSpan_WhenItemPresent_ShouldSucceed()
    {
        new byte[] { 0xAA, 0xBB, 0xCC }.AsSpan().Must().Contain(0xBB);
    }

    [Fact]
    public void NotToContain_WhenItemAbsent_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().NotContain(9);
    }

    [Fact]
    public void NotToContain_WhenItemPresent_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 3 }.AsSpan().Must().NotContain(2));
    }

    [Fact]
    public void ToStartWith_WhenPrefixMatches_ShouldSucceed()
    {
        new int[] { 1, 2, 3, 4, 5 }.AsSpan().Must().StartWith(new int[] { 1, 2, 3 }.AsSpan());
    }

    [Fact]
    public void ToStartWith_WhenPrefixDoesNotMatch_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 3, 4, 5 }.AsSpan().Must().StartWith(new int[] { 2, 3 }.AsSpan()));
    }

    [Fact]
    public void ToStartWith_WhenPrefixLongerThanSpan_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2 }.AsSpan().Must().StartWith(new int[] { 1, 2, 3, 4 }.AsSpan()));
    }

    [Fact]
    public void ToStartWith_WhenFullSpanMatchesPrefix_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().StartWith(new int[] { 1, 2, 3 }.AsSpan());
    }

    [Fact]
    public void ToEndWith_WhenSuffixMatches_ShouldSucceed()
    {
        new int[] { 1, 2, 3, 4, 5 }.AsSpan().Must().EndWith(new int[] { 3, 4, 5 }.AsSpan());
    }

    [Fact]
    public void ToEndWith_WhenSuffixDoesNotMatch_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 3, 4, 5 }.AsSpan().Must().EndWith(new int[] { 1, 2 }.AsSpan()));
    }

    [Fact]
    public void ToEndWith_WhenSuffixLongerThanSpan_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2 }.AsSpan().Must().EndWith(new int[] { 0, 1, 2, 3 }.AsSpan()));
    }

    [Fact]
    public void HaveCount_AliasForHaveLength_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().HaveCount(3);
    }

    [Fact]
    public void HaveCountGreaterThan_WhenAboveMinimum_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().HaveCountGreaterThan(2);
    }

    [Fact]
    public void HaveCountGreaterThan_WhenAtOrBelowMinimum_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2 }.AsSpan().Must().HaveCountGreaterThan(2));
    }

    [Fact]
    public void HaveCountLessThan_WhenBelowMaximum_ShouldSucceed()
    {
        new int[] { 1, 2 }.AsSpan().Must().HaveCountLessThan(3);
    }

    [Fact]
    public void HaveCountLessThan_WhenAtOrAboveMaximum_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 3 }.AsSpan().Must().HaveCountLessThan(3));
    }

    [Fact]
    public void Contain_WithPredicate_WhenMatchExists_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().Contain(x => x > 2);
    }

    [Fact]
    public void Contain_WithPredicate_WhenNoMatch_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 3 }.AsSpan().Must().Contain(x => x > 10));
    }

    [Fact]
    public void NotContain_WithPredicate_WhenNoMatch_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().NotContain(x => x > 10);
    }

    [Fact]
    public void NotContain_WithPredicate_WhenMatchExists_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 3 }.AsSpan().Must().NotContain(x => x == 2));
    }

    [Fact]
    public void BeUnique_WhenAllUnique_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().BeUnique();
    }

    [Fact]
    public void BeUnique_WhenDuplicateExists_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 1 }.AsSpan().Must().BeUnique());
    }

    [Fact]
    public void HaveUniqueCount_WhenCorrect_ShouldSucceed()
    {
        new int[] { 1, 1, 2, 3 }.AsSpan().Must().HaveUniqueCount(3);
    }

    [Fact]
    public void HaveUniqueCount_WhenIncorrect_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 3 }.AsSpan().Must().HaveUniqueCount(2));
    }

    [Fact]
    public void BeInAscendingOrder_WhenSorted_ShouldSucceed()
    {
        new int[] { 1, 2, 2, 3 }.AsSpan().Must().BeInAscendingOrder();
    }

    [Fact]
    public void BeInAscendingOrder_WhenUnsorted_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 3, 2 }.AsSpan().Must().BeInAscendingOrder());
    }

    [Fact]
    public void BeInDescendingOrder_WhenSorted_ShouldSucceed()
    {
        new int[] { 3, 2, 1 }.AsSpan().Must().BeInDescendingOrder();
    }

    [Fact]
    public void BeInDescendingOrder_WhenUnsorted_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 3, 2 }.AsSpan().Must().BeInDescendingOrder());
    }

    [Fact]
    public void BeInDescendingOrder_WithKeySelector_WhenUnsorted_ShouldThrow()
    {
        var items = new[] { new { Age = 1 }, new { Age = 2 } };
        AssertSpanThrows(() => items.AsSpan().Must().BeInDescendingOrder(x => x.Age));
    }

    [Fact]
    public void BeInAscendingOrder_WithKeySelector_WhenSorted_ShouldSucceed()
    {
        var items = new[] { new { Age = 1 }, new { Age = 2 } };
        items.AsSpan().Must().BeInAscendingOrder(x => x.Age);
    }

    [Fact]
    public void BeInAscendingOrder_WithKeySelector_WhenUnsorted_ShouldThrow()
    {
        var items = new[] { new { Age = 2 }, new { Age = 1 } };
        AssertSpanThrows(() => items.AsSpan().Must().BeInAscendingOrder(x => x.Age));
    }

    [Fact]
    public void AllSatisfy_WhenAllMatch_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().AllSatisfy(x => x > 0);
    }

    [Fact]
    public void AllSatisfy_WhenOneFails_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, -1, 3 }.AsSpan().Must().AllSatisfy(x => x > 0));
    }

    [Fact]
    public void AnySatisfy_WhenOneMatches_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().AnySatisfy(x => x == 2);
    }

    [Fact]
    public void AnySatisfy_WhenNoneMatch_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 3 }.AsSpan().Must().AnySatisfy(x => x > 10));
    }

    [Fact]
    public void NoneSatisfy_WhenNoneMatch_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().NoneSatisfy(x => x > 10);
    }

    [Fact]
    public void NoneSatisfy_WhenOneMatches_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 3 }.AsSpan().Must().NoneSatisfy(x => x == 2));
    }

    [Fact]
    public void HaveCountMatching_WhenCountCorrect_ShouldSucceed()
    {
        new int[] { 1, 2, 3, 4, 5 }.AsSpan().Must().HaveCountMatching(3, x => x > 2);
    }

    [Fact]
    public void HaveCountMatching_WhenCountWrong_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 3, 4, 5 }.AsSpan().Must().HaveCountMatching(1, x => x > 2));
    }

    [Fact]
    public void BeEquivalentTo_SpanOverload_WhenMultisetMatches_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().BeEquivalentTo(new int[] { 3, 2, 1 }.AsSpan());
    }

    [Fact]
    public void BeEquivalentTo_SpanOverload_WhenLengthDiffers_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2 }.AsSpan().Must().BeEquivalentTo(new int[] { 1, 2, 3 }.AsSpan()));
    }

    [Fact]
    public void BeEquivalentTo_SpanOverload_WhenMultisetDiffers_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 1, 2 }.AsSpan().Must().BeEquivalentTo(new int[] { 1, 2, 2 }.AsSpan()));
    }

    [Fact]
    public void BeEquivalentTo_EnumerableOverload_WhenMultisetMatches_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().BeEquivalentTo([3, 2, 1]);
    }

    [Fact]
    public void BeEquivalentTo_EnumerableOverload_WhenMultisetDiffers_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 3 }.AsSpan().Must().BeEquivalentTo([1, 2, 4]));
    }

    [Fact]
    public void BeSequenceEqual_SpanOverload_WhenSequencesMatch_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().BeSequenceEqual(new int[] { 1, 2, 3 }.AsSpan());
    }

    [Fact]
    public void BeSequenceEqual_SpanOverload_WhenElementDiffers_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 3 }.AsSpan().Must().BeSequenceEqual(new int[] { 1, 2, 4 }.AsSpan()));
    }

    [Fact]
    public void BeSequenceEqual_EnumerableOverload_WhenSequencesMatch_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().BeSequenceEqual([1, 2, 3]);
    }

    [Fact]
    public void BeSequenceEqual_EnumerableOverload_WhenElementDiffers_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 3 }.AsSpan().Must().BeSequenceEqual([1, 2, 4]));
    }

    [Fact]
    public void ContainInOrder_SpanOverload_WhenSubsequencePresent_ShouldSucceed()
    {
        new int[] { 1, 2, 3, 4 }.AsSpan().Must().ContainInOrder(new int[] { 2, 4 }.AsSpan());
    }

    [Fact]
    public void ContainInOrder_SpanOverload_WhenSubsequenceMissing_ShouldThrow()
    {
        AssertSpanThrows(() => new int[] { 1, 2, 3 }.AsSpan().Must().ContainInOrder(new int[] { 3, 1 }.AsSpan()));
    }

    [Fact]
    public void ContainInOrder_EnumerableOverload_WhenSubsequencePresent_ShouldSucceed()
    {
        new int[] { 1, 2, 3, 4 }.AsSpan().Must().ContainInOrder([2, 4]);
    }

    [Fact]
    public void ContainInOrder_EmptyExpected_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().ContainInOrder((IEnumerable<int>)Array.Empty<int>());
    }

    private static void AssertSpanThrows(Action invoke)
    {
        var threw = false;
        try { invoke(); }
        catch (OmniAssertionException) { threw = true; }
        Xunit.Assert.True(threw);
    }
}
