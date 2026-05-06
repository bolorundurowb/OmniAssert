using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class ExceptionAssertionTests
{
    // ── Throws ───────────────────────────────────────────────────────────────

    [Fact]
    public void Throws_WhenCorrectExceptionType_ShouldSucceed()
    {
        var result = Throws<ArgumentException>(() => throw new ArgumentException("bad"));
        Xunit.Assert.IsType<ArgumentException>(result.Exception);
    }

    [Fact]
    public void Throws_WhenWrongExceptionType_ShouldThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            Throws<ArgumentException>(() => throw new InvalidOperationException("x")));
        Xunit.Assert.Contains("InvalidOperationException", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Throws_WhenNoExceptionThrown_ShouldThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            Throws<ArgumentException>(() => { }));
        Xunit.Assert.Contains("did not throw", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ── ThrowsAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task ThrowsAsync_WhenCorrectExceptionType_ShouldSucceed()
    {
        var result = await ThrowsAsync<ArgumentException>(async () =>
        {
            await Task.Yield();
            throw new ArgumentException("bad");
        });
        Xunit.Assert.IsType<ArgumentException>(result.Exception);
    }

    [Fact]
    public async Task ThrowsAsync_WhenWrongExceptionType_ShouldThrow()
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
    public async Task ThrowsAsync_WhenNoExceptionThrown_ShouldThrow()
    {
        var ex = await Xunit.Assert.ThrowsAsync<OmniAssertionException>(async () =>
            await ThrowsAsync<ArgumentException>(async () => await Task.Yield()));
        Xunit.Assert.Contains("did not throw", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ── NotThrow ─────────────────────────────────────────────────────────────

    [Fact]
    public void NotThrow_WhenNoException_ShouldSucceed()
    {
        NotThrow(() => { });
    }

    [Fact]
    public void NotThrow_WhenExceptionThrown_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => NotThrow(() => throw new Exception("fail")));
    }

    // ── NotThrowAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task NotThrowAsync_WhenNoException_ShouldSucceed()
    {
        await NotThrowAsync(async () => await Task.Yield());
    }

    [Fact]
    public async Task NotThrowAsync_WhenExceptionThrown_ShouldThrow()
    {
        await Xunit.Assert.ThrowsAsync<OmniAssertionException>(async () => await NotThrowAsync(async () =>
        {
            await Task.Yield();
            throw new Exception("fail");
        }));
    }

    // ── ExceptionAssertions.WithMessage ──────────────────────────────────────

    [Fact]
    public void WithMessage_WhenMessageMatches_ShouldSucceed()
    {
        Throws<ArgumentException>(() => throw new ArgumentException("bad"))
            .WithMessage("bad");
    }

    [Fact]
    public void WithMessage_WhenMessageMismatch_ShouldThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            Throws<ArgumentException>(() => throw new ArgumentException("actual"))
                .WithMessage("expected"));
        Xunit.Assert.Contains("actual", ex.Message, StringComparison.Ordinal);
    }

    // ── ExceptionAssertions.WithInnerException ───────────────────────────────

    [Fact]
    public void WithInnerException_WhenInnerExceptionPresent_ShouldSucceed()
    {
        Throws<Exception>(() => throw new Exception("outer", new InvalidOperationException("inner")))
            .WithInnerException<InvalidOperationException>();
    }

    [Fact]
    public void WithInnerException_WhenInnerExceptionMissing_ShouldThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            Throws<Exception>(() => throw new Exception("outer")).WithInnerException<InvalidOperationException>());
        Xunit.Assert.Contains("inner exception", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WithMessage_AndWithInnerException_CanBeChained()
    {
        var inner = new InvalidOperationException("inner");
        Throws<Exception>(() => throw new Exception("outer", inner))
            .WithInnerException<InvalidOperationException>();
    }

    // ── CompleteWithin ───────────────────────────────────────────────────────

    [Fact]
    public async Task CompleteWithin_WhenTaskCompletesInTime_ShouldSucceed()
    {
        await CompleteWithin(TimeSpan.FromSeconds(1), async () => await Task.Delay(10));
    }

    [Fact]
    public async Task CompleteWithin_WhenTaskTimesOut_ShouldThrow()
    {
        await Xunit.Assert.ThrowsAsync<OmniAssertionException>(async () =>
            await CompleteWithin(TimeSpan.FromMilliseconds(10), async () => await Task.Delay(500)));
    }

    [Fact]
    public async Task CompleteWithin_WhenTaskThrowsException_ShouldPropagate()
    {
        await Xunit.Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await CompleteWithin(TimeSpan.FromSeconds(2), async () =>
            {
                await Task.Yield();
                throw new InvalidOperationException("boom");
            }));
    }
}
