using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>Assertions for <see cref="Uri"/> subjects from <see cref="Assert.Verify(Uri?, string?)"/>.</summary>
public readonly struct UriAssertions
{
    private readonly Uri? _actual;
    private readonly string _expression;

    internal UriAssertions(Uri? actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    /// <summary>Verifies that the URI is equal to the <paramref name="expected"/> URI.</summary>
    /// <param name="expected">The expected URI.</param>
    /// <param name="expectedExpression">The expression for the expected URI (automatically captured).</param>
    public void ToBe(Uri? expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (Equals(_actual, expected))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be {expectedExpression ?? "expected"} ({StringFormatter.Quote(expected?.ToString())}), but was {StringFormatter.Quote(_actual?.ToString())}.",
            _expression);
    }

    /// <summary>Verifies that the URI scheme matches <paramref name="expectedScheme"/>.</summary>
    /// <param name="expectedScheme">The expected URI scheme, such as "https".</param>
    /// <param name="schemeExpression">The expression for the expected scheme (automatically captured).</param>
    public void HaveScheme(string expectedScheme, [CallerArgumentExpression(nameof(expectedScheme))] string? schemeExpression = null)
    {
        if (_actual is not null && string.Equals(_actual.Scheme, expectedScheme, StringComparison.OrdinalIgnoreCase))
            return;

        var actualScheme = _actual is null ? null : _actual.Scheme;
        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have scheme {schemeExpression ?? "expectedScheme"} ({StringFormatter.Quote(expectedScheme)}), but was {StringFormatter.Quote(actualScheme)}.",
            _expression);
    }

    /// <summary>Verifies that the URI host matches <paramref name="expectedHost"/>.</summary>
    /// <param name="expectedHost">The expected URI host, such as "api.example.com".</param>
    /// <param name="hostExpression">The expression for the expected host (automatically captured).</param>
    public void HaveHost(string expectedHost, [CallerArgumentExpression(nameof(expectedHost))] string? hostExpression = null)
    {
        if (_actual is not null && string.Equals(_actual.Host, expectedHost, StringComparison.OrdinalIgnoreCase))
            return;

        var actualHost = _actual is null ? null : _actual.Host;
        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have host {hostExpression ?? "expectedHost"} ({StringFormatter.Quote(expectedHost)}), but was {StringFormatter.Quote(actualHost)}.",
            _expression);
    }

    /// <summary>Verifies that the URI absolute path matches <paramref name="expectedPath"/>.</summary>
    /// <param name="expectedPath">The expected absolute path, such as "/v1/users".</param>
    /// <param name="pathExpression">The expression for the expected path (automatically captured).</param>
    public void HavePath(string expectedPath, [CallerArgumentExpression(nameof(expectedPath))] string? pathExpression = null)
    {
        if (_actual is not null && string.Equals(_actual.AbsolutePath, expectedPath, StringComparison.Ordinal))
            return;

        var actualPath = _actual is null ? null : _actual.AbsolutePath;
        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have path {pathExpression ?? "expectedPath"} ({StringFormatter.Quote(expectedPath)}), but was {StringFormatter.Quote(actualPath)}.",
            _expression);
    }

    /// <summary>Verifies that the URI query string matches <paramref name="expectedQuery"/>.</summary>
    /// <param name="expectedQuery">The expected query string with or without leading '?', such as "id=123".</param>
    /// <param name="queryExpression">The expression for the expected query (automatically captured).</param>
    public void HaveQuery(string expectedQuery, [CallerArgumentExpression(nameof(expectedQuery))] string? queryExpression = null)
    {
        var normalizedExpected = NormalizeQuery(expectedQuery);
        var actualQuery = _actual is null ? null : NormalizeQuery(_actual.Query);
        if (_actual is not null && string.Equals(actualQuery, normalizedExpected, StringComparison.Ordinal))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have query {queryExpression ?? "expectedQuery"} ({StringFormatter.Quote(normalizedExpected)}), but was {StringFormatter.Quote(actualQuery)}.",
            _expression);
    }

    private static string NormalizeQuery(string query) =>
        query.StartsWith('?') ? query[1..] : query;
}
