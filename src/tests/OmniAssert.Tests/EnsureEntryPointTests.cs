namespace OmniAssert.Tests;

public class EnsureEntryPointTests
{
    [Fact]
    public void Must_string_assertions_work_from_Ensure_entry()
    {
        "hello".Must().Contain("ell");
        "hello".Must().StartWith("he");
    }

    [Fact]
    public void VerifyNullable_still_uses_modern_grammar()
    {
        string? value = "ok";
        value.VerifyNullable().NotBeNull();
        value.Must().Be("ok");
    }

    [Fact]
    public void Ensure_throws_and_not_throw_work()
    {
        Ensure.Throws<ArgumentException>(() => throw new ArgumentException("bad"));
        Ensure.NotThrow(() => { });
    }

    [Fact]
    public async Task Ensure_async_exception_helpers_work()
    {
        await Ensure.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await Task.Yield();
            throw new InvalidOperationException();
        });

        await Ensure.NotThrowAsync(async () => await Task.Yield());
    }

    [Fact]
    public void FileExists_and_DirectoryExists_resolve_via_Ensure()
    {
        var file = Path.Combine(Path.GetTempPath(), $"omni-{Guid.NewGuid():N}.txt");
        var dir = Path.Combine(Path.GetTempPath(), $"omni-{Guid.NewGuid():N}");
        try
        {
            File.WriteAllText(file, "x");
            Directory.CreateDirectory(dir);

            file.FileExists();
            dir.DirectoryExists().BeEmpty();
        }
        finally
        {
            if (File.Exists(file))
                File.Delete(file);
            if (Directory.Exists(dir))
                Directory.Delete(dir);
        }
    }

    [Fact]
    public void Throws_WhenWrongExceptionType_ShouldFail()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Ensure.Throws<ArgumentException>(() => throw new InvalidOperationException("wrong")));
    }

    [Fact]
    public void Throws_WhenNoExceptionThrown_ShouldFail()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Ensure.Throws<ArgumentException>(() => { }));
    }

    [Fact]
    public void Throws_Func_WhenCorrect_ShouldSucceed()
    {
        var result = Ensure.Throws<ArgumentException>((Func<object?>)(() => throw new ArgumentException("bad")));
        Xunit.Assert.NotNull(result.Exception);
    }

    [Fact]
    public void Throws_Func_WhenWrongType_ShouldFail()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Ensure.Throws<ArgumentException>((Func<object?>)(() => throw new InvalidOperationException("wrong"))));
    }

    [Fact]
    public void Throws_Func_WhenNoThrow_ShouldFail()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Ensure.Throws<ArgumentException>((Func<object?>)(() => 42)));
    }

    [Fact]
    public void NotThrow_WhenThrows_ShouldFail()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Ensure.NotThrow(() => throw new InvalidOperationException("boom")));
    }

    [Fact]
    public void NotThrow_Func_WhenNoThrow_ShouldSucceed()
    {
        Ensure.NotThrow((Func<object?>)(() => 42));
    }

    [Fact]
    public void NotThrow_Func_WhenThrows_ShouldFail()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Ensure.NotThrow((Func<object?>)(() => throw new InvalidOperationException("boom"))));
    }

    [Fact]
    public async Task ThrowsAsync_WhenWrongExceptionType_ShouldFail()
    {
        await Xunit.Assert.ThrowsAsync<OmniAssertionException>(async () =>
            await Ensure.ThrowsAsync<ArgumentException>(async () =>
            {
                await Task.Yield();
                throw new InvalidOperationException("wrong");
            }));
    }

    [Fact]
    public async Task ThrowsAsync_WhenNoExceptionThrown_ShouldFail()
    {
        await Xunit.Assert.ThrowsAsync<OmniAssertionException>(async () =>
            await Ensure.ThrowsAsync<ArgumentException>(async () =>
            {
                await Task.Yield();
            }));
    }

    [Fact]
    public async Task ThrowsAsync_Func_WhenCorrect_ShouldSucceed()
    {
        var result = await Ensure.ThrowsAsync<ArgumentException>((Func<Task<object?>>)(async () =>
        {
            await Task.Yield();
            throw new ArgumentException("bad");
        }));
        Xunit.Assert.NotNull(result.Exception);
    }

    [Fact]
    public async Task ThrowsAsync_Func_WhenWrongType_ShouldFail()
    {
        await Xunit.Assert.ThrowsAsync<OmniAssertionException>(async () =>
            await Ensure.ThrowsAsync<ArgumentException>((Func<Task<object?>>)(async () =>
            {
                await Task.Yield();
                throw new InvalidOperationException("wrong");
            })));
    }

    [Fact]
    public async Task ThrowsAsync_Func_WhenNoThrow_ShouldFail()
    {
        await Xunit.Assert.ThrowsAsync<OmniAssertionException>(async () =>
            await Ensure.ThrowsAsync<ArgumentException>((Func<Task<object?>>)(async () =>
            {
                await Task.Yield();
                return 42;
            })));
    }

    [Fact]
    public async Task NotThrowAsync_WhenThrows_ShouldFail()
    {
        await Xunit.Assert.ThrowsAsync<OmniAssertionException>(async () =>
            await Ensure.NotThrowAsync(async () =>
            {
                await Task.Yield();
                throw new InvalidOperationException("boom");
            }));
    }

    [Fact]
    public async Task NotThrowAsync_Func_WhenNoThrow_ShouldSucceed()
    {
        await Ensure.NotThrowAsync((Func<Task<object?>>)(async () =>
        {
            await Task.Yield();
            return 42;
        }));
    }

    [Fact]
    public async Task NotThrowAsync_Func_WhenThrows_ShouldFail()
    {
        await Xunit.Assert.ThrowsAsync<OmniAssertionException>(async () =>
            await Ensure.NotThrowAsync((Func<Task<object?>>)(async () =>
            {
                await Task.Yield();
                throw new InvalidOperationException("boom");
            })));
    }

    [Fact]
    public async Task CompleteWithin_WhenCompletesInTime_ShouldSucceed()
    {
        await TimeSpan.FromSeconds(5).CompleteWithin(async () =>
        {
            await Task.Yield();
        });
    }

    [Fact]
    public async Task CompleteWithin_WhenTimesOut_ShouldFail()
    {
        await Xunit.Assert.ThrowsAsync<OmniAssertionException>(async () =>
            await TimeSpan.FromMilliseconds(1).CompleteWithin(async () =>
            {
                await Task.Delay(5000);
            }));
    }

    [Fact]
    public void Expression_with_AssertionCapture_WhenTrue_ShouldSucceed()
    {
        var capture = new AssertionCapture("1 + 1 == 2", null);
        Ensure.Expression(true, capture);
    }

    [Fact]
    public void Expression_with_AssertionCapture_WhenFalse_ShouldThrow()
    {
        var capture = new AssertionCapture("1 + 1 == 3", null);
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Ensure.Expression(false, capture));
    }

    [Fact]
    public void Expression_and_VerifyExpression_produce_identical_outcomes()
    {
#pragma warning disable CS0618
#pragma warning disable OA004
        var capture = new AssertionCapture("flag", null);
        Ensure.Expression(true, capture);
        Ensure.VerifyExpression(true, capture);
        Xunit.Assert.Throws<OmniAssertionException>(() => Ensure.Expression(false, capture));
        Xunit.Assert.Throws<OmniAssertionException>(() => Ensure.VerifyExpression(false, capture));
#pragma warning restore OA004
#pragma warning restore CS0618
    }

    [Fact]
    public void VerifyExpression_with_AssertionCapture_WhenTrue_ShouldSucceed()
    {
#pragma warning disable CS0618
#pragma warning disable OA004
        var capture = new AssertionCapture("1 + 1 == 2", null);
        Ensure.VerifyExpression(true, capture);
#pragma warning restore OA004
#pragma warning restore CS0618
    }

    [Fact]
    public void VerifyExpression_with_AssertionCapture_WhenFalse_ShouldThrow()
    {
#pragma warning disable CS0618
#pragma warning disable OA004
        var capture = new AssertionCapture("1 + 1 == 3", null);
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Ensure.VerifyExpression(false, capture));
#pragma warning restore OA004
#pragma warning restore CS0618
    }

    [Fact]
    public void FileExists_WhenFileDoesNotExist_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            "C:\\nonexistent\\file.txt".FileExists());
    }

    [Fact]
    public void DirectoryExists_WhenDirDoesNotExist_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            "C:\\nonexistent\\dir".DirectoryExists());
    }
}
