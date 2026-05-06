using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class ObjectAssertionTests
{
    private class Animal { }
    private class Dog : Animal { }

    // ── ToBeOfType ───────────────────────────────────────────────────────────

    [Fact]
    public void ToBeOfType_WhenExactTypeMatches_ShouldSucceed()
    {
        object obj = "hello";
        Verify(obj).ToBeOfType<string>();
    }

    [Fact]
    public void ToBeOfType_WhenTypeDiffers_ShouldThrow()
    {
        object obj = 42;
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(obj).ToBeOfType<string>());
    }

    [Fact]
    public void ToBeOfType_WhenSubtypeProvided_ShouldThrow()
    {
        object obj = new Dog();
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(obj).ToBeOfType<Animal>());
    }

    [Fact]
    public void ToBeOfType_WithinScope_WhenTypeDiffers_ShouldCollectRatherThanThrow()
    {
        object obj = 42;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            Verify(obj).ToBeOfType<string>();
        });
        Xunit.Assert.NotNull(ex);
    }

    // ── ToBeAssignableTo ─────────────────────────────────────────────────────

    [Fact]
    public void ToBeAssignableTo_WhenExactTypeMatches_ShouldSucceed()
    {
        object obj = "hello";
        Verify(obj).ToBeAssignableTo<string>();
    }

    [Fact]
    public void ToBeAssignableTo_WhenSubtypeMatches_ShouldSucceed()
    {
        object obj = new Dog();
        Verify(obj).ToBeAssignableTo<Animal>();
    }

    [Fact]
    public void ToBeAssignableTo_WhenInterfaceMatches_ShouldSucceed()
    {
        object obj = "hello";
        Verify(obj).ToBeAssignableTo<IEnumerable<char>>();
    }

    [Fact]
    public void ToBeAssignableTo_WhenNotAssignable_ShouldThrow()
    {
        object obj = 42;
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(obj).ToBeAssignableTo<IEnumerable<int>>());
    }

    [Fact]
    public void ToBeAssignableTo_WithinScope_WhenNotAssignable_ShouldCollectRatherThanThrow()
    {
        object obj = 42;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            Verify(obj).ToBeAssignableTo<string>();
        });
        Xunit.Assert.NotNull(ex);
    }

    // ── ToBeEquivalentTo ─────────────────────────────────────────────────────

    [Fact]
    public void ToBeEquivalentTo_WhenObjectsEquivalent_ShouldSucceed()
    {
        var a = new { Name = "Alice", Age = 30 };
        var b = new { Name = "Alice", Age = 30 };
        Verify((object)a).ToBeEquivalentTo(b);
    }

    [Fact]
    public void ToBeEquivalentTo_WhenPropertyValuesDiffer_ShouldThrow()
    {
        var a = new { Name = "Alice" };
        var b = new { Name = "Bob" };
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify((object)a).ToBeEquivalentTo(b));
    }

    [Fact]
    public void ToBeEquivalentTo_WhenActualIsNull_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify((object?)null).ToBeEquivalentTo(new { A = 1 }));
    }

    [Fact]
    public void ToBeEquivalentTo_WhenExpectedIsNull_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify((object)new { A = 1 }).ToBeEquivalentTo(null));
    }

    [Fact]
    public void ToBeEquivalentTo_WithinScope_WhenValuesDiffer_ShouldCollectRatherThanThrow()
    {
        var a = new { Name = "Alice" };
        var b = new { Name = "Bob" };
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            Verify((object)a).ToBeEquivalentTo(b);
        });
        Xunit.Assert.NotNull(ex);
    }
}
