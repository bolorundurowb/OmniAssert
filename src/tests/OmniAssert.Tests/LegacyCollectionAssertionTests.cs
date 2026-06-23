namespace OmniAssert.Tests;

#pragma warning disable CS0618
#pragma warning disable OA002
#pragma warning disable OA003

/// <summary>Covers obsolete To* collection wrappers until removal in v3.</summary>
public class LegacyCollectionAssertionTests
{
    [Fact]
    public void Legacy_ToBe_delegates_to_Be()
    {
        IEnumerable<int> arr = new[] { 1, 2, 3 };
        arr.Verify().ToBe(arr);
    }

    [Fact]
    public void Legacy_ToContain_delegates_to_Contain()
    {
        (new[] { 1, 2, 3 }).Verify().ToContain(2);
    }

    [Fact]
    public void Legacy_ToBeEmpty_delegates_to_BeEmpty()
    {
        Array.Empty<int>().Verify().ToBeEmpty();
    }

    [Fact]
    public void Legacy_NotToBeEmpty_delegates_to_NotBeEmpty()
    {
        (new[] { 1 }).Verify().NotToBeEmpty();
    }

    [Fact]
    public void Legacy_NotToContain_delegates_to_NotContain()
    {
        (new[] { 1, 2, 3 }).Verify().NotToContain(9);
    }

    [Fact]
    public void Legacy_ToHaveCount_delegates_to_HaveCount()
    {
        (new[] { 1, 2, 3 }).Verify().ToHaveCount(3);
    }

    [Fact]
    public void Legacy_ToHaveCountGreaterThan_delegates_to_HaveCountGreaterThan()
    {
        (new[] { 1, 2, 3 }).Verify().ToHaveCountGreaterThan(2);
    }

    [Fact]
    public void Legacy_ToHaveCountLessThan_delegates_to_HaveCountLessThan()
    {
        (new[] { 1, 2 }).Verify().ToHaveCountLessThan(3);
    }

    [Fact]
    public void Legacy_ToBeUnique_delegates_to_BeUnique()
    {
        (new[] { 1, 2, 3 }).Verify().ToBeUnique();
    }

    [Fact]
    public void Legacy_ToHaveUniqueCount_delegates_to_HaveUniqueCount()
    {
        (new[] { 1, 1, 2 }).Verify().ToHaveUniqueCount(2);
    }

    [Fact]
    public void Legacy_ToBeInAscendingOrder_delegates_to_BeInAscendingOrder()
    {
        (new[] { 1, 2, 3 }).Verify().ToBeInAscendingOrder();
    }

    [Fact]
    public void Legacy_ToBeInDescendingOrder_delegates_to_BeInDescendingOrder()
    {
        (new[] { 3, 2, 1 }).Verify().ToBeInDescendingOrder();
    }

    [Fact]
    public void Legacy_ToHaveCountMatching_delegates_to_HaveCountMatching()
    {
        (new[] { 1, 2, 3, 4 }).Verify().ToHaveCountMatching(2, x => x > 2);
    }

    [Fact]
    public void Legacy_ToBeEquivalentTo_delegates_to_BeEquivalentTo()
    {
        (new[] { 1, 2, 3 }).Verify().ToBeEquivalentTo(new[] { 3, 2, 1 });
    }

    [Fact]
    public void Legacy_ToBeSequenceEqual_delegates_to_BeSequenceEqual()
    {
        (new[] { 1, 2, 3 }).Verify().ToBeSequenceEqual(new[] { 1, 2, 3 });
    }

    [Fact]
    public void Legacy_ToContainInOrder_delegates_to_ContainInOrder()
    {
        (new[] { 1, 2, 3, 4 }).Verify().ToContainInOrder(new[] { 2, 4 });
    }
}

#pragma warning restore OA003
#pragma warning restore OA002
#pragma warning restore CS0618
