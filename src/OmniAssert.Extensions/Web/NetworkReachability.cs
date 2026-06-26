using System.Net;

namespace OmniAssert.Extensions.Web;

internal static class NetworkReachability
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

    /// <summary>Test hook: when set, reachability checks use this handler instead of the default client.</summary>
    internal static Func<HttpMessageHandler>? TestHandlerFactory { get; set; }

    internal static bool ShouldSkip =>
        Environment.GetEnvironmentVariable("OMNIASSERT_SKIP_NETWORK") == "1";

    internal static ReachabilityResult Check(string? value, TimeSpan? timeout = null) =>
        CheckAsync(value, timeout).GetAwaiter().GetResult();

    internal static async Task<ReachabilityResult> CheckAsync(string? value, TimeSpan? timeout = null)
    {
        if (ShouldSkip)
            return ReachabilityResult.Skipped;

        if (string.IsNullOrWhiteSpace(value))
            return ReachabilityResult.Failed("value was empty");

        var url = value.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                  || value.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            ? value
            : $"https://{value}";

        var effectiveTimeout = timeout ?? DefaultTimeout;

        try
        {
            using var client = CreateClient(effectiveTimeout);
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
            if (IsReachableStatus(response.StatusCode))
                return ReachabilityResult.Ok;

            response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
            if (IsReachableStatus(response.StatusCode))
                return ReachabilityResult.Ok;

            return ReachabilityResult.Failed($"status {(int)response.StatusCode}");
        }
        catch (Exception ex)
        {
            return ReachabilityResult.Failed(ex.Message);
        }
    }

    private static HttpClient CreateClient(TimeSpan timeout)
    {
        if (TestHandlerFactory is not null)
            return new HttpClient(TestHandlerFactory(), disposeHandler: true) { Timeout = timeout };

        return new HttpClient { Timeout = timeout };
    }

    private static bool IsReachableStatus(HttpStatusCode statusCode)
    {
        var code = (int)statusCode;
        return code >= 200 && code < 400;
    }
}

internal readonly struct ReachabilityResult
{
    private ReachabilityResult(bool skipped, bool success, string? failureDetail)
    {
        WasSkipped = skipped;
        Success = success;
        FailureDetail = failureDetail;
    }

    internal bool WasSkipped { get; }
    internal bool Success { get; }
    internal string? FailureDetail { get; }

    internal static ReachabilityResult Ok => new(false, true, null);
    internal static ReachabilityResult Skipped => new(true, true, null);
    internal static ReachabilityResult Failed(string detail) => new(false, false, detail);
}
