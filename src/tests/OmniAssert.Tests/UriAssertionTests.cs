namespace OmniAssert.Tests;

public class UriAssertionTests
{
    [Fact]
    public void Be_WhenUrisMatch_ShouldSucceed()
    {
        var uri = new Uri("https://example.com/path");
        uri.Must().Be(uri);
    }

    [Fact]
    public void Be_WhenUrisDiffer_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Uri("https://example.com/a")).Must().Be(new Uri("https://example.com/b")));
    }

    [Fact]
    public void HaveScheme_WhenMatching_ShouldSucceed()
    {
        (new Uri("https://api.example.com/v1/users?id=123")).Must().HaveScheme("https");
    }

    [Fact]
    public void HaveScheme_WhenDifferent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Uri("http://api.example.com/v1/users?id=123")).Must().HaveScheme("https"));
    }

    [Fact]
    public void HaveHost_WhenMatching_ShouldSucceed()
    {
        (new Uri("https://api.example.com/v1/users?id=123")).Must().HaveHost("api.example.com");
    }

    [Fact]
    public void HaveHost_WhenDifferent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Uri("https://example.com/v1/users?id=123")).Must().HaveHost("api.example.com"));
    }

    [Fact]
    public void HavePath_WhenMatching_ShouldSucceed()
    {
        (new Uri("https://api.example.com/v1/users?id=123")).Must().HavePath("/v1/users");
    }

    [Fact]
    public void HavePath_WhenDifferent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Uri("https://api.example.com/v2/users?id=123")).Must().HavePath("/v1/users"));
    }

    [Fact]
    public void HaveQuery_WhenMatchingWithoutQuestionPrefix_ShouldSucceed()
    {
        (new Uri("https://api.example.com/v1/users?id=123")).Must().HaveQuery("id=123");
    }

    [Fact]
    public void HaveQuery_WhenMatchingWithQuestionPrefix_ShouldSucceed()
    {
        (new Uri("https://api.example.com/v1/users?id=123")).Must().HaveQuery("?id=123");
    }

    [Fact]
    public void HaveQuery_WhenDifferent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new Uri("https://api.example.com/v1/users?id=456")).Must().HaveQuery("id=123"));
    }

    [Fact]
    public void UriAssertions_WhenUriIsNull_ShouldThrowOnPropertyAssertions()
    {
        Uri? uri = null;
        Xunit.Assert.Throws<OmniAssertionException>(() => (uri).Must().HaveHost("api.example.com"));
    }

    [Fact]
    public void HaveHost_WithSubdomain_ShouldSucceed()
    {
        (new Uri("https://sub.example.com/path")).Must().HaveHost("sub.example.com");
    }

    [Fact]
    public void HavePath_WithMultipleLevels_ShouldSucceed()
    {
        (new Uri("https://example.com/v1/users/123/profile")).Must().HavePath("/v1/users/123/profile");
    }

    [Fact]
    public void HavePath_WithEmptyPath_ShouldSucceed()
    {
        (new Uri("https://example.com")).Must().HavePath("/");
    }

    [Fact]
    public void HaveQuery_WhenEmpty_ShouldSucceed()
    {
        (new Uri("https://api.example.com/v1/users")).Must().HaveQuery("");
    }

    [Fact]
    public void HaveQuery_WithMultipleParameters_ShouldSucceed()
    {
        (new Uri("https://api.example.com/search?q=test&limit=10&sort=name")).Must()
            .HaveQuery("q=test&limit=10&sort=name");
    }

    [Fact]
    public void HaveScheme_CaseSensitive_ShouldSucceed()
    {
        (new Uri("https://example.com")).Must().HaveScheme("https");
    }

    [Fact]
    public void HaveScheme_WithFileScheme_ShouldSucceed()
    {
        (new Uri("file:///C:/temp/file.txt")).Must().HaveScheme("file");
    }
}
