using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>Assertions for <see cref="TimeSpan"/> subjects from <see cref="Assert.Verify(TimeSpan, string?)"/>.</summary>
public readonly struct TimeSpanAssertions
{
    private readonly TimeSpan _actual;
    private readonly string _expression;

    internal TimeSpanAssertions(TimeSpan actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    public void ToBe(TimeSpan expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual == expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the timespan is strictly positive.</summary>
    public void ToBePositive()
    {
        if (_actual > TimeSpan.Zero)
            return;

        VerificationFlow.Fail(FormatSingle("to be positive", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the timespan is strictly negative.</summary>
    public void ToBeNegative()
    {
        if (_actual < TimeSpan.Zero)
            return;

        VerificationFlow.Fail(FormatSingle("to be negative", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the timespan is greater than <paramref name="expected"/>.</summary>
    /// <param name="expected">The lower bound that actual duration must exceed.</param>
    /// <param name="expectedExpression">The expression for the expected timespan (automatically captured).</param>
    public void ToBeGreaterThan(TimeSpan expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual > expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be greater than", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the timespan is less than <paramref name="expected"/>.</summary>
    /// <param name="expected">The upper bound that actual duration must be below.</param>
    /// <param name="expectedExpression">The expression for the expected timespan (automatically captured).</param>
    public void ToBeLessThan(TimeSpan expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual < expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be less than", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the timespan is not equal to <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The value the timespan must not equal.</param>
    /// <param name="unexpectedExpression">The expression for the unexpected timespan (automatically captured).</param>
    public void NotToBe(TimeSpan unexpected, [CallerArgumentExpression(nameof(unexpected))] string? unexpectedExpression = null)
    {
        if (_actual != unexpected)
            return;

        VerificationFlow.Fail(FormatFailure("not to be", unexpected, unexpectedExpression ?? "unexpected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the timespan is greater than or equal to <paramref name="expected"/>.</summary>
    /// <param name="expected">The lower bound (inclusive) that actual duration must meet or exceed.</param>
    /// <param name="expectedExpression">The expression for the expected timespan (automatically captured).</param>
    public void ToBeGreaterThanOrEqualTo(TimeSpan expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual >= expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be greater than or equal to", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the timespan is less than or equal to <paramref name="expected"/>.</summary>
    /// <param name="expected">The upper bound (inclusive) that actual duration must not exceed.</param>
    /// <param name="expectedExpression">The expression for the expected timespan (automatically captured).</param>
    public void ToBeLessThanOrEqualTo(TimeSpan expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual <= expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be less than or equal to", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the timespan is within the inclusive range [<paramref name="min"/>, <paramref name="max"/>].</summary>
    /// <param name="min">The minimum inclusive duration.</param>
    /// <param name="max">The maximum inclusive duration.</param>
    /// <param name="minExpression">The expression for the minimum value (automatically captured).</param>
    /// <param name="maxExpression">The expression for the maximum value (automatically captured).</param>
    public void ToBeInRange(TimeSpan min, TimeSpan max,
        [CallerArgumentExpression(nameof(min))] string? minExpression = null,
        [CallerArgumentExpression(nameof(max))] string? maxExpression = null)
    {
        if (_actual >= min && _actual <= max)
            return;

        var minLabel = minExpression ?? "min";
        var maxLabel = maxExpression ?? "max";
        var msg = $"Verification failed: expected {_expression} to be in range " +
                  $"[{minLabel}({FormatValue(min)}), {maxLabel}({FormatValue(max)})], " +
                  $"but was {FormatValue(_actual)}.";
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the timespan matches one of the provided expected values.</summary>
    /// <param name="expected">Allowed timespan values.</param>
    public void ToBeOneOf(params TimeSpan[] expected)
    {
        if (expected is not null)
        {
            foreach (var candidate in expected)
            {
                if (_actual == candidate)
                    return;
            }
        }

        var expectedList = expected is null || expected.Length == 0
            ? "[]"
            : $"[{string.Join(", ", expected.Select(FormatValue))}]";
        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be one of {expectedList}, but was {FormatValue(_actual)}.",
            _expression);
    }

    private static string FormatValue(TimeSpan value) => value.ToString("c", System.Globalization.CultureInfo.InvariantCulture);

    private static string FormatFailure(string relation, TimeSpan expected, string expectedLabel, TimeSpan actual, string actualLabel)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Verification failed.");
        sb.Append(AnsiColour.Expected($"Expected {actualLabel} {relation} {expectedLabel}: "));
        sb.AppendLine(AnsiColour.Expected(FormatValue(expected)));
        sb.Append(AnsiColour.Actual($"Got {actualLabel}: "));
        sb.Append(AnsiColour.Actual(FormatValue(actual)));
        return sb.ToString();
    }

    private static string FormatSingle(string expectedDesc, TimeSpan actual, string actualLabel)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Verification failed.");
        sb.Append(AnsiColour.Expected($"Expected {actualLabel} {expectedDesc}."));
        sb.AppendLine();
        sb.Append(AnsiColour.Actual($"Got {actualLabel}: "));
        sb.Append(AnsiColour.Actual(FormatValue(actual)));
        return sb.ToString();
    }
}
