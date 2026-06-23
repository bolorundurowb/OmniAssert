using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>Assertions for <see cref="DateTime"/> (unspecified, UTC, or local per how the value was constructed).</summary>
public readonly struct DateTimeAssertions
{
    private readonly DateTime _actual;
    private readonly string _expression;

    internal DateTimeAssertions(DateTime actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    /// <summary>Verifies that the date/time is after the <paramref name="expected"/> date/time.</summary>
    /// <param name="expected">The date/time to compare against.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBeAfter(DateTime expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual > expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be after", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date/time is before the <paramref name="expected"/> date/time.</summary>
    /// <param name="expected">The date/time to compare against.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBeBefore(DateTime expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual < expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be before", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date/time is within the specified <paramref name="precision"/> of the <paramref name="expected"/> date/time.</summary>
    /// <param name="precision">The maximum allowed difference.</param>
    /// <param name="expected">The expected date/time.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBeWithin(TimeSpan precision, DateTime expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (Math.Abs((_actual - expected).Ticks) <= precision.Ticks)
            return;

        VerificationFlow.Fail(FormatFailure($"to be within {precision} of", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date/time is exactly equal to the <paramref name="expected"/> date/time.</summary>
    /// <param name="expected">The expected date/time.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBe(DateTime expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual == expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date/time is not equal to the <paramref name="unexpected"/> date/time.</summary>
    /// <param name="unexpected">The unexpected date/time.</param>
    /// <param name="unexpectedExpression">The expression for the unexpected value (automatically captured).</param>
    public void NotToBe(DateTime unexpected, [CallerArgumentExpression(nameof(unexpected))] string? unexpectedExpression = null)
    {
        if (_actual != unexpected)
            return;

        VerificationFlow.Fail(FormatFailure("not to be", unexpected, unexpectedExpression ?? "unexpected", _actual, _expression), _expression);
    }

    private static string FormatFailure(string relation, DateTime expected, string expectedLabel, DateTime actual, string actualLabel)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Verification failed.");
        sb.Append(AnsiColour.Expected($"Expected {actualLabel} {relation} {expectedLabel}: "));
        sb.AppendLine(AnsiColour.Expected(expected.ToString("O")));
        sb.Append(AnsiColour.Actual($"Got {actualLabel}: "));
        sb.Append(AnsiColour.Actual(actual.ToString("O")));
        return sb.ToString();
    }
}

/// <summary>Assertions for <see cref="DateTimeOffset"/> subjects (includes offset in comparisons).</summary>
public readonly struct DateTimeOffsetAssertions
{
    private readonly DateTimeOffset _actual;
    private readonly string _expression;

    internal DateTimeOffsetAssertions(DateTimeOffset actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    /// <summary>Verifies that the date/time offset is after the <paramref name="expected"/> date/time offset.</summary>
    /// <param name="expected">The date/time offset to compare against.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBeAfter(DateTimeOffset expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual > expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be after", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date/time offset is before the <paramref name="expected"/> date/time offset.</summary>
    /// <param name="expected">The date/time offset to compare against.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBeBefore(DateTimeOffset expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual < expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be before", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date/time offset is within the specified <paramref name="precision"/> of the <paramref name="expected"/> date/time offset.</summary>
    /// <param name="precision">The maximum allowed difference.</param>
    /// <param name="expected">The expected date/time offset.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBeWithin(TimeSpan precision, DateTimeOffset expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (Math.Abs((_actual - expected).Ticks) <= precision.Ticks)
            return;

        VerificationFlow.Fail(FormatFailure($"to be within {precision} of", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date/time offset is exactly equal to the <paramref name="expected"/> date/time offset.</summary>
    /// <param name="expected">The expected date/time offset.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBe(DateTimeOffset expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual == expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date/time offset is not equal to the <paramref name="unexpected"/> date/time offset.</summary>
    /// <param name="unexpected">The unexpected date/time offset.</param>
    /// <param name="unexpectedExpression">The expression for the unexpected value (automatically captured).</param>
    public void NotToBe(DateTimeOffset unexpected, [CallerArgumentExpression(nameof(unexpected))] string? unexpectedExpression = null)
    {
        if (_actual != unexpected)
            return;

        VerificationFlow.Fail(FormatFailure("not to be", unexpected, unexpectedExpression ?? "unexpected", _actual, _expression), _expression);
    }

    private static string FormatFailure(string relation, DateTimeOffset expected, string expectedLabel, DateTimeOffset actual, string actualLabel)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Verification failed.");
        sb.Append(AnsiColour.Expected($"Expected {actualLabel} {relation} {expectedLabel}: "));
        sb.AppendLine(AnsiColour.Expected(expected.ToString("O")));
        sb.Append(AnsiColour.Actual($"Got {actualLabel}: "));
        sb.Append(AnsiColour.Actual(actual.ToString("O")));
        return sb.ToString();
    }
}
