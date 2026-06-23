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
}
