using System.Runtime.CompilerServices;
using System.Text;

namespace OmniAssert;

public readonly struct IntAssertions
{
    private readonly int _actual;
    private readonly string _expression;

    internal IntAssertions(int actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    public void ToBe(int expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual == expected)
            return;

        var msg = FormatPair(expected, expectedExpression ?? "expected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    public void NotToBe(int unexpected, [CallerArgumentExpression(nameof(unexpected))] string? unexpectedExpression = null)
    {
        if (_actual != unexpected)
            return;

        var msg = FormatPair(unexpected, unexpectedExpression ?? "unexpected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    private static string FormatPair(int expected, string expectedLabel, int actual, string actualLabel)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Verification failed.");
        sb.Append(AnsiColour.Expected("Expected "));
        sb.Append(expectedLabel);
        sb.Append(": ");
        sb.AppendLine(AnsiColour.Expected(expected.ToString(System.Globalization.CultureInfo.InvariantCulture)));
        sb.Append(AnsiColour.Actual("Actual "));
        sb.Append(actualLabel);
        sb.Append(": ");
        sb.Append(AnsiColour.Actual(actual.ToString(System.Globalization.CultureInfo.InvariantCulture)));
        return sb.ToString();
    }
}
