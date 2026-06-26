using System.Net;
using System.Net.Http;
using System.Text;
using OmniAssert.Extensions.Web;

namespace OmniAssert.Extensions.Tests;

public class WebAssertionTests
{
    [Fact]
    public void BeEmailAddress_ValidEmail_ShouldSucceed()
    {
        "user@example.com".Must().BeEmailAddress();
    }

    [Fact]
    public void BeEmailAddress_EmptyString_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "".Must().BeEmailAddress());
    }

    [Fact]
    public void BeEmailAddress_MissingDomain_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "a@".Must().BeEmailAddress());
    }

    [Fact]
    public void BeEmailAddress_MissingLocal_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "@b.com".Must().BeEmailAddress());
    }

    [Fact]
    public void BeEmailAddress_WithSpaces_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "a b@c.com".Must().BeEmailAddress());
    }

    [Fact]
    public void BeEmailAddress_NoDotInDomain_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "a@localhost".Must().BeEmailAddress());
    }

    [Fact]
    public void BeUrl_ValidHttpUrl_ShouldSucceed()
    {
        "https://example.com".Must().BeUrl();
    }

    [Fact]
    public void BeUrl_InvalidUrl_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "not-a-url".Must().BeUrl());
    }

    [Fact]
    public void BeUrl_FtpScheme_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "ftp://x.com".Must().BeUrl());
    }

    [Fact]
    public void BeAbsoluteUrl_Alias_ShouldSucceed()
    {
        "https://example.com/path".Must().BeAbsoluteUrl();
    }

    [Fact]
    public void BeRelativeUrl_ValidRelative_ShouldSucceed()
    {
        "/path/to/resource".Must().BeRelativeUrl();
    }

    [Fact]
    public void BeRelativeUrl_AbsoluteUrl_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "https://a.com".Must().BeRelativeUrl());
    }

    [Fact]
    public void BeRelativeUrl_NoLeadingSlash_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "relative/no-slash".Must().BeRelativeUrl());
    }

    [Fact]
    public void BeSlug_ValidSlug_ShouldSucceed()
    {
        "my-post-title".Must().BeSlug();
    }

    [Fact]
    public void BeSlug_Uppercase_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "My Post".Must().BeSlug());
    }

    [Fact]
    public void BeSlug_LeadingHyphen_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "--bad".Must().BeSlug());
    }

    [Fact]
    public void BeSlug_TrailingUnderscore_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "slug_".Must().BeSlug());
    }

    [Fact]
    public void BeJson_ValidJson_ShouldSucceed()
    {
        """{"key": "value"}""".Must().BeJson();
    }

    [Fact]
    public void BeJson_InvalidJson_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "{bad".Must().BeJson());
    }

    [Fact]
    public void BeJson_EmptyString_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "".Must().BeJson());
    }

    [Fact]
    public void BeXml_ValidXml_ShouldSucceed()
    {
        "<root><item>test</item></root>".Must().BeXml();
    }

    [Fact]
    public void BeXml_InvalidXml_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "<unclosed".Must().BeXml());
    }

    [Fact]
    public void BeHtml_ValidHtml_ShouldSucceed()
    {
        "<html><body><p>Hello</p></body></html>".Must().BeHtml();
    }

    [Fact]
    public void BeHtml_PlainText_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "plain text".Must().BeHtml());
    }

    [Fact]
    public void BeIpAddress_ValidIpv4_ShouldSucceed()
    {
        "192.168.1.1".Must().BeIpAddress();
    }

    [Fact]
    public void BeIpAddress_InvalidIp_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "999.999.1.1".Must().BeIpAddress());
    }

    [Fact]
    public void BeValidIpAddress_Alias_ShouldSucceed()
    {
        "::1".Must().BeValidIpAddress();
    }

    [Fact]
    public void BeIpv4_ValidIpv4_ShouldSucceed()
    {
        "10.0.0.1".Must().BeIpv4();
    }

    [Fact]
    public void BeIpv4_Ipv6String_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "::1".Must().BeIpv4());
    }

    [Fact]
    public void BeIpv6_ValidIpv6_ShouldSucceed()
    {
        "::1".Must().BeIpv6();
    }

    [Fact]
    public void BeIpv6_Ipv4String_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "192.168.1.1".Must().BeIpv6());
    }

    [Fact]
    public void BeValidHostname_ValidHostname_ShouldSucceed()
    {
        "example.com".Must().BeValidHostname();
    }

    [Fact]
    public void BeValidHostname_Empty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "".Must().BeValidHostname());
    }

    [Fact]
    public void BeValidHostname_OnlyHyphen_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "-".Must().BeValidHostname());
    }

    [Fact]
    public void BeValidMacAddress_ValidMac_ShouldSucceed()
    {
        "00:1A:2B:3C:4D:5E".Must().BeValidMacAddress();
    }

    [Fact]
    public void BeValidMacAddress_InvalidMac_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "00:1A:2B:3C".Must().BeValidMacAddress());
    }

    [Fact]
    public void BeMacAddress_Alias_ShouldSucceed()
    {
        "00-1A-2B-3C-4D-5E".Must().BeMacAddress();
    }

    [Fact]
    public void BeValidPort_ValidPort_ShouldSucceed()
    {
        8080.Must().BeValidPort();
    }

    [Fact]
    public void BeValidPort_Negative_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => (-1).Must().BeValidPort());
    }

    [Fact]
    public void BeValidPort_TooHigh_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => 65536.Must().BeValidPort());
    }

    [Fact]
    public void BeValidPort_BoundaryZero_ShouldSucceed()
    {
        0.Must().BeValidPort();
    }

    [Fact]
    public void BeValidPort_BoundaryMax_ShouldSucceed()
    {
        65535.Must().BeValidPort();
    }

    [Fact]
    public void BeSuccessStatusCode_200_ShouldSucceed()
    {
        200.Must().BeSuccessStatusCode();
    }

    [Fact]
    public void BeSuccessStatusCode_404_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => 404.Must().BeSuccessStatusCode());
    }

    [Fact]
    public void BeHttpStatusCode_ExactMatch_ShouldSucceed()
    {
        200.Must().BeHttpStatusCode(200);
    }

    [Fact]
    public void BeHttpStatusCode_Mismatch_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => 404.Must().BeHttpStatusCode(200));
    }

    [Fact]
    public void HttpResponseMessage_BeSuccessStatusCode_ShouldSucceed()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        HttpResponseMessageExtensions.Must(response).BeSuccessStatusCode();
    }

    [Fact]
    public void HttpResponseMessage_BeSuccessStatusCode_WhenError_ShouldThrow()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        Xunit.Assert.Throws<OmniAssertionException>(() => HttpResponseMessageExtensions.Must(response).BeSuccessStatusCode());
    }

    [Fact]
    public void HttpResponseMessage_HaveContentType_Matching_ShouldSucceed()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        };
        HttpResponseMessageExtensions.Must(response).HaveContentType("application/json");
    }

    [Fact]
    public void HttpResponseMessage_HaveContentType_Mismatch_ShouldThrow()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("hello", Encoding.UTF8, "text/plain")
        };
        Xunit.Assert.Throws<OmniAssertionException>(() => HttpResponseMessageExtensions.Must(response).HaveContentType("application/json"));
    }

    [Fact]
    public void BeReachable_WhenSkipEnvSet_ShouldSucceed()
    {
        Environment.SetEnvironmentVariable("OMNIASSERT_SKIP_NETWORK", "1");
        try
        {
            "https://example.com".Must().BeReachable();
        }
        finally
        {
            Environment.SetEnvironmentVariable("OMNIASSERT_SKIP_NETWORK", null);
        }
    }

    [Fact]
    public async Task BeReachableAsync_WhenSkipEnvSet_ShouldSucceed()
    {
        Environment.SetEnvironmentVariable("OMNIASSERT_SKIP_NETWORK", "1");
        try
        {
            await "https://example.com".Must().BeReachableAsync();
        }
        finally
        {
            Environment.SetEnvironmentVariable("OMNIASSERT_SKIP_NETWORK", null);
        }
    }

    [Fact]
    public void BeReachable_WithMockHandler_ReturningOk_ShouldSucceed()
    {
        NetworkReachability.TestHandlerFactory = () => new OkHttpMessageHandler();
        try
        {
            "https://example.com".Must().BeReachable();
        }
        finally
        {
            NetworkReachability.TestHandlerFactory = null;
        }
    }

    [Fact]
    public async Task BeReachableAsync_WithMockHandler_ReturningOk_ShouldSucceed()
    {
        NetworkReachability.TestHandlerFactory = () => new OkHttpMessageHandler();
        try
        {
            await "example.com".Must().BeReachableAsync();
        }
        finally
        {
            NetworkReachability.TestHandlerFactory = null;
        }
    }

    [Fact]
    public void BeReachable_WithMockHandler_ReturningError_ShouldThrow()
    {
        NetworkReachability.TestHandlerFactory = () => new StatusCodeHttpMessageHandler(HttpStatusCode.ServiceUnavailable);
        try
        {
            Xunit.Assert.Throws<OmniAssertionException>(() => "https://example.com".Must().BeReachable());
        }
        finally
        {
            NetworkReachability.TestHandlerFactory = null;
        }
    }

    private sealed class OkHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
    }

    private sealed class StatusCodeHttpMessageHandler(HttpStatusCode statusCode) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(statusCode));
    }
}
