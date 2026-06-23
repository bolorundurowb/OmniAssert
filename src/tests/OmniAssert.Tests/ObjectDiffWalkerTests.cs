namespace OmniAssert.Tests;

public class ObjectDiffWalkerTests
{
    private sealed class Node
    {
        public string Name { get; set; } = "";
        public Node? Child { get; set; }
    }

    [Fact]
    public void Verify_NodeEquivalent_WhenObjectsEquivalent_ShouldReturnNull()
    {
        var a = new Node { Name = "a", Child = new Node { Name = "inner" } };
        var b = new Node { Name = "a", Child = new Node { Name = "inner" } };
        var diff = ObjectDiffWalker.Diff(a, b, "root");
        Xunit.Assert.Null(diff);
    }

    [Fact]
    public void Verify_NodeEquivalent_WhenNestedPropertyDiffers_ShouldIncludeBreadcrumbPath()
    {
        var a = new Node { Name = "x", Child = new Node { Name = "inner" } };
        var b = new Node { Name = "x", Child = new Node { Name = "other" } };
        var diff = ObjectDiffWalker.Diff(a, b, "root");
        Xunit.Assert.NotNull(diff);
        var msg = diff!.FormatMessage();
        Xunit.Assert.Contains("Child.Name", msg, StringComparison.Ordinal);
        Xunit.Assert.Contains("inner", msg, StringComparison.Ordinal);
        Xunit.Assert.Contains("other", msg, StringComparison.Ordinal);
    }

    [Fact]
    public void Verify_NodeEquivalent_WhenGraphsContainCycles_ShouldNotStackOverflow()
    {
        var a = new Node { Name = "loop" };
        a.Child = a;
        var b = new Node { Name = "loop" };
        b.Child = b;
        var diff = ObjectDiffWalker.Diff(a, b, "root");
        Xunit.Assert.Null(diff);
    }

    [Fact]
    public void Verify_ToBeEquivalentTo_WhenPropertyValuesDiffer_ShouldThrow()
    {
        var a = new Node { Name = "one" };
        var b = new Node { Name = "two" };
        Xunit.Assert.Throws<OmniAssertionException>(() => (a).Must().BeEquivalentTo(b));
    }

    [Fact]
    public void Verify_ToBeEquivalentTo_WhenTypesDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (new { A = 1 }).Must().BeEquivalentTo(new { B = 1 }));
    }

    [Fact]
    public void Verify_ToBeEquivalentTo_WhenActualIsNull_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ((object?)null).Must().BeEquivalentTo(new { A = 1 }));
    }

    [Fact]
    public void Verify_ToBeEquivalentTo_WhenCollectionElementsDiffer_ShouldThrow()
    {
        var a = new[] { 1, 2, 3 };
        var b = new[] { 1, 2, 4 };
        Xunit.Assert.Throws<OmniAssertionException>(() => (a).Must().BeEquivalentTo(b));
    }

    [Fact]
    public void Verify_ToBeEquivalentTo_WhenCollectionCountsDiffer_ShouldThrow()
    {
        var a = new[] { 1, 2 };
        var b = new[] { 1, 2, 3 };
        Xunit.Assert.Throws<OmniAssertionException>(() => (a).Must().BeEquivalentTo(b));
    }

    private sealed class WithList
    {
        public List<int> Items { get; set; } = [];
    }

    [Fact]
    public void Diff_WhenEnumerablePropertiesDiffer_ShouldDetectMismatch()
    {
        var a = new WithList { Items = [1, 2, 3] };
        var b = new WithList { Items = [1, 2, 4] };
        var diff = ObjectDiffWalker.Diff(a, b, "root");
        Xunit.Assert.NotNull(diff);
        var msg = diff!.FormatMessage();
        Xunit.Assert.Contains("Items[2]", msg, StringComparison.Ordinal);
    }

    [Fact]
    public void Diff_WhenEnumerablePropertiesAreEqual_ShouldReturnNull()
    {
        var a = new WithList { Items = [1, 2, 3] };
        var b = new WithList { Items = [1, 2, 3] };
        var diff = ObjectDiffWalker.Diff(a, b, "root");
        Xunit.Assert.Null(diff);
    }

    [Fact]
    public void Diff_WhenEnumerablePropertyActualIsShorter_ShouldDetectMismatch()
    {
        var a = new WithList { Items = [1, 2, 3] };
        var b = new WithList { Items = [1, 2] };
        var diff = ObjectDiffWalker.Diff(a, b, "root");
        Xunit.Assert.NotNull(diff);
        var msg = diff!.FormatMessage();
        Xunit.Assert.Contains("Items[2]", msg, StringComparison.Ordinal);
    }

    [Fact]
    public void Diff_WhenEnumerablePropertyActualIsLonger_ShouldDetectMismatch()
    {
        var a = new WithList { Items = [1] };
        var b = new WithList { Items = [1, 2] };
        var diff = ObjectDiffWalker.Diff(a, b, "root");
        Xunit.Assert.NotNull(diff);
        var msg = diff!.FormatMessage();
        Xunit.Assert.Contains("Items[1]", msg, StringComparison.Ordinal);
    }

    [Fact]
    public void Diff_WhenBothExpectedAndActualAreNull_ShouldReturnNull()
    {
        var diff = ObjectDiffWalker.Diff(null, null, "root");
        Xunit.Assert.Null(diff);
    }

    [Fact]
    public void Diff_WhenExpectedIsNullAndActualIsNotNull_ShouldDetectMismatch()
    {
        var diff = ObjectDiffWalker.Diff(null, new Node { Name = "x" }, "root");
        Xunit.Assert.NotNull(diff);
    }

    [Fact]
    public void Diff_WhenExpectedIsNotNullAndActualIsNull_ShouldDetectMismatch()
    {
        var diff = ObjectDiffWalker.Diff(new Node { Name = "x" }, null, "root");
        Xunit.Assert.NotNull(diff);
    }

    [Fact]
    public void Diff_WhenSameReference_ShouldReturnNull()
    {
        var a = new Node { Name = "x" };
        var diff = ObjectDiffWalker.Diff(a, a, "root");
        Xunit.Assert.Null(diff);
    }

    [Fact]
    public void Diff_WhenTypesDiffer_ShouldDetectMismatch()
    {
        var diff = ObjectDiffWalker.Diff(new Node { Name = "x" }, new { Name = "x" }, "root");
        Xunit.Assert.NotNull(diff);
    }

    private sealed class WithThrowingProperty
    {
        public string Name { get; set; } = "";
        public string Throwing => throw new InvalidOperationException("boom");
    }

    [Fact]
    public void Diff_WhenPropertyGetterThrows_ShouldDetectMismatch()
    {
        var a = new WithThrowingProperty { Name = "x" };
        var b = new WithThrowingProperty { Name = "x" };
        var diff = ObjectDiffWalker.Diff(a, b, "root");
        Xunit.Assert.NotNull(diff);
        var msg = diff!.FormatMessage();
        Xunit.Assert.Contains("Throwing", msg, StringComparison.Ordinal);
    }

    private sealed class WithNestedEnumerable
    {
        public List<WithList> Children { get; set; } = [];
    }

    [Fact]
    public void Diff_WhenNestedEnumerableElementsDiffer_ShouldDetectMismatch()
    {
        var a = new WithNestedEnumerable
        {
            Children =
            [
                new() { Items = [1, 2] },
                new() { Items = [3, 4] }
            ]
        };
        var b = new WithNestedEnumerable
        {
            Children =
            [
                new() { Items = [1, 2] },
                new() { Items = [3, 5] }
            ]
        };
        var diff = ObjectDiffWalker.Diff(a, b, "root");
        Xunit.Assert.NotNull(diff);
        var msg = diff!.FormatMessage();
        Xunit.Assert.Contains("Children[1].Items[1]", msg, StringComparison.Ordinal);
    }

    [Fact]
    public void Diff_WhenEmptyEnumerables_ShouldReturnNull()
    {
        var a = new WithList { Items = [] };
        var b = new WithList { Items = [] };
        var diff = ObjectDiffWalker.Diff(a, b, "root");
        Xunit.Assert.Null(diff);
    }

    [Fact]
    public void Diff_WhenLeafValuesDiffer_ShouldDetectMismatch()
    {
        var diff = ObjectDiffWalker.Diff(42, 99, "root");
        Xunit.Assert.NotNull(diff);
    }

    [Fact]
    public void Diff_WhenLeafValuesAreEqual_ShouldReturnNull()
    {
        var diff = ObjectDiffWalker.Diff(42, 42, "root");
        Xunit.Assert.Null(diff);
    }
}
