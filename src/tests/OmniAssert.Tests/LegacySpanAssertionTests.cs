namespace OmniAssert.Tests;

#pragma warning disable CS0618
#pragma warning disable OA003

/// <summary>Covers obsolete To* span wrappers until removal in v3.</summary>
public class LegacySpanAssertionTests
{
    [Fact]
    public void Legacy_ToStar_wrappers_delegate_to_modern_methods()
    {
        var span = new int[] { 1, 2, 3, 4, 5 }.AsSpan();
        var prefix = new int[] { 1, 2 }.AsSpan();
        var suffix = new int[] { 4, 5 }.AsSpan();

        span.Must().ToEqual(span);
        span.Must().NotToEqual(new int[] { 9 }.AsSpan());
        Array.Empty<int>().AsSpan().Must().ToBeEmpty();
        span.Must().NotToBeEmpty();
        span.Must().ToHaveLength(5);
        span.Must().ToHaveCount(5);
        span.Must().ToHaveCountGreaterThan(4);
        span.Must().ToHaveCountLessThan(6);
        span.Must().ToContain(3);
        span.Must().NotToContain(9);
        span.Must().ToStartWith(prefix);
        span.Must().ToEndWith(suffix);
        span.Must().ToBeUnique();
        span.Must().ToHaveUniqueCount(5);
        span.Must().ToBeInAscendingOrder();
        new int[] { 5, 4, 3 }.AsSpan().Must().ToBeInDescendingOrder();
        span.Must().ToHaveCountMatching(3, x => x > 2);
        span.Must().ToBeEquivalentTo(new int[] { 5, 4, 3, 2, 1 }.AsSpan());
        span.Must().ToBeSequenceEqual(new int[] { 1, 2, 3, 4, 5 }.AsSpan());
        span.Must().ToContainInOrder(new int[] { 2, 4 }.AsSpan());
    }
}

#pragma warning restore OA003
#pragma warning restore CS0618
