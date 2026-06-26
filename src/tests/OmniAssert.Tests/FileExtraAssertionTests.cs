namespace OmniAssert.Tests;

public class FileExtraAssertionTests
{
    [Fact]
    public void NotBeEmpty_WhenFileHasContent_ShouldSucceed()
    {
        var path = CreateTempFilePath();
        File.WriteAllText(path, "data");
        try
        {
            (path).FileExists().NotBeEmpty();
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void NotBeEmpty_WhenFileEmpty_ShouldThrow()
    {
        var path = CreateTempFilePath();
        File.WriteAllText(path, string.Empty);
        try
        {
            Xunit.Assert.Throws<OmniAssertionException>(() => (path).FileExists().NotBeEmpty());
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void HaveExtension_WhenMatchesWithDot_ShouldSucceed()
    {
        var path = CreateTempFilePath(".json");
        File.WriteAllText(path, "{}");
        try
        {
            (path).FileExists().HaveExtension(".json");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void HaveExtension_WhenMatchesWithoutDotAndDifferentCase_ShouldSucceed()
    {
        var path = CreateTempFilePath(".JSON");
        File.WriteAllText(path, "{}");
        try
        {
            (path).FileExists().HaveExtension("json");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void HaveExtension_WhenMismatch_ShouldThrow()
    {
        var path = CreateTempFilePath(".txt");
        File.WriteAllText(path, "x");
        try
        {
            Xunit.Assert.Throws<OmniAssertionException>(() => (path).FileExists().HaveExtension(".json"));
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static string CreateTempFilePath(string extension = ".tmp") =>
        Path.Combine(Path.GetTempPath(), $"omniassert-{Guid.NewGuid():N}{extension}");
}
