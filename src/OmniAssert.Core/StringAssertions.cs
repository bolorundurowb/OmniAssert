using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace OmniAssert;

/// <summary>Provides assertions for string subjects.</summary>
public readonly struct StringAssertions
{
    private readonly string? _actual;
    private readonly string _expression;

    internal StringAssertions(string? actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    /// <summary>Verifies that the string is equal to the <paramref name="expected"/> string using ordinal comparison.</summary>
    /// <param name="expected">The expected string.</param>
    /// <param name="expectedExpression">The expression for the expected string (automatically captured).</param>
    public void ToBe(string? expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (string.Equals(_actual, expected, StringComparison.Ordinal))
            return;

        var msg = FormatStrings("to be", expected, expectedExpression ?? "expected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the string is not equal to the <paramref name="unexpected"/> string using ordinal comparison.</summary>
    /// <param name="unexpected">The unexpected string.</param>
    /// <param name="unexpectedExpression">The expression for the unexpected string (automatically captured).</param>
    public void NotToBe(string? unexpected, [CallerArgumentExpression(nameof(unexpected))] string? unexpectedExpression = null)
    {
        if (!string.Equals(_actual, unexpected, StringComparison.Ordinal))
            return;

        var msg = FormatStrings("not to be", unexpected, unexpectedExpression ?? "unexpected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the string contains the specified <paramref name="substring"/> using ordinal comparison.</summary>
    /// <param name="substring">The substring expected to be present.</param>
    /// <param name="substringExpression">The expression for the substring (automatically captured).</param>
    public void ToContain(string substring, [CallerArgumentExpression(nameof(substring))] string? substringExpression = null)
    {
        if (_actual is not null && substring.Length > 0 && _actual.Contains(substring, StringComparison.Ordinal))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to contain {substringExpression ?? "substring"} ({Quote(substring)}), but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string is empty.</summary>
    public void ToBeEmpty()
    {
        if (string.IsNullOrEmpty(_actual))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be empty, but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string is null or empty.</summary>
    public void ToBeNullOrEmpty()
    {
        if (string.IsNullOrEmpty(_actual))
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be null or empty, but was {Quote(_actual)}.", _expression);
    }

    /// <summary>Verifies that the string is null, empty, or consists only of white-space characters.</summary>
    public void ToBeNullOrWhiteSpace()
    {
        if (string.IsNullOrWhiteSpace(_actual))
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be null or white space, but was {Quote(_actual)}.", _expression);
    }

    /// <summary>Verifies that the string is null.</summary>
    public void ToBeNull()
    {
        if (_actual is null)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be null, but was {Quote(_actual)}.", _expression);
    }

    /// <summary>Verifies that the string is not null.</summary>
    public void NotToBeNull()
    {
        if (_actual is not null)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} not to be null.", _expression);
    }

    /// <summary>Verifies that the string starts with the specified <paramref name="prefix"/>.</summary>
    /// <param name="prefix">The expected prefix.</param>
    /// <param name="comparison">The string comparison culture/options.</param>
    /// <param name="prefixExpression">The expression for the prefix (automatically captured).</param>
    public void ToStartWith(string prefix, StringComparison comparison = StringComparison.Ordinal, [CallerArgumentExpression(nameof(prefix))] string? prefixExpression = null)
    {
        if (_actual is not null && _actual.StartsWith(prefix, comparison))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to start with {prefixExpression ?? "prefix"} ({Quote(prefix)}), but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string ends with the specified <paramref name="suffix"/>.</summary>
    /// <param name="suffix">The expected suffix.</param>
    /// <param name="comparison">The string comparison culture/options.</param>
    /// <param name="suffixExpression">The expression for the suffix (automatically captured).</param>
    public void ToEndWith(string suffix, StringComparison comparison = StringComparison.Ordinal, [CallerArgumentExpression(nameof(suffix))] string? suffixExpression = null)
    {
        if (_actual is not null && _actual.EndsWith(suffix, comparison))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to end with {suffixExpression ?? "suffix"} ({Quote(suffix)}), but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string matches the specified regular expression.</summary>
    /// <param name="regexPattern">The regular expression pattern.</param>
    /// <param name="options">The regex options.</param>
    /// <param name="patternExpression">The expression for the pattern (automatically captured).</param>
    public void ToMatch(string regexPattern, RegexOptions options = RegexOptions.None, [CallerArgumentExpression(nameof(regexPattern))] string? patternExpression = null)
    {
        if (_actual is not null && Regex.IsMatch(_actual, regexPattern, options))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to match regex {patternExpression ?? "pattern"} ({Quote(regexPattern)}), but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string is equal to the <paramref name="expected"/> string using the specified <paramref name="comparison"/>.</summary>
    /// <param name="expected">The expected string.</param>
    /// <param name="comparison">The string comparison culture/options.</param>
    /// <param name="expectedExpression">The expression for the expected string (automatically captured).</param>
    public void ToBe(string? expected, StringComparison comparison, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (string.Equals(_actual, expected, comparison))
            return;

        var msg = FormatStrings("to be", expected, expectedExpression ?? "expected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the string contains the specified <paramref name="substring"/> using the specified <paramref name="comparison"/>.</summary>
    /// <param name="substring">The substring expected to be present.</param>
    /// <param name="comparison">The string comparison culture/options.</param>
    /// <param name="substringExpression">The expression for the substring (automatically captured).</param>
    public void ToContain(string substring, StringComparison comparison, [CallerArgumentExpression(nameof(substring))] string? substringExpression = null)
    {
        if (_actual is not null && substring.Length > 0 && _actual.Contains(substring, comparison))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to contain {substringExpression ?? "substring"} ({Quote(substring)}), but was {Quote(_actual)}.",
            _expression);
    }

    private static string FormatStrings(string relation, string? expected, string expectedLabel, string? actual, string actualLabel)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Verification failed.");
        sb.Append(AnsiColour.Expected("Expected "));
        sb.Append(expectedLabel);
        sb.Append(": ");
        sb.AppendLine(AnsiColour.Expected(Quote(expected)));
        sb.Append(AnsiColour.Actual("Actual "));
        sb.Append(actualLabel);
        sb.Append(": ");
        sb.Append(AnsiColour.Actual(Quote(actual)));
        return sb.ToString();
    }

    private static string Quote(string? s) => s switch
    {
        null => "null",
        "" => "\"\"",
        _ => "\"" + s.Replace("\"", "\\\"", StringComparison.Ordinal) + "\""
    };
}
