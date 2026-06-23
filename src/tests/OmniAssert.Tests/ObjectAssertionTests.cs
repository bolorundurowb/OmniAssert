namespace OmniAssert.Tests;

public class ObjectAssertionTests
{
    private class Animal { }
    private class Dog : Animal { }

    [Fact]
    public void ToBeNull_WhenObjectIsNull_ShouldSucceed()
    {
        ((object?)null).Must().BeNull();
    }

    [Fact]
    public void ToBeNull_WhenObjectIsNotNull_ShouldThrow()
    {
        object obj = "hello";
        Xunit.Assert.Throws<OmniAssertionException>(() => obj.Must().BeNull());
    }

    [Fact]
    public void ToBeNull_WithinScope_WhenObjectIsNotNull_ShouldCollectRatherThanThrow()
    {
        object obj = "hello";
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            obj.Must().BeNull();
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void NotToBeNull_WhenObjectIsNotNull_ShouldSucceed()
    {
        object obj = "hello";
        obj.Must().NotBeNull();
    }

    [Fact]
    public void NotToBeNull_WhenObjectIsNull_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ((object?)null).Must().NotBeNull());
    }

    [Fact]
    public void NotToBeNull_WithinScope_WhenObjectIsNull_ShouldCollectRatherThanThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            ((object?)null).Must().NotBeNull();
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void ToBeOfType_WhenExactTypeMatches_ShouldSucceed()
    {
        object obj = "hello";
        (obj).Must().BeOfType<string>();
    }

    [Fact]
    public void ToBeOfType_WhenTypeDiffers_ShouldThrow()
    {
        object obj = 42;
        Xunit.Assert.Throws<OmniAssertionException>(() => (obj).Must().BeOfType<string>());
    }

    [Fact]
    public void ToBeOfType_WhenSubtypeProvided_ShouldThrow()
    {
        object obj = new Dog();
        Xunit.Assert.Throws<OmniAssertionException>(() => (obj).Must().BeOfType<Animal>());
    }

    [Fact]
    public void ToBeOfType_WithinScope_WhenTypeDiffers_ShouldCollectRatherThanThrow()
    {
        object obj = 42;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (obj).Must().BeOfType<string>();
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void ToBeAssignableTo_WhenExactTypeMatches_ShouldSucceed()
    {
        object obj = "hello";
        (obj).Must().BeAssignableTo<string>();
    }

    [Fact]
    public void ToBeAssignableTo_WhenSubtypeMatches_ShouldSucceed()
    {
        object obj = new Dog();
        (obj).Must().BeAssignableTo<Animal>();
    }

    [Fact]
    public void ToBeAssignableTo_WhenInterfaceMatches_ShouldSucceed()
    {
        object obj = "hello";
        (obj).Must().BeAssignableTo<IEnumerable<char>>();
    }

    [Fact]
    public void ToBeAssignableTo_WhenNotAssignable_ShouldThrow()
    {
        object obj = 42;
        Xunit.Assert.Throws<OmniAssertionException>(() => (obj).Must().BeAssignableTo<IEnumerable<int>>());
    }

    [Fact]
    public void ToBeAssignableTo_WithinScope_WhenNotAssignable_ShouldCollectRatherThanThrow()
    {
        object obj = 42;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (obj).Must().BeAssignableTo<string>();
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void NotToBeOfType_WhenTypeDiffers_ShouldSucceed()
    {
        object obj = 42;
        obj.Must().NotBeOfType<string>();
    }

    [Fact]
    public void NotToBeOfType_WhenExactTypeMatches_ShouldThrow()
    {
        object obj = "hello";
        Xunit.Assert.Throws<OmniAssertionException>(() => obj.Must().NotBeOfType<string>());
    }

    [Fact]
    public void NotToBeOfType_WhenSubtypeProvided_ShouldSucceed()
    {
        object obj = new Dog();
        obj.Must().NotBeOfType<Animal>();
    }

    [Fact]
    public void NotToBeOfType_WithinScope_WhenExactTypeMatches_ShouldCollectRatherThanThrow()
    {
        object obj = "hello";
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            obj.Must().NotBeOfType<string>();
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void NotToBeAssignableTo_WhenNotAssignable_ShouldSucceed()
    {
        object obj = 42;
        obj.Must().NotBeAssignableTo<string>();
    }

    [Fact]
    public void NotToBeAssignableTo_WhenExactTypeMatches_ShouldThrow()
    {
        object obj = "hello";
        Xunit.Assert.Throws<OmniAssertionException>(() => obj.Must().NotBeAssignableTo<string>());
    }

    [Fact]
    public void NotToBeAssignableTo_WhenSubtypeMatches_ShouldThrow()
    {
        object obj = new Dog();
        Xunit.Assert.Throws<OmniAssertionException>(() => obj.Must().NotBeAssignableTo<Animal>());
    }

    [Fact]
    public void NotToBeAssignableTo_WithinScope_WhenAssignable_ShouldCollectRatherThanThrow()
    {
        object obj = "hello";
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            obj.Must().NotBeAssignableTo<string>();
        });
        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void ToBeEquivalentTo_WhenObjectsEquivalent_ShouldSucceed()
    {
        var a = new { Name = "Alice", Age = 30 };
        var b = new { Name = "Alice", Age = 30 };
        ((object)a).Must().BeEquivalentTo(b);
    }

    [Fact]
    public void ToBeEquivalentTo_WhenPropertyValuesDiffer_ShouldThrow()
    {
        var a = new { Name = "Alice" };
        var b = new { Name = "Bob" };
        Xunit.Assert.Throws<OmniAssertionException>(() => ((object)a).Must().BeEquivalentTo(b));
    }

    [Fact]
    public void ToBeEquivalentTo_WhenActualIsNull_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ((object?)null).Must().BeEquivalentTo(new { A = 1 }));
    }

    [Fact]
    public void ToBeEquivalentTo_WhenExpectedIsNull_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ((object)new { A = 1 }).Must().BeEquivalentTo(null));
    }

    [Fact]
    public void ToBeEquivalentTo_WithinScope_WhenValuesDiffer_ShouldCollectRatherThanThrow()
    {
        var a = new { Name = "Alice" };
        var b = new { Name = "Bob" };
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            ((object)a).Must().BeEquivalentTo(b);
        });
        Xunit.Assert.NotNull(ex);
    }
}
