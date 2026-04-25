using static OmniAssert.Assert;

namespace OmniAssert.Tests;

/// <summary>Targets branches and APIs under-covered by other suites (see Coverlet reports).</summary>
public class AdditionalCoverageTests
{
    [Fact]
    public void Throws_WhenWrongExceptionType_ShouldFail()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            Throws<ArgumentException>(() => throw new InvalidOperationException("x")));
        Xunit.Assert.Contains("InvalidOperationException", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Throws_WhenNoThrow_ShouldFail()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            Throws<ArgumentException>(() => { }));
        Xunit.Assert.Contains("did not throw", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ThrowsAsync_WhenWrongExceptionType_ShouldFail()
    {
        var ex = await Xunit.Assert.ThrowsAsync<OmniAssertionException>(async () =>
            await ThrowsAsync<ArgumentException>(async () =>
            {
                await Task.Yield();
                throw new InvalidOperationException();
            }));
        Xunit.Assert.Contains("InvalidOperationException", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ThrowsAsync_WhenNoThrow_ShouldFail()
    {
        var ex = await Xunit.Assert.ThrowsAsync<OmniAssertionException>(async () =>
            await ThrowsAsync<ArgumentException>(async () =>
            {
                await Task.Yield();
            }));
        Xunit.Assert.Contains("did not throw", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ExceptionAssertions_WithMessage_WhenMismatch_ShouldFail()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            Throws<ArgumentException>(() => throw new ArgumentException("actual"))
                .WithMessage("expected"));
        Xunit.Assert.Contains("actual", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ExceptionAssertions_WithInnerException_WhenMissing_ShouldFail()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            Throws<Exception>(() => throw new Exception("outer")).WithInnerException<InvalidOperationException>());
        Xunit.Assert.Contains("inner exception", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ExceptionAssertions_WithInnerException_WhenPresent_ShouldChain()
    {
        Throws<Exception>(() => throw new Exception("outer", new InvalidOperationException("inner")))
            .WithInnerException<InvalidOperationException>();
    }

    [Fact]
    public void Verify_StringNotToBe_WhenDifferent_ShouldSucceed()
    {
        Verify("a").NotToBe("b");
    }

    [Fact]
    public void Verify_StringNotToBe_WhenEqual_ShouldFail()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("same").NotToBe("same"));
    }

    [Fact]
    public void Verify_StringToBeNull_WhenNull_ShouldSucceed()
    {
        string? s = null;
        Verify(s).ToBeNull();
    }

    [Fact]
    public void Verify_StringToBeNull_WhenNotNull_ShouldFail()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify("x").ToBeNull());
    }

    [Fact]
    public void Verify_StringNotToBeNull_WhenSet_ShouldSucceed()
    {
        Verify("ok").NotToBeNull();
    }

    [Fact]
    public void Verify_StringNotToBeNull_WhenNull_ShouldFail()
    {
        string? s = null;
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(s).NotToBeNull());
    }

    [Fact]
    public void Verify_StringToBeNullOrEmpty_WhenEmpty_ShouldSucceed()
    {
        Verify("").ToBeNullOrEmpty();
    }

    [Fact]
    public void Verify_StringToContain_WithComparison_WhenMatch_ShouldSucceed()
    {
        Verify("AbC").ToContain("bc", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Verify_StringToContain_WithComparison_WhenMissing_ShouldFail()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify("abc").ToContain("XX", StringComparison.Ordinal));
    }

    [Fact]
    public void Verify_StringToStartWith_IgnoreCase_ShouldSucceed()
    {
        Verify("Hello").ToStartWith("hel", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Verify_StringToEndWith_IgnoreCase_ShouldSucceed()
    {
        Verify("Hello").ToEndWith("LO", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Verify_IntNotToBe_WhenDifferent_ShouldSucceed()
    {
        Verify(1).NotToBe(2);
    }

    [Fact]
    public void Verify_IntNotToBe_WhenEqual_ShouldFail()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(7).NotToBe(7));
    }

    [Fact]
    public void Verify_IntToBeGreaterThan_WhenTrue_ShouldSucceed()
    {
        Verify(10).ToBeGreaterThan(9);
    }

    [Fact]
    public void Verify_IntToBeGreaterThan_WhenFalse_ShouldFail()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(1).ToBeGreaterThan(2));
    }

    [Fact]
    public void Verify_IntToBeGreaterThanOrEqualTo_WhenEqual_ShouldSucceed()
    {
        Verify(3).ToBeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public void Verify_IntToBeLessThan_WhenTrue_ShouldSucceed()
    {
        Verify(2).ToBeLessThan(3);
    }

    [Fact]
    public void Verify_IntToBeLessThan_WhenFalse_ShouldFail()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(5).ToBeLessThan(1));
    }

    [Fact]
    public void Verify_IntToBeLessThanOrEqualTo_WhenEqual_ShouldSucceed()
    {
        Verify(4).ToBeLessThanOrEqualTo(4);
    }

    [Fact]
    public void Verify_IntToBeInRange_WhenOutOfRange_ShouldFail()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(100).ToBeInRange(0, 10));
    }

    [Fact]
    public void Verify_ByteOverload_ShouldSucceed()
    {
        Verify((byte)9).ToBe((byte)9);
    }

    [Fact]
    public void VerifyNullable_ValueTypeToBeNull_WhenHasValue_ShouldFail()
    {
        int? n = 1;
        Xunit.Assert.Throws<OmniAssertionException>(() => VerifyNullable(n).ToBeNull());
    }

    [Fact]
    public void VerifyNullable_ReferenceToBeNull_WhenNotNull_ShouldFail()
    {
        var s = "a";
        Xunit.Assert.Throws<OmniAssertionException>(() => VerifyNullable(s).ToBeNull());
    }

    [Fact]
    public void VerifyNullable_ReferenceNotToBeNull_WhenNull_ShouldFail()
    {
        string? s = null;
        Xunit.Assert.Throws<OmniAssertionException>(() => VerifyNullable(s).NotToBeNull());
    }

    [Fact]
    public void Verify_CollectionToContain_NonICollectionEnumerable_ShouldSucceed()
    {
        static IEnumerable<int> Yield123()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        Verify(Yield123()).ToContain(2);
    }

    [Fact]
    public void Verify_CollectionToBeEmpty_NonICollection_ShouldSucceed()
    {
        static IEnumerable<int> YieldNone()
        {
            yield break;
        }

        Verify(YieldNone()).ToBeEmpty();
    }

    [Fact]
    public void Verify_CollectionHasCount_NonICollection_ShouldSucceed()
    {
        static IEnumerable<int> YieldThree()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        Verify(YieldThree()).HasCount(3);
    }

    [Fact]
    public void Verify_CollectionToBeEquivalentTo_WhenCountDiffers_ShouldFail()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new[] { 1, 2 }).ToBeEquivalentTo(new[] { 1, 2, 3 }));
    }

    [Fact]
    public void Verify_CollectionToBeEquivalentTo_WhenMultisetDiffers_ShouldFail()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new[] { 1, 1, 2 }).ToBeEquivalentTo(new[] { 1, 2, 2 }));
    }

    [Fact]
    public void Verify_CollectionToBeEmpty_WhenNotEmpty_ShouldFail()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(new[] { 1 }).ToBeEmpty());
    }

    [Fact]
    public void Verify_DateTimeOffsetToBeAfter_WhenNotAfter_ShouldFail()
    {
        var t = DateTimeOffset.Parse("2020-01-01T00:00:00Z");
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(t).ToBeAfter(t.AddDays(1)));
    }

    [Fact]
    public void Verify_DateTimeOffsetToBeBefore_WhenNotBefore_ShouldFail()
    {
        var t = DateTimeOffset.Parse("2020-06-01T00:00:00Z");
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(t).ToBeBefore(t.AddDays(-1)));
    }

    [Fact]
    public void Verify_DateTimeOffsetToBeWithin_WhenOutside_ShouldFail()
    {
        var t = DateTimeOffset.Parse("2030-01-01T12:00:00Z");
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(t).ToBeWithin(TimeSpan.FromSeconds(1), t.AddHours(1)));
    }

    [Fact]
    public async Task CompleteWithin_WhenTaskThrows_ShouldPropagate()
    {
        await Xunit.Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await CompleteWithin(TimeSpan.FromSeconds(2), async () =>
            {
                await Task.Yield();
                throw new InvalidOperationException("boom");
            }));
    }
}
