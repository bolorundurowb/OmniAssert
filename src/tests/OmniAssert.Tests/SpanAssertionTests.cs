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
        var threw = false;
        try { new int[] { 1, 2, 3 }.AsSpan().Must().Equal(new int[] { 1, 2, 4 }.AsSpan()); }
        catch (OmniAssertionException) { threw = true; }
        Xunit.Assert.True(threw);
    }

    [Fact]
    public void ToEqual_WhenLengthsDiffer_ShouldThrow()
    {
        var threw = false;
        try { new int[] { 1, 2, 3 }.AsSpan().Must().Equal(new int[] { 1, 2 }.AsSpan()); }
        catch (OmniAssertionException) { threw = true; }
        Xunit.Assert.True(threw);
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
        var threw = false;
        try { new int[] { 1, 2, 3 }.AsSpan().Must().NotEqual(new int[] { 1, 2, 3 }.AsSpan()); }
        catch (OmniAssertionException) { threw = true; }
        Xunit.Assert.True(threw);
    }

    [Fact]
    public void ToBeEmpty_WhenEmpty_ShouldSucceed()
    {
        Array.Empty<int>().AsSpan().Must().BeEmpty();
    }

    [Fact]
    public void ToBeEmpty_WhenNotEmpty_ShouldThrow()
    {
        var threw = false;
        try { new int[] { 1 }.AsSpan().Must().BeEmpty(); }
        catch (OmniAssertionException) { threw = true; }
        Xunit.Assert.True(threw);
    }

    [Fact]
    public void NotToBeEmpty_WhenNotEmpty_ShouldSucceed()
    {
        new int[] { 1, 2 }.AsSpan().Must().NotBeEmpty();
    }

    [Fact]
    public void NotToBeEmpty_WhenEmpty_ShouldThrow()
    {
        var threw = false;
        try { Array.Empty<int>().AsSpan().Must().NotBeEmpty(); }
        catch (OmniAssertionException) { threw = true; }
        Xunit.Assert.True(threw);
    }

    [Fact]
    public void ToHaveLength_WhenCorrect_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().HaveLength(3);
    }

    [Fact]
    public void ToHaveLength_WhenIncorrect_ShouldThrow()
    {
        var threw = false;
        try { new int[] { 1, 2, 3 }.AsSpan().Must().HaveLength(5); }
        catch (OmniAssertionException) { threw = true; }
        Xunit.Assert.True(threw);
    }

    [Fact]
    public void ToContain_WhenItemPresent_ShouldSucceed()
    {
        new int[] { 1, 2, 3 }.AsSpan().Must().Contain(2);
    }

    [Fact]
    public void ToContain_WhenItemAbsent_ShouldThrow()
    {
        var threw = false;
        try { new int[] { 1, 2, 3 }.AsSpan().Must().Contain(9); }
        catch (OmniAssertionException) { threw = true; }
        Xunit.Assert.True(threw);
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
        var threw = false;
        try { new int[] { 1, 2, 3 }.AsSpan().Must().NotContain(2); }
        catch (OmniAssertionException) { threw = true; }
        Xunit.Assert.True(threw);
    }

    [Fact]
    public void ToStartWith_WhenPrefixMatches_ShouldSucceed()
    {
        new int[] { 1, 2, 3, 4, 5 }.AsSpan().Must().StartWith(new int[] { 1, 2, 3 }.AsSpan());
    }

    [Fact]
    public void ToStartWith_WhenPrefixDoesNotMatch_ShouldThrow()
    {
        var threw = false;
        try { new int[] { 1, 2, 3, 4, 5 }.AsSpan().Must().StartWith(new int[] { 2, 3 }.AsSpan()); }
        catch (OmniAssertionException) { threw = true; }
        Xunit.Assert.True(threw);
    }

    [Fact]
    public void ToStartWith_WhenPrefixLongerThanSpan_ShouldThrow()
    {
        var threw = false;
        try { new int[] { 1, 2 }.AsSpan().Must().StartWith(new int[] { 1, 2, 3, 4 }.AsSpan()); }
        catch (OmniAssertionException) { threw = true; }
        Xunit.Assert.True(threw);
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
        var threw = false;
        try { new int[] { 1, 2, 3, 4, 5 }.AsSpan().Must().EndWith(new int[] { 1, 2 }.AsSpan()); }
        catch (OmniAssertionException) { threw = true; }
        Xunit.Assert.True(threw);
    }

    [Fact]
    public void ToEndWith_WhenSuffixLongerThanSpan_ShouldThrow()
    {
        var threw = false;
        try { new int[] { 1, 2 }.AsSpan().Must().EndWith(new int[] { 0, 1, 2, 3 }.AsSpan()); }
        catch (OmniAssertionException) { threw = true; }
        Xunit.Assert.True(threw);
    }

}
