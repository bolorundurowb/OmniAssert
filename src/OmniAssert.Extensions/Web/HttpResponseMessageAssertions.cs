using System.Net;
using OmniAssert.Extensions.Internal;

namespace OmniAssert.Extensions.Web;

/// <summary>Assertions for <see cref="HttpResponseMessage"/> subjects from <see cref="HttpResponseMessageExtensions.Must"/>.</summary>
public readonly struct HttpResponseMessageAssertions
{
    private readonly HttpResponseMessage _actual;
    private readonly string _expression;

    internal HttpResponseMessageAssertions(HttpResponseMessage actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    /// <summary>Verifies that the response status code is in the success range (<c>200</c>–<c>299</c>).</summary>
    public void BeSuccessStatusCode()
    {
        var code = (int)_actual.StatusCode;
        if (code >= 200 && code <= 299)
            return;

        VerificationFlow.Fail(
            FailureMessages.ExpectedGot(_expression, $"to be a success status code (2xx), but was {code}", null),
            _expression);
    }

    /// <summary>Verifies that the response status code equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected HTTP status code.</param>
    public void BeHttpStatusCode(int expected)
    {
        var code = (int)_actual.StatusCode;
        if (code == expected)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have status code {expected}, but was {code}.",
            _expression);
    }

    /// <summary>
    /// Verifies that the <c>Content-Type</c> media type equals <paramref name="expected"/> (case-insensitive;
    /// charset and other parameters are ignored).
    /// </summary>
    /// <param name="expected">The expected media type (for example <c>application/json</c>).</param>
    public void HaveContentType(string expected)
    {
        if (_actual.Content?.Headers.ContentType is null)
        {
            VerificationFlow.Fail(
                FailureMessages.ExpectedOnly(_expression, $"to have Content-Type {expected}, but no Content-Type header was present"),
                _expression);
            return;
        }

        var mediaType = _actual.Content.Headers.ContentType.MediaType;
        if (string.Equals(mediaType, expected, StringComparison.OrdinalIgnoreCase))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have Content-Type {expected}, but was {mediaType}.",
            _expression);
    }
}

/// <summary>Entry points for asserting on <see cref="HttpResponseMessage"/> instances.</summary>
public static class HttpResponseMessageExtensions
{
    /// <summary>Begins a fluent assertion chain on an <see cref="HttpResponseMessage"/>.</summary>
    /// <param name="actual">The HTTP response under verification.</param>
    /// <param name="expression">The expression for <paramref name="actual"/> (automatically captured).</param>
    /// <returns>Assertions for the response.</returns>
    public static HttpResponseMessageAssertions Must(this HttpResponseMessage actual,
        [System.Runtime.CompilerServices.CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");
}
