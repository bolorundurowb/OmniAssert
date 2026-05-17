namespace OmniAssert.Tests;

public class ObjectAssertionTests
{
    private class Animal { }
    private class Dog : Animal { }

    [Fact]
    public void ToBeNull_WhenObjectIsNull_ShouldSucceed()
    {
        ((object?)null).Verify().ToBeNull();
    }

    [Fact]
    public void ToBeNull_WhenObjectIsNotNull_ShouldThrow()
    {
        object obj = "hello";
        Xunit.Assert.Throws<OmniAssertionException>(() => obj.Verify().ToBeNull());
    }

    [Fact]
    public void ToBeNull_WithinScope_WhenObjectIsNotNull_ShouldCollectRatherThanThrow()
    {
        object obj = "hello";
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            obj.Verify().ToBeNull();
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void NotToBeNull_WhenObjectIsNotNull_ShouldSucceed()
    {
        object obj = "hello";
        obj.Verify().NotToBeNull();
    }

    [Fact]
    public void NotToBeNull_WhenObjectIsNull_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ((object?)null).Verify().NotToBeNull());
    }

    [Fact]
    public void NotToBeNull_WithinScope_WhenObjectIsNull_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            ((object?)null).Verify().NotToBeNull();
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void ToBeOfType_WhenExactTypeMatches_ShouldSucceed()
    {
        object obj = "hello";
        (obj).Verify().ToBeOfType<string>();
    }

    [Fact]
    public void ToBeOfType_WhenTypeDiffers_ShouldThrow()
    {
        object obj = 42;
        Xunit.Assert.Throws<OmniAssertionException>(() => (obj).Verify().ToBeOfType<string>());
    }

    [Fact]
    public void ToBeOfType_WhenSubtypeProvided_ShouldThrow()
    {
        object obj = new Dog();
        Xunit.Assert.Throws<OmniAssertionException>(() => (obj).Verify().ToBeOfType<Animal>());
    }

    [Fact]
    public void ToBeOfType_WithinScope_WhenTypeDiffers_ShouldCollectRatherThanThrow()
    {
        object obj = 42;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (obj).Verify().ToBeOfType<string>();
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void ToBeAssignableTo_WhenExactTypeMatches_ShouldSucceed()
    {
        object obj = "hello";
        (obj).Verify().ToBeAssignableTo<string>();
    }

    [Fact]
    public void ToBeAssignableTo_WhenSubtypeMatches_ShouldSucceed()
    {
        object obj = new Dog();
        (obj).Verify().ToBeAssignableTo<Animal>();
    }

    [Fact]
    public void ToBeAssignableTo_WhenInterfaceMatches_ShouldSucceed()
    {
        object obj = "hello";
        (obj).Verify().ToBeAssignableTo<IEnumerable<char>>();
    }

    [Fact]
    public void ToBeAssignableTo_WhenNotAssignable_ShouldThrow()
    {
        object obj = 42;
        Xunit.Assert.Throws<OmniAssertionException>(() => (obj).Verify().ToBeAssignableTo<IEnumerable<int>>());
    }

    [Fact]
    public void ToBeAssignableTo_WithinScope_WhenNotAssignable_ShouldCollectRatherThanThrow()
    {
        object obj = 42;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (obj).Verify().ToBeAssignableTo<string>();
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void NotToBeOfType_WhenTypeDiffers_ShouldSucceed()
    {
        object obj = 42;
        obj.Verify().NotToBeOfType<string>();
    }

    [Fact]
    public void NotToBeOfType_WhenExactTypeMatches_ShouldThrow()
    {
        object obj = "hello";
        Xunit.Assert.Throws<OmniAssertionException>(() => obj.Verify().NotToBeOfType<string>());
    }

    [Fact]
    public void NotToBeOfType_WhenSubtypeProvided_ShouldSucceed()
    {
        object obj = new Dog();
        obj.Verify().NotToBeOfType<Animal>();
    }

    [Fact]
    public void NotToBeOfType_WithinScope_WhenExactTypeMatches_ShouldCollectRatherThanThrow()
    {
        object obj = "hello";
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            obj.Verify().NotToBeOfType<string>();
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void NotToBeAssignableTo_WhenNotAssignable_ShouldSucceed()
    {
        object obj = 42;
        obj.Verify().NotToBeAssignableTo<string>();
    }

    [Fact]
    public void NotToBeAssignableTo_WhenExactTypeMatches_ShouldThrow()
    {
        object obj = "hello";
        Xunit.Assert.Throws<OmniAssertionException>(() => obj.Verify().NotToBeAssignableTo<string>());
    }

    [Fact]
    public void NotToBeAssignableTo_WhenSubtypeMatches_ShouldThrow()
    {
        object obj = new Dog();
        Xunit.Assert.Throws<OmniAssertionException>(() => obj.Verify().NotToBeAssignableTo<Animal>());
    }

    [Fact]
    public void NotToBeAssignableTo_WithinScope_WhenAssignable_ShouldCollectRatherThanThrow()
    {
        object obj = "hello";
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            obj.Verify().NotToBeAssignableTo<string>();
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void ToBeEquivalentTo_WhenObjectsEquivalent_ShouldSucceed()
    {
        var a = new { Name = "Alice", Age = 30 };
        var b = new { Name = "Alice", Age = 30 };
        ((object)a).Verify().ToBeEquivalentTo(b);
    }

    [Fact]
    public void ToBeEquivalentTo_WhenPropertyValuesDiffer_ShouldThrow()
    {
        var a = new { Name = "Alice" };
        var b = new { Name = "Bob" };
        Xunit.Assert.Throws<OmniAssertionException>(() => ((object)a).Verify().ToBeEquivalentTo(b));
    }

    [Fact]
    public void ToBeEquivalentTo_WhenActualIsNull_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ((object?)null).Verify().ToBeEquivalentTo(new { A = 1 }));
    }

    [Fact]
    public void ToBeEquivalentTo_WhenExpectedIsNull_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ((object)new { A = 1 }).Verify().ToBeEquivalentTo(null));
    }

    [Fact]
    public void ToBeEquivalentTo_WithinScope_WhenValuesDiffer_ShouldCollectRatherThanThrow()
    {
        var a = new { Name = "Alice" };
        var b = new { Name = "Bob" };
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            ((object)a).Verify().ToBeEquivalentTo(b);
        });
        Xunit.Assert.NotNull(ex);
    }
}
