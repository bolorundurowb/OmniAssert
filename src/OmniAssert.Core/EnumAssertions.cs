using System.Runtime.CompilerServices;
using System.Text;

namespace OmniAssert;

/// <summary>Provides assertions for enum subjects.</summary>
/// <typeparam name="T">The enum type.</typeparam>
public readonly struct EnumAssertions<T> where T : struct, Enum
{
    private readonly T _actual;
    private readonly string _expression;

    internal EnumAssertions(T actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    /// <summary>Verifies that the enum value is equal to the <paramref name="expected"/> value.</summary>
    /// <param name="expected">The expected enum value.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBe(T expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (EqualityComparer<T>.Default.Equals(_actual, expected))
            return;

        var msg = FormatPair(expected, expectedExpression ?? "expected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the enum value is not equal to the <paramref name="unexpected"/> value.</summary>
    /// <param name="unexpected">The unexpected enum value.</param>
    /// <param name="unexpectedExpression">The expression for the unexpected value (automatically captured).</param>
    public void NotToBe(T unexpected, [CallerArgumentExpression(nameof(unexpected))] string? unexpectedExpression = null)
    {
        if (!EqualityComparer<T>.Default.Equals(_actual, unexpected))
            return;

        var msg = FormatPair(unexpected, unexpectedExpression ?? "unexpected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    private static string FormatPair(T expected, string expectedLabel, T actual, string actualLabel)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Verification failed.");
        sb.Append(AnsiColour.Expected("Expected "));
        sb.Append(expectedLabel);
        sb.Append(": ");
        sb.AppendLine(AnsiColour.Expected(expected.ToString()));
        sb.Append(AnsiColour.Actual("Actual "));
        sb.Append(actualLabel);
        sb.Append(": ");
        sb.Append(AnsiColour.Actual(actual.ToString()));
        return sb.ToString();
    }
}
