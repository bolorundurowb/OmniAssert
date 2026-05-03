using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class ExtendedAssertionTests
{
    [Fact]
    public void Verify_DoubleToBeApproximately_WhenWithinTolerance_ShouldSucceed()
    {
        Verify(1.0).ToBeApproximately(1.001, 0.01);
    }

    [Fact]
    public void Verify_DoubleToBeInRange_WhenInBounds_ShouldSucceed()
    {
        Verify(5.0).ToBeInRange(1.0, 10.0);
    }

    [Fact]
    public void VerifyNullable_ReferenceTypeToBeNull_WhenNull_ShouldSucceed()
    {
        string? val = null;
        VerifyNullable(val).ToBeNull();
    }

    [Fact]
    public void VerifyNullable_ValueTypeNotToBeNull_WhenNotNull_ShouldSucceed()
    {
        int? val = 42;
        VerifyNullable(val).NotToBeNull();
    }

    [Fact]
    public void Verify_StringAdvanced_WhenMatching_ShouldSucceed()
    {
        Verify("hello world").ToStartWith("hello");
        Verify("hello world").ToEndWith("world");
        Verify("123-456").ToMatch(@"^\d{3}-\d{3}$");
        Verify("HELLO").ToBe("hello", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Verify_StringToStartWith_WhenMismatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("hello").ToStartWith("hi"));
    }

    [Fact]
    public void Verify_StringToEndWith_WhenMismatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("hello").ToEndWith("bye"));
    }

    [Fact]
    public void Verify_StringToMatch_WhenMismatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("123").ToMatch(@"^[a-z]+$"));
    }

    [Fact]
    public void Verify_CollectionAdvanced_WhenValid_ShouldSucceed()
    {
        Verify([1, 2, 3]).HasCount(3);
        Verify([1, 2, 3]).AllSatisfy(x => x > 0);
        Verify([1, 2, 3]).ToBeEquivalentTo([3, 2, 1]);
        Verify([1, 2, 3]).NotToContain(4);
    }

    [Fact]
    public void Verify_CollectionToContain_WhenMissing_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify([1, 2, 3]).ToContain(4));
    }

    [Fact]
    public void Verify_CollectionNotToContain_WhenPresent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify([1, 2, 3]).NotToContain(2));
    }

    [Fact]
    public void Verify_CollectionHasCount_WhenCountDiffers_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify([1, 2, 3]).HasCount(2));
    }

    [Fact]
    public void Verify_CollectionAllSatisfy_WhenOneFails_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify([1, 2, -1]).AllSatisfy(x => x > 0));
    }

    [Fact]
    public void Verify_CollectionToBeEquivalentTo_WhenElementsDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify([1, 2, 3]).ToBeEquivalentTo([1, 2, 4]));
    }

    [Fact]
    public void Verify_ExceptionAssertions_WhenConditionsMet_ShouldSucceed()
    {
        Throws<ArgumentException>(() => throw new ArgumentException("bad"))
            .WithMessage("bad");

        NotThrow(() => { });
    }

    [Fact]
    public void Verify_NotThrow_WhenThrows_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => NotThrow(() => throw new Exception("fail")));
    }

    [Fact]
    public async Task Verify_AsyncExceptionAssertions_WhenConditionsMet_ShouldSucceed()
    {
        await ThrowsAsync<ArgumentException>(async () =>
        {
            await Task.Yield();
            throw new ArgumentException("bad");
        }).ContinueWith(t => t.Result.WithMessage("bad"));

        await NotThrowAsync(async () => await Task.Yield());
    }

    [Fact]
    public async Task Verify_NotThrowAsync_WhenThrows_ShouldThrow()
    {
        await Xunit.Assert.ThrowsAsync<OmniAssertionException>(async () => await NotThrowAsync(async () =>
        {
            await Task.Yield();
            throw new Exception("fail");
        }));
    }

    [Fact]
    public async Task CompleteWithin_Action_WhenCompletesInTime_ShouldSucceed()
    {
        await CompleteWithin(TimeSpan.FromSeconds(1), async () => await Task.Delay(10));
    }

    [Fact]
    public async Task CompleteWithin_Action_WhenTimesOut_ShouldThrow()
    {
        await Xunit.Assert.ThrowsAsync<OmniAssertionException>(async () =>
            await CompleteWithin(TimeSpan.FromMilliseconds(10), async () => await Task.Delay(100)));
    }

    [Fact]
    public void Verify_TypeAssertions_WhenTypeMatches_ShouldSucceed()
    {
        object obj = "hello";
        Verify(obj).ToBeOfType<string>();
        Verify(obj).ToBeAssignableTo<IEnumerable<char>>();
    }

    [Fact]
    public void Verify_ToBeOfType_WhenTypeDiffers_ShouldThrow()
    {
        object obj = 42;
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(obj).ToBeOfType<string>());
    }

    [Fact]
    public void Verify_ToBeAssignableTo_WhenNotAssignable_ShouldThrow()
    {
        object obj = 42;
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(obj).ToBeAssignableTo<IEnumerable<int>>());
    }

    [Fact]
    public void Verify_DateTimeAssertions_WhenConditionsMet_ShouldSucceed()
    {
        var now = DateTime.UtcNow;
        Verify(now).ToBeAfter(now.AddSeconds(-1));
        Verify(now).ToBeBefore(now.AddSeconds(1));
        Verify(now).ToBeWithin(TimeSpan.FromSeconds(1), now.AddMilliseconds(500));
    }

    [Fact]
    public void Verify_DateTimeToBeAfter_WhenBefore_ShouldThrow()
    {
        var now = DateTime.UtcNow;
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(now).ToBeAfter(now.AddSeconds(1)));
    }

    [Fact]
    public void Verify_DateTimeToBeBefore_WhenAfter_ShouldThrow()
    {
        var now = DateTime.UtcNow;
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(now).ToBeBefore(now.AddSeconds(-1)));
    }

    [Fact]
    public void Verify_DateTimeToBeWithin_WhenOutsidePrecision_ShouldThrow()
    {
        var now = DateTime.UtcNow;
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(now).ToBeWithin(TimeSpan.FromMilliseconds(100), now.AddSeconds(1)));
    }

    [Fact]
    public void Verify_DateTimeOffsetAssertions_WhenConditionsMet_ShouldSucceed()
    {
        var now = DateTimeOffset.UtcNow;
        Verify(now).ToBeAfter(now.AddSeconds(-1));
        Verify(now).ToBeBefore(now.AddSeconds(1));
        Verify(now).ToBeWithin(TimeSpan.FromSeconds(1), now.AddMilliseconds(500));
    }
}
