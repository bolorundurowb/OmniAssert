namespace OmniAssert.Tests;

/// <summary>Covers <see cref="Expect"/> wrappers not already exercised via <see cref="Ensure"/> in exception tests.</summary>
public class ExpectEntryPointTests
{
    [Fact]
    public void Throws_FuncReturningValue_WhenThrows_ShouldSucceed()
    {
        var result = Expect.Throws<ArgumentException>(() => throw new ArgumentException("bad"));
        Xunit.Assert.IsType<ArgumentException>(result.Exception);
    }

    [Fact]
    public void NotThrow_FuncReturningValue_WhenNoException_ShouldSucceed()
    {
        Expect.NotThrow(() => 42);
    }

    [Fact]
    public void NotThrow_WhenExceptionThrown_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Expect.NotThrow(() => throw new InvalidOperationException("boom")));
    }

    [Fact]
    public async Task ThrowsAsync_FuncReturningValue_WhenThrows_ShouldSucceed()
    {
        var result = await Expect.ThrowsAsync<ArgumentException>(async () =>
        {
            await Task.Yield();
            throw new ArgumentException("bad");
        });
        Xunit.Assert.IsType<ArgumentException>(result.Exception);
    }

    [Fact]
    public async Task NotThrowAsync_FuncReturningValue_WhenNoException_ShouldSucceed()
    {
        await Expect.NotThrowAsync(async () =>
        {
            await Task.Yield();
            return (object?)42;
        });
    }

    [Fact]
    public async Task CompleteWithin_WhenTaskCompletesInTime_ShouldSucceed()
    {
        await Expect.CompleteWithin(TimeSpan.FromSeconds(1), async () => await Task.Delay(10));
    }

    [Fact]
    public async Task CompleteWithin_WhenTaskTimesOut_ShouldThrow()
    {
        await Xunit.Assert.ThrowsAsync<OmniAssertionException>(async () =>
            await Expect.CompleteWithin(TimeSpan.FromMilliseconds(10), async () => await Task.Delay(500)));
    }
}
