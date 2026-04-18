using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace OmniAssert;

public readonly struct StringAssertions
{
    private readonly string? _actual;
    private readonly string _expression;

    internal StringAssertions(string? actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    public void ToBe(string? expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (string.Equals(_actual, expected, StringComparison.Ordinal))
            return;

        var msg = FormatStrings("to be", expected, expectedExpression ?? "expected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    public void NotToBe(string? unexpected, [CallerArgumentExpression(nameof(unexpected))] string? unexpectedExpression = null)
    {
        if (!string.Equals(_actual, unexpected, StringComparison.Ordinal))
            return;

        var msg = FormatStrings("not to be", unexpected, unexpectedExpression ?? "unexpected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    public void ToContain(string substring, [CallerArgumentExpression(nameof(substring))] string? substringExpression = null)
    {
        if (_actual is not null && substring.Length > 0 && _actual.Contains(substring, StringComparison.Ordinal))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to contain {substringExpression ?? "substring"} ({Quote(substring)}), but was {Quote(_actual)}.",
            _expression);
    }

    public void ToBeEmpty()
    {
        if (string.IsNullOrEmpty(_actual))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be empty, but was {Quote(_actual)}.",
            _expression);
    }

    public void ToBeNullOrEmpty()
    {
        if (string.IsNullOrEmpty(_actual))
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be null or empty, but was {Quote(_actual)}.", _expression);
    }

    public void ToBeNullOrWhiteSpace()
    {
        if (string.IsNullOrWhiteSpace(_actual))
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be null or white space, but was {Quote(_actual)}.", _expression);
    }

    public void ToBeNull()
    {
        if (_actual is null)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be null, but was {Quote(_actual)}.", _expression);
    }

    public void NotToBeNull()
    {
        if (_actual is not null)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} not to be null.", _expression);
    }

    public void ToStartWith(string prefix, StringComparison comparison = StringComparison.Ordinal, [CallerArgumentExpression(nameof(prefix))] string? prefixExpression = null)
    {
        if (_actual is not null && _actual.StartsWith(prefix, comparison))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to start with {prefixExpression ?? "prefix"} ({Quote(prefix)}), but was {Quote(_actual)}.",
            _expression);
    }

    public void ToEndWith(string suffix, StringComparison comparison = StringComparison.Ordinal, [CallerArgumentExpression(nameof(suffix))] string? suffixExpression = null)
    {
        if (_actual is not null && _actual.EndsWith(suffix, comparison))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to end with {suffixExpression ?? "suffix"} ({Quote(suffix)}), but was {Quote(_actual)}.",
            _expression);
    }

    public void ToMatch(string regexPattern, RegexOptions options = RegexOptions.None, [CallerArgumentExpression(nameof(regexPattern))] string? patternExpression = null)
    {
        if (_actual is not null && Regex.IsMatch(_actual, regexPattern, options))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to match regex {patternExpression ?? "pattern"} ({Quote(regexPattern)}), but was {Quote(_actual)}.",
            _expression);
    }

    public void ToBe(string? expected, StringComparison comparison, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (string.Equals(_actual, expected, comparison))
            return;

        var msg = FormatStrings("to be", expected, expectedExpression ?? "expected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

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
