using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class ExtendedAssertionTests
{
    [Fact]
    public void Verify_Double_ToBeApproximately_ShouldSucceed()
    {
        Verify(1.0).ToBeApproximately(1.001, 0.01);
    }

    [Fact]
    public void Verify_Double_ToBeInRange_ShouldSucceed()
    {
        Verify(5.0).ToBeInRange(1.0, 10.0);
    }

    [Fact]
    public void VerifyNullable_ReferenceType_ToBeNull_ShouldSucceed()
    {
        string? val = null;
        VerifyNullable(val).ToBeNull();
    }

    [Fact]
    public void VerifyNullable_ValueType_NotToBeNull_ShouldSucceed()
    {
        int? val = 42;
        VerifyNullable(val).NotToBeNull();
    }

    [Fact]
    public void String_Advanced_ShouldSucceed()
    {
        Verify("hello world").ToStartWith("hello");
        Verify("hello world").ToEndWith("world");
        Verify("123-456").ToMatch(@"^\d{3}-\d{3}$");
        Verify("HELLO").ToBe("hello", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Collection_Advanced_ShouldSucceed()
    {
        Verify([1, 2, 3]).HasCount(3);
        Verify([1, 2, 3]).AllSatisfy(x => x > 0);
        Verify([1, 2, 3]).ToBeEquivalentTo([3, 2, 1]);
        Verify([1, 2, 3]).NotToContain(4);
    }

    [Fact]
    public void Exception_Assertions_ShouldSucceed()
    {
        Throws<ArgumentException>(() => throw new ArgumentException("bad"))
            .WithMessage("bad");
        
        NotThrow(() => { });
    }

    [Fact]
    public async Task Async_Exception_Assertions_ShouldSucceed()
    {
        await ThrowsAsync<ArgumentException>(async () => 
        {
            await Task.Yield();
            throw new ArgumentException("bad");
        }).ContinueWith(t => t.Result.WithMessage("bad"));

        await NotThrowAsync(async () => await Task.Yield());
    }

    [Fact]
    public async Task Async_CompleteWithin_ShouldSucceed()
    {
        await CompleteWithin(TimeSpan.FromSeconds(1), async () => await Task.Delay(10));
    }

    [Fact]
    public void Type_Assertions_ShouldSucceed()
    {
        object obj = "hello";
        Verify(obj).ToBeOfType<string>();
        Verify(obj).ToBeAssignableTo<IEnumerable<char>>();
    }

    [Fact]
    public void DateTime_Assertions_ShouldSucceed()
    {
        var now = DateTime.UtcNow;
        Verify(now).ToBeAfter(now.AddSeconds(-1));
        Verify(now).ToBeBefore(now.AddSeconds(1));
        Verify(now).ToBeWithin(TimeSpan.FromSeconds(1), now.AddMilliseconds(500));
    }
}
