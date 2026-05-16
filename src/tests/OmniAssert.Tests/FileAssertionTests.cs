using OmniAssert;
namespace OmniAssert.Tests;

public class FileAssertionTests
{
    [Fact]
    public void FileExists_WhenFileExists_ShouldSucceed()
    {
        var path = CreateTempFilePath();
        File.WriteAllText(path, "hello");
        try
        {
            (path).FileExists();
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void FileExists_WhenFileMissing_ShouldThrow()
    {
        var path = CreateTempFilePath();
        Xunit.Assert.Throws<OmniAssertionException>(() => (path).FileExists());
    }

    [Fact]
    public void HaveContent_WhenContentMatches_ShouldSucceed()
    {
        var path = CreateTempFilePath();
        File.WriteAllText(path, "hello world");
        try
        {
            (path).FileExists().HaveContent("hello world");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void HaveContent_WhenContentDiffers_ShouldThrow()
    {
        var path = CreateTempFilePath();
        File.WriteAllText(path, "hello world");
        try
        {
            Xunit.Assert.Throws<OmniAssertionException>(() => (path).FileExists().HaveContent("other"));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void BeEmpty_WhenEmptyFile_ShouldSucceed()
    {
        var path = CreateTempFilePath();
        File.WriteAllText(path, string.Empty);
        try
        {
            (path).FileExists().BeEmpty();
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void BeEmpty_WhenFileHasContent_ShouldThrow()
    {
        var path = CreateTempFilePath();
        File.WriteAllText(path, "x");
        try
        {
            Xunit.Assert.Throws<OmniAssertionException>(() => (path).FileExists().BeEmpty());
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void HaveContent_WhenFileMissingInsideScope_ShouldCollectAssertionFailures()
    {
        var path = CreateTempFilePath();

        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using var scope = new AssertionScope();
            (path).FileExists().HaveContent("expected");
        });

        Xunit.Assert.Equal(2, ex.InnerExceptions.Count);
        foreach (var inner in ex.InnerExceptions)
            Xunit.Assert.IsType<OmniAssertionException>(inner);
    }

    private static string CreateTempFilePath() =>
        Path.Combine(Path.GetTempPath(), $"omniassert-{Guid.NewGuid():N}.tmp");
}
