using static OmniAssert.Assert;

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
            FileExists(path);
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
        Xunit.Assert.Throws<OmniAssertionException>(() => FileExists(path));
    }

    [Fact]
    public void HaveContent_WhenContentMatches_ShouldSucceed()
    {
        var path = CreateTempFilePath();
        File.WriteAllText(path, "hello world");
        try
        {
            FileExists(path).HaveContent("hello world");
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
            Xunit.Assert.Throws<OmniAssertionException>(() => FileExists(path).HaveContent("other"));
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
            FileExists(path).BeEmpty();
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
            Xunit.Assert.Throws<OmniAssertionException>(() => FileExists(path).BeEmpty());
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
            FileExists(path).HaveContent("expected");
        });

        Xunit.Assert.Equal(2, ex.InnerExceptions.Count);
        foreach (var inner in ex.InnerExceptions)
            Xunit.Assert.IsType<OmniAssertionException>(inner);
    }

    private static string CreateTempFilePath() =>
        Path.Combine(Path.GetTempPath(), $"omniassert-{Guid.NewGuid():N}.tmp");
}
