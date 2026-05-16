namespace OmniAssert.Tests;

public class DirectoryAssertionTests
{
    [Fact]
    public void DirectoryExists_WhenDirectoryExists_ShouldSucceed()
    {
        var path = CreateTempDirectoryPath();
        Directory.CreateDirectory(path);
        try
        {
            (path).DirectoryExists();
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
        Xunit.Assert.Throws<OmniAssertionException>(() => (path).DirectoryExists());
    }

    [Fact]
    public void BeEmpty_WhenEmptyDirectory_ShouldSucceed()
    {
        var path = CreateTempDirectoryPath();
        Directory.CreateDirectory(path);
        try
        {
            (path).DirectoryExists().BeEmpty();
        }
        finally
        {
            Directory.Delete(path);
        }
    }

    [Fact]
    public void BeEmpty_WhenDirectoryHasEntry_ShouldThrow()
    {
        var path = CreateTempDirectoryPath();
        Directory.CreateDirectory(path);
        File.WriteAllText(Path.Combine(path, "item.txt"), "x");
        try
        {
            Xunit.Assert.Throws<OmniAssertionException>(() => (path).DirectoryExists().BeEmpty());
        }
        finally
        {
            Directory.Delete(path, recursive: true);
        }
    }

    [Fact]
    public void BeEmpty_WhenDirectoryMissingInsideScope_ShouldCollectAssertionFailures()
    {
        var path = CreateTempDirectoryPath();

        var ex = Xunit.Assert.Throws<AggregateException>(() =>
        {
            using var scope = new AssertionScope();
            (path).DirectoryExists().BeEmpty();
        });

        Xunit.Assert.Equal(2, ex.InnerExceptions.Count);
        foreach (var inner in ex.InnerExceptions)
            Xunit.Assert.IsType<OmniAssertionException>(inner);
    }

    private static string CreateTempDirectoryPath() =>
        Path.Combine(Path.GetTempPath(), $"omniassert-{Guid.NewGuid():N}");
}
