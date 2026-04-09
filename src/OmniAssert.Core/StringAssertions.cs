using System.Runtime.CompilerServices;
using System.Text;

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
