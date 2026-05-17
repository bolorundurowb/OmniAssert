namespace OmniAssert.Tests;

public class ExceptionAssertionTests
{
    // ── Throws ───────────────────────────────────────────────────────────────

    [Fact]
    public void Throws_WhenCorrectExceptionType_ShouldSucceed()
    {
        var result = ((Action)(() => throw new ArgumentException("bad"))).Throws<ArgumentException>();
        Xunit.Assert.IsType<ArgumentException>(result.Exception);
    }

    [Fact]
    public void Throws_WhenWrongExceptionType_ShouldThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            ((Action)(() => throw new InvalidOperationException("x"))).Throws<ArgumentException>());
        Xunit.Assert.Contains("InvalidOperationException", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Throws_WhenNoExceptionThrown_ShouldThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            ((Action)(() => { })).Throws<ArgumentException>());
        Xunit.Assert.Contains("did not throw", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ── ThrowsAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task ThrowsAsync_WhenCorrectExceptionType_ShouldSucceed()
    {
        var result = await ((Func<Task>)(async () =>
        {
            await Task.Yield();
            throw new ArgumentException("bad");
        })).ThrowsAsync<ArgumentException>();
        Xunit.Assert.IsType<ArgumentException>(result.Exception);
    }

    [Fact]
    public async Task ThrowsAsync_WhenWrongExceptionType_ShouldThrow()
    {
        var ex = await Xunit.Assert.ThrowsAsync<OmniAssertionException>(async () =>
            await ((Func<Task>)(async () =>
            {
                await Task.Yield();
                throw new InvalidOperationException();
            })).ThrowsAsync<ArgumentException>());
        Xunit.Assert.Contains("InvalidOperationException", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ThrowsAsync_WhenNoExceptionThrown_ShouldThrow()
    {
        var ex = await Xunit.Assert.ThrowsAsync<OmniAssertionException>(async () =>
            await ((Func<Task>)(async () => await Task.Yield())).ThrowsAsync<ArgumentException>());
        Xunit.Assert.Contains("did not throw", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ── NotThrow ─────────────────────────────────────────────────────────────

    [Fact]
    public void NotThrow_WhenNoException_ShouldSucceed()
    {
        ((Action)(() => { })).NotThrow();
    }

    [Fact]
    public void NotThrow_WhenExceptionThrown_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => ((Action)(() => throw new Exception("fail"))).NotThrow());
    }

    // ── NotThrowAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task NotThrowAsync_WhenNoException_ShouldSucceed()
    {
        await ((Func<Task>)(async () => await Task.Yield())).NotThrowAsync();
    }

    [Fact]
    public async Task NotThrowAsync_WhenExceptionThrown_ShouldThrow()
    {
        await Xunit.Assert.ThrowsAsync<OmniAssertionException>(async () => await ((Func<Task>)(async () =>
        {
            await Task.Yield();
            throw new Exception("fail");
        })).NotThrowAsync());
    }

    // ── ExceptionAssertions.WithMessage ──────────────────────────────────────

    [Fact]
    public void WithMessage_WhenMessageMatches_ShouldSucceed()
    {
        ((Action)(() => throw new ArgumentException("bad"))).Throws<ArgumentException>()
            .WithMessage("bad");
    }

    [Fact]
    public void WithMessage_WhenMessageMismatch_ShouldThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            ((Action)(() => throw new ArgumentException("actual"))).Throws<ArgumentException>()
                .WithMessage("expected"));
        Xunit.Assert.Contains("actual", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void WithMessageContaining_WhenSubstringExists_ShouldSucceed()
    {
        ((Action)(() => throw new ArgumentException("User 42 not found in /api/users")))
            .Throws<ArgumentException>()
            .WithMessageContaining("42 not found");
    }

    [Fact]
    public void WithMessageContaining_WhenSubstringMissing_ShouldThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            ((Action)(() => throw new ArgumentException("User 42 not found in /api/users")))
                .Throws<ArgumentException>()
                .WithMessageContaining("99 not found"));
        Xunit.Assert.Contains("42 not found", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ThrowsAsync_WithMessageContaining_WhenSubstringExists_ShouldSucceed()
    {
        (await ((Func<Task>)(async () =>
        {
            await Task.Yield();
            throw new ArgumentException("Entity id=123 failed validation");
        })).ThrowsAsync<ArgumentException>()).WithMessageContaining("id=123");
    }

    // ── ExceptionAssertions.WithInnerException ───────────────────────────────

    [Fact]
    public void WithInnerException_WhenInnerExceptionPresent_ShouldSucceed()
    {
        ((Action)(() => throw new Exception("outer", new InvalidOperationException("inner"))))
            .Throws<Exception>()
            .WithInnerException<InvalidOperationException>();
    }

    [Fact]
    public void WithInnerException_WhenInnerExceptionMissing_ShouldThrow()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
            ((Action)(() => throw new Exception("outer"))).Throws<Exception>().WithInnerException<InvalidOperationException>());
        Xunit.Assert.Contains("inner exception", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WithMessage_AndWithInnerException_CanBeChained()
    {
        var inner = new InvalidOperationException("inner");
        ((Action)(() => throw new Exception("outer", inner)))
            .Throws<Exception>()
            .WithInnerException<InvalidOperationException>();
    }

    // ── CompleteWithin ───────────────────────────────────────────────────────

    [Fact]
    public async Task CompleteWithin_WhenTaskCompletesInTime_ShouldSucceed()
    {
        await TimeSpan.FromSeconds(1).CompleteWithin(async () => await Task.Delay(10));
    }

    [Fact]
    public async Task CompleteWithin_WhenTaskTimesOut_ShouldThrow()
    {
        await Xunit.Assert.ThrowsAsync<OmniAssertionException>(async () =>
            await TimeSpan.FromMilliseconds(10).CompleteWithin(async () => await Task.Delay(500)));
    }

    [Fact]
    public async Task CompleteWithin_WhenTaskThrowsException_ShouldPropagate()
    {
        await Xunit.Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await TimeSpan.FromSeconds(2).CompleteWithin(async () =>
            {
                await Task.Yield();
                throw new InvalidOperationException("boom");
            }));
    }
}
