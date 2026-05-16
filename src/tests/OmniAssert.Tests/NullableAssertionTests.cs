using OmniAssert;
namespace OmniAssert.Tests;

public class NullableAssertionTests
{
    [Fact]
    public void NullableValue_ToBeNull_WhenNull_ShouldSucceed()
    {
        int? value = null;
        (value).VerifyNullable().ToBeNull();
    }

    [Fact]
    public void NullableValue_ToBeNull_WhenHasValue_ShouldThrow()
    {
        int? value = 42;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => (value).VerifyNullable().ToBeNull());
        Xunit.Assert.Contains("null", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NullableValue_ToBeNull_WhenHasValue_MessageContainsActualValue()
    {
        int? value = 99;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => (value).VerifyNullable().ToBeNull());
        Xunit.Assert.Contains("99", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NullableValue_NotToBeNull_WhenHasValue_ShouldSucceed()
    {
        int? value = 7;
        (value).VerifyNullable().NotToBeNull();
    }

    [Fact]
    public void NullableValue_NotToBeNull_WhenNull_ShouldThrow()
    {
        int? value = null;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => (value).VerifyNullable().NotToBeNull());
        Xunit.Assert.Contains("null", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NullableValue_ToBeNull_WithinScope_WhenHasValue_ShouldCollect()
    {
        int? value = 5;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (value).VerifyNullable().ToBeNull();
        });
        Xunit.Assert.Contains("null", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NullableValue_NotToBeNull_WithinScope_WhenNull_ShouldCollect()
    {
        int? value = null;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (value).VerifyNullable().NotToBeNull();
        });
        Xunit.Assert.Contains("null", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NullableValue_StructTypes_ToBeNull_WhenNull_ShouldSucceed()
    {
        double? d = null;
        DateTime? dt = null;
        (d).VerifyNullable().ToBeNull();
        (dt).VerifyNullable().ToBeNull();
    }

    [Fact]
    public void NullableValue_StructTypes_NotToBeNull_WhenHasValue_ShouldSucceed()
    {
        double? d = 3.14;
        DateTime? dt = DateTime.UtcNow;
        (d).VerifyNullable().NotToBeNull();
        (dt).VerifyNullable().NotToBeNull();
    }

    [Fact]
    public void NullableReference_ToBeNull_WhenNull_ShouldSucceed()
    {
        string? value = null;
        (value).VerifyNullable().ToBeNull();
    }

    [Fact]
    public void NullableReference_ToBeNull_WhenNotNull_ShouldThrow()
    {
        string? value = "hello";
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => (value).VerifyNullable().ToBeNull());
        Xunit.Assert.Contains("null", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NullableReference_ToBeNull_WhenNotNull_MessageContainsActualValue()
    {
        string? value = "world";
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => (value).VerifyNullable().ToBeNull());
        Xunit.Assert.Contains("world", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NullableReference_NotToBeNull_WhenNotNull_ShouldSucceed()
    {
        string? value = "present";
        (value).VerifyNullable().NotToBeNull();
    }

    [Fact]
    public void NullableReference_NotToBeNull_WhenNull_ShouldThrow()
    {
        string? value = null;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => (value).VerifyNullable().NotToBeNull());
        Xunit.Assert.Contains("null", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NullableReference_ToBeNull_WithinScope_WhenNotNull_ShouldCollect()
    {
        string? value = "not null";
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (value).VerifyNullable().ToBeNull();
        });
        Xunit.Assert.Contains("null", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NullableReference_NotToBeNull_WithinScope_WhenNull_ShouldCollect()
    {
        string? value = null;
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            (value).VerifyNullable().NotToBeNull();
        });
        Xunit.Assert.Contains("null", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NullableReference_ClassType_ToBeNull_WhenNull_ShouldSucceed()
    {
        List<int>? list = null;
        (list).VerifyNullable().ToBeNull();
    }

    [Fact]
    public void NullableReference_ClassType_NotToBeNull_WhenNotNull_ShouldSucceed()
    {
        List<int>? list = new List<int>();
        (list).VerifyNullable().NotToBeNull();
    }
}
