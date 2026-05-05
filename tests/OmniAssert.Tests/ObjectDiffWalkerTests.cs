using static OmniAssert.Assert;

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
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(a).ToBeEquivalentTo(b));
    }

    [Fact]
    public void Verify_ToBeEquivalentTo_WhenTypesDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(new { A = 1 }).ToBeEquivalentTo(new { B = 1 }));
    }

    [Fact]
    public void Verify_ToBeEquivalentTo_WhenActualIsNull_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify((object?)null).ToBeEquivalentTo(new { A = 1 }));
    }

    [Fact]
    public void Verify_ToBeEquivalentTo_WhenCollectionElementsDiffer_ShouldThrow()
    {
        var a = new[] { 1, 2, 3 };
        var b = new[] { 1, 2, 4 };
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(a).ToBeEquivalentTo(b));
    }

    [Fact]
    public void Verify_ToBeEquivalentTo_WhenCollectionCountsDiffer_ShouldThrow()
    {
        var a = new[] { 1, 2 };
        var b = new[] { 1, 2, 3 };
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(a).ToBeEquivalentTo(b));
    }
}
