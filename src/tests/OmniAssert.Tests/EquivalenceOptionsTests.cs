namespace OmniAssert.Tests;

public class EquivalenceOptionsTests
{
    private sealed class Person
    {
        public string Name { get; init; } = string.Empty;
        public List<string> Tags { get; init; } = new();
    }

    // Collection: case-insensitive multiset
    [Fact]
    public void Collection_BeEquivalentTo_IgnoreCase_WhenOnlyCaseDiffers_ShouldSucceed() =>
        (new[] { "Apple", "banana" }).Must()
            .BeEquivalentTo(new[] { "apple", "BANANA" }, new EquivalenceOptions { IgnoreCase = true });

    [Fact]
    public void Collection_BeEquivalentTo_IgnoreCase_OrderIndependent_ShouldSucceed() =>
        (new[] { "a", "B" }).Must()
            .BeEquivalentTo(new[] { "b", "A" }, new EquivalenceOptions { IgnoreCase = true });

    [Fact]
    public void Collection_BeEquivalentTo_IgnoreCase_WhenContentDiffers_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { "apple" }).Must()
                .BeEquivalentTo(new[] { "orange" }, new EquivalenceOptions { IgnoreCase = true }));

    [Fact]
    public void Collection_BeEquivalentTo_WithoutIgnoreCase_WhenCaseDiffers_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new[] { "Apple" }).Must()
                .BeEquivalentTo(new[] { "apple" }, EquivalenceOptions.Default));

    // Object: case-insensitive leaf
    [Fact]
    public void Object_BeEquivalentTo_IgnoreCase_WhenNameCaseDiffers_ShouldSucceed()
    {
        var actual = new Person { Name = "alice", Tags = new() { "x" } };
        var expected = new Person { Name = "ALICE", Tags = new() { "x" } };
        ((object)actual).Must().BeEquivalentTo(expected, new EquivalenceOptions { IgnoreCase = true });
    }

    [Fact]
    public void Object_BeEquivalentTo_WithoutIgnoreCase_WhenNameCaseDiffers_ShouldThrow()
    {
        var actual = new Person { Name = "alice" };
        var expected = new Person { Name = "ALICE" };
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            ((object)actual).Must().BeEquivalentTo(expected));
    }

    // Object: order-insensitive nested collection
    [Fact]
    public void Object_BeEquivalentTo_IgnoreCollectionOrder_WhenTagsReordered_ShouldSucceed()
    {
        var actual = new Person { Name = "bob", Tags = new() { "a", "b", "c" } };
        var expected = new Person { Name = "bob", Tags = new() { "c", "a", "b" } };
        ((object)actual).Must()
            .BeEquivalentTo(expected, new EquivalenceOptions { IgnoreCollectionOrder = true });
    }

    [Fact]
    public void Object_BeEquivalentTo_WithoutIgnoreOrder_WhenTagsReordered_ShouldThrow()
    {
        var actual = new Person { Name = "bob", Tags = new() { "a", "b", "c" } };
        var expected = new Person { Name = "bob", Tags = new() { "c", "a", "b" } };
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            ((object)actual).Must().BeEquivalentTo(expected));
    }

    [Fact]
    public void Object_BeEquivalentTo_IgnoreCollectionOrder_WhenTagsDiffer_ShouldThrow()
    {
        var actual = new Person { Name = "bob", Tags = new() { "a", "b" } };
        var expected = new Person { Name = "bob", Tags = new() { "a", "z" } };
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            ((object)actual).Must()
                .BeEquivalentTo(expected, new EquivalenceOptions { IgnoreCollectionOrder = true }));
    }

    // Combined case + order
    [Fact]
    public void Object_BeEquivalentTo_IgnoreCaseAndOrder_ShouldSucceed()
    {
        var actual = new Person { Name = "carol", Tags = new() { "Red", "green" } };
        var expected = new Person { Name = "CAROL", Tags = new() { "GREEN", "RED" } };
        ((object)actual).Must()
            .BeEquivalentTo(expected, new EquivalenceOptions { IgnoreCase = true, IgnoreCollectionOrder = true });
    }
}
