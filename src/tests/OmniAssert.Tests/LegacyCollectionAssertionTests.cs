namespace OmniAssert.Tests;

#pragma warning disable CS0618
#pragma warning disable OA002
#pragma warning disable OA003

/// <summary>Covers obsolete To* collection wrappers until removal in v3.</summary>
public class LegacyCollectionAssertionTests
{
    [Fact]
    public void Legacy_ToStar_wrappers_delegate_to_modern_methods()
    {
        IEnumerable<int> arr = new[] { 1, 2, 3 };
        arr.Verify().ToBe(arr);
        arr.Verify().ToContain(2);
        Array.Empty<int>().Verify().ToBeEmpty();
        (new[] { 1 }).Verify().NotToBeEmpty();
        (new[] { 1, 2, 3 }).Verify().NotToContain(9);
        (new[] { 1, 2, 3 }).Verify().ToHaveCount(3);
        (new[] { 1, 2, 3 }).Verify().ToHaveCountGreaterThan(2);
        (new[] { 1, 2 }).Verify().ToHaveCountLessThan(3);
        (new[] { 1, 2, 3 }).Verify().ToBeUnique();
        (new[] { 1, 1, 2 }).Verify().ToHaveUniqueCount(2);
        (new[] { 1, 2, 3 }).Verify().ToBeInAscendingOrder();
        (new[] { 3, 2, 1 }).Verify().ToBeInDescendingOrder();
        (new[] { 1, 2, 3, 4 }).Verify().ToHaveCountMatching(2, x => x > 2);
        (new[] { 1, 2, 3 }).Verify().ToBeEquivalentTo(new[] { 3, 2, 1 });
        (new[] { 1, 2, 3 }).Verify().ToBeSequenceEqual(new[] { 1, 2, 3 });
        (new[] { 1, 2, 3, 4 }).Verify().ToContainInOrder(new[] { 2, 4 });
    }
}

#pragma warning restore OA003
#pragma warning restore OA002
#pragma warning restore CS0618
