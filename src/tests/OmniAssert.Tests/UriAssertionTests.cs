namespace OmniAssert.Tests;

public class UriAssertionTests
{
    [Fact]
    public void HaveScheme_WhenMatching_ShouldSucceed()
    {
        (new Uri("https://api.example.com/v1/users?id=123")).Verify().HaveScheme("https");
    }

    [Fact]
    public void HaveScheme_WhenDifferent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Uri("http://api.example.com/v1/users?id=123")).Verify().HaveScheme("https"));
    }

    [Fact]
    public void HaveHost_WhenMatching_ShouldSucceed()
    {
        (new Uri("https://api.example.com/v1/users?id=123")).Verify().HaveHost("api.example.com");
    }

    [Fact]
    public void HaveHost_WhenDifferent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Uri("https://example.com/v1/users?id=123")).Verify().HaveHost("api.example.com"));
    }

    [Fact]
    public void HavePath_WhenMatching_ShouldSucceed()
    {
        (new Uri("https://api.example.com/v1/users?id=123")).Verify().HavePath("/v1/users");
    }

    [Fact]
    public void HavePath_WhenDifferent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Uri("https://api.example.com/v2/users?id=123")).Verify().HavePath("/v1/users"));
    }

    [Fact]
    public void HaveQuery_WhenMatchingWithoutQuestionPrefix_ShouldSucceed()
    {
        (new Uri("https://api.example.com/v1/users?id=123")).Verify().HaveQuery("id=123");
    }

    [Fact]
    public void HaveQuery_WhenMatchingWithQuestionPrefix_ShouldSucceed()
    {
        (new Uri("https://api.example.com/v1/users?id=123")).Verify().HaveQuery("?id=123");
    }

    [Fact]
    public void HaveQuery_WhenDifferent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Uri("https://api.example.com/v1/users?id=456")).Verify().HaveQuery("id=123"));
    }

    [Fact]
    public void UriAssertions_WhenUriIsNull_ShouldThrowOnPropertyAssertions()
    {
        Uri? uri = null;
        Xunit.Assert.Throws<OmniAssertionException>(() => (uri).Verify().HaveHost("api.example.com"));
    }

    [Fact]
    public void HaveHost_WithSubdomain_ShouldSucceed()
    {
        (new Uri("https://sub.example.com/path")).Verify().HaveHost("sub.example.com");
    }

    [Fact]
    public void HavePath_WithMultipleLevels_ShouldSucceed()
    {
        (new Uri("https://example.com/v1/users/123/profile")).Verify().HavePath("/v1/users/123/profile");
    }

    [Fact]
    public void HavePath_WithEmptyPath_ShouldSucceed()
    {
        (new Uri("https://example.com")).Verify().HavePath("/");
    }

    [Fact]
    public void HaveQuery_WhenEmpty_ShouldSucceed()
    {
        (new Uri("https://api.example.com/v1/users")).Verify().HaveQuery("");
    }

    [Fact]
    public void HaveQuery_WithMultipleParameters_ShouldSucceed()
    {
        (new Uri("https://api.example.com/search?q=test&limit=10&sort=name")).Verify()
            .HaveQuery("q=test&limit=10&sort=name");
    }

    [Fact]
    public void HaveScheme_CaseSensitive_ShouldSucceed()
    {
        (new Uri("https://example.com")).Verify().HaveScheme("https");
    }

    [Fact]
    public void HaveScheme_WithFileScheme_ShouldSucceed()
    {
        (new Uri("file:///C:/temp/file.txt")).Verify().HaveScheme("file");
    }
}
