using System.Runtime.CompilerServices;
using System.Text;

namespace OmniAssert;

/// <summary>Assertions for enum subjects from <see cref="Ensure.Must{T}(T, string?)"/>.</summary>
/// <typeparam name="T">Backed enum type.</typeparam>
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
    public void Be(T expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (EqualityComparer<T>.Default.Equals(_actual, expected))
            return;

        var msg = FormatPair(expected, expectedExpression ?? "expected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the enum value is not equal to the <paramref name="unexpected"/> value.</summary>
    /// <param name="unexpected">The unexpected enum value.</param>
    /// <param name="unexpectedExpression">The expression for the unexpected value (automatically captured).</param>
    public void NotBe(T unexpected, [CallerArgumentExpression(nameof(unexpected))] string? unexpectedExpression = null)
    {
        if (!EqualityComparer<T>.Default.Equals(_actual, unexpected))
            return;

        var msg = FormatPair(unexpected, unexpectedExpression ?? "unexpected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the enum value matches one of the provided <paramref name="expected"/> values.</summary>
    /// <param name="expected">Allowed enum values.</param>
    public void BeOneOf(params T[] expected)
    {
        if (expected is not null)
        {
            foreach (var candidate in expected)
            {
                if (EqualityComparer<T>.Default.Equals(_actual, candidate))
                    return;
            }
        }

        var expectedList = expected is null || expected.Length == 0
            ? "[]"
            : $"[{string.Join(", ", expected.Select(v => v.ToString()))}]";
        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be one of {expectedList}, but was {_actual}.",
            _expression);
    }

    private static string FormatPair(T expected, string expectedLabel, T actual, string actualLabel)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Verification failed.");
        sb.Append(AnsiColour.Expected("Expected "));
        sb.Append(expectedLabel);
        sb.Append(": ");
        sb.AppendLine(AnsiColour.Expected(expected.ToString()));
        sb.Append(AnsiColour.Actual("Got "));
        sb.Append(actualLabel);
        sb.Append(": ");
        sb.Append(AnsiColour.Actual(actual.ToString()));
        return sb.ToString();
    }
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBe(T expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null) => Be(expected, expectedExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void NotToBe(T unexpected, [CallerArgumentExpression(nameof(unexpected))] string? unexpectedExpression = null) => NotBe(unexpected, unexpectedExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeOneOf(params T[] expected) => BeOneOf(expected);
}
