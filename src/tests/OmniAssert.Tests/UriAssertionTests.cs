using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class UriAssertionTests
{
    [Fact]
    public void HaveScheme_WhenMatching_ShouldSucceed()
    {
        Verify(new Uri("https://api.example.com/v1/users?id=123")).HaveScheme("https");
    }

    [Fact]
    public void HaveScheme_WhenDifferent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new Uri("http://api.example.com/v1/users?id=123")).HaveScheme("https"));
    }

    [Fact]
    public void HaveHost_WhenMatching_ShouldSucceed()
    {
        Verify(new Uri("https://api.example.com/v1/users?id=123")).HaveHost("api.example.com");
    }

    [Fact]
    public void HaveHost_WhenDifferent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new Uri("https://example.com/v1/users?id=123")).HaveHost("api.example.com"));
    }

    [Fact]
    public void HavePath_WhenMatching_ShouldSucceed()
    {
        Verify(new Uri("https://api.example.com/v1/users?id=123")).HavePath("/v1/users");
    }

    [Fact]
    public void HavePath_WhenDifferent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new Uri("https://api.example.com/v2/users?id=123")).HavePath("/v1/users"));
    }

    [Fact]
    public void HaveQuery_WhenMatchingWithoutQuestionPrefix_ShouldSucceed()
    {
        Verify(new Uri("https://api.example.com/v1/users?id=123")).HaveQuery("id=123");
    }

    [Fact]
    public void HaveQuery_WhenMatchingWithQuestionPrefix_ShouldSucceed()
    {
        Verify(new Uri("https://api.example.com/v1/users?id=123")).HaveQuery("?id=123");
    }

    [Fact]
    public void HaveQuery_WhenDifferent_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Verify(new Uri("https://api.example.com/v1/users?id=456")).HaveQuery("id=123"));
    }

    [Fact]
    public void UriAssertions_WhenUriIsNull_ShouldThrowOnPropertyAssertions()
    {
        Uri? uri = null;
        Xunit.Assert.Throws<OmniAssertionException>(() => Verify(uri).HaveHost("api.example.com"));
    }
}
