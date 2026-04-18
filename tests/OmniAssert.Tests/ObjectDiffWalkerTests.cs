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
    public void Diff_WhenObjectsEquivalent_ShouldReturnNull()
    {
        var a = new Node { Name = "a", Child = new Node { Name = "inner" } };
        var b = new Node { Name = "a", Child = new Node { Name = "inner" } };
        var diff = ObjectDiffWalker.Diff(a, b, "root");
        Xunit.Assert.Null(diff);
    }

    [Fact]
    public void Diff_WhenNestedPropertyDiffers_ShouldIncludeBreadcrumbPath()
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
    public void Diff_WhenGraphsContainCycles_ShouldNotStackOverflow()
    {
        var a = new Node { Name = "loop" };
        a.Child = a;
        var b = new Node { Name = "loop" };
        b.Child = b;
        var diff = ObjectDiffWalker.Diff(a, b, "root");
        Xunit.Assert.Null(diff);
    }

    [Fact]
    public void VerifyEquivalent_WhenPropertyValuesDiffer_ShouldThrowOmniAssertionException()
    {
        var a = new Node { Name = "one" };
        var b = new Node { Name = "two" };
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(a).ToBeEquivalentTo(b));
    }
}
