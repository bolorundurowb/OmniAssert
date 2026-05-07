using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class FileSystemAssertionTests
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
    public void FileBeEmpty_WhenEmptyFile_ShouldSucceed()
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
    public void FileBeEmpty_WhenFileHasContent_ShouldThrow()
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
    public void DirectoryExists_WhenDirectoryExists_ShouldSucceed()
    {
        var path = CreateTempDirectoryPath();
        Directory.CreateDirectory(path);
        try
        {
            DirectoryExists(path);
        }
        finally
        {
            Directory.Delete(path);
        }
    }

    [Fact]
    public void DirectoryExists_WhenDirectoryMissing_ShouldThrow()
    {
        var path = CreateTempDirectoryPath();
        Xunit.Assert.Throws<OmniAssertionException>(() => DirectoryExists(path));
    }

    [Fact]
    public void DirectoryBeEmpty_WhenEmptyDirectory_ShouldSucceed()
    {
        var path = CreateTempDirectoryPath();
        Directory.CreateDirectory(path);
        try
        {
            DirectoryExists(path).BeEmpty();
        }
        finally
        {
            Directory.Delete(path);
        }
    }

    [Fact]
    public void DirectoryBeEmpty_WhenDirectoryHasEntry_ShouldThrow()
    {
        var path = CreateTempDirectoryPath();
        Directory.CreateDirectory(path);
        File.WriteAllText(Path.Combine(path, "item.txt"), "x");
        try
        {
            Xunit.Assert.Throws<OmniAssertionException>(() => DirectoryExists(path).BeEmpty());
        }
        finally
        {
            Directory.Delete(path, recursive: true);
        }
    }

    private static string CreateTempFilePath() =>
        Path.Combine(Path.GetTempPath(), $"omniassert-{Guid.NewGuid():N}.tmp");

    private static string CreateTempDirectoryPath() =>
        Path.Combine(Path.GetTempPath(), $"omniassert-{Guid.NewGuid():N}");
}
