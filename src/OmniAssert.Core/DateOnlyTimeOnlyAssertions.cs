using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>Assertions for <see cref="DateOnly"/> subjects from <see cref="Assert.Verify(DateOnly, string?)"/>.</summary>
public readonly struct DateOnlyAssertions
{
    private readonly DateOnly _actual;
    private readonly string _expression;

    internal DateOnlyAssertions(DateOnly actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    /// <summary>Verifies that the date is equal to <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected date value.</param>
    /// <param name="expectedExpression">The expression for the expected date (automatically captured).</param>
    public void ToBe(DateOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual == expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date is before <paramref name="expected"/>.</summary>
    /// <param name="expected">The date that the actual value must be earlier than.</param>
    /// <param name="expectedExpression">The expression for the expected date (automatically captured).</param>
    public void ToBeBefore(DateOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual < expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be before", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date is after <paramref name="expected"/>.</summary>
    /// <param name="expected">The date that the actual value must be later than.</param>
    /// <param name="expectedExpression">The expression for the expected date (automatically captured).</param>
    public void ToBeAfter(DateOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual > expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be after", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date year matches <paramref name="expectedYear"/>.</summary>
    /// <param name="expectedYear">The expected year component.</param>
    /// <param name="yearExpression">The expression for the expected year (automatically captured).</param>
    public void HasYear(int expectedYear, [CallerArgumentExpression(nameof(expectedYear))] string? yearExpression = null)
    {
        if (_actual.Year == expectedYear)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have year {yearExpression ?? "expectedYear"} ({expectedYear}), but was {_actual.Year}.",
            _expression);
    }

    /// <summary>Verifies that the date month matches <paramref name="expectedMonth"/>.</summary>
    /// <param name="expectedMonth">The expected month component.</param>
    /// <param name="monthExpression">The expression for the expected month (automatically captured).</param>
    public void HasMonth(int expectedMonth, [CallerArgumentExpression(nameof(expectedMonth))] string? monthExpression = null)
    {
        if (_actual.Month == expectedMonth)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have month {monthExpression ?? "expectedMonth"} ({expectedMonth}), but was {_actual.Month}.",
            _expression);
    }

    /// <summary>Verifies that the date day matches <paramref name="expectedDay"/>.</summary>
    /// <param name="expectedDay">The expected day component.</param>
    /// <param name="dayExpression">The expression for the expected day (automatically captured).</param>
    public void HasDay(int expectedDay, [CallerArgumentExpression(nameof(expectedDay))] string? dayExpression = null)
    {
        if (_actual.Day == expectedDay)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have day {dayExpression ?? "expectedDay"} ({expectedDay}), but was {_actual.Day}.",
            _expression);
    }

    private static string FormatDate(DateOnly value) => value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

    private static string FormatFailure(string relation, DateOnly expected, string expectedLabel, DateOnly actual, string actualLabel)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Verification failed.");
        sb.Append(AnsiColour.Expected($"Expected {actualLabel} {relation} {expectedLabel}: "));
        sb.AppendLine(AnsiColour.Expected(FormatDate(expected)));
        sb.Append(AnsiColour.Actual($"Got {actualLabel}: "));
        sb.Append(AnsiColour.Actual(FormatDate(actual)));
        return sb.ToString();
    }
}

/// <summary>Assertions for <see cref="TimeOnly"/> subjects from <see cref="Assert.Verify(TimeOnly, string?)"/>.</summary>
public readonly struct TimeOnlyAssertions
{
    private readonly TimeOnly _actual;
    private readonly string _expression;

    internal TimeOnlyAssertions(TimeOnly actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    /// <summary>Verifies that the time is equal to <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected time value.</param>
    /// <param name="expectedExpression">The expression for the expected time (automatically captured).</param>
    public void ToBe(TimeOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual == expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the time is before <paramref name="expected"/>.</summary>
    /// <param name="expected">The time that the actual value must be earlier than.</param>
    /// <param name="expectedExpression">The expression for the expected time (automatically captured).</param>
    public void ToBeBefore(TimeOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual < expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be before", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the time is after <paramref name="expected"/>.</summary>
    /// <param name="expected">The time that the actual value must be later than.</param>
    /// <param name="expectedExpression">The expression for the expected time (automatically captured).</param>
    public void ToBeAfter(TimeOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual > expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be after", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    private static string FormatTime(TimeOnly value) => value.ToString("HH:mm:ss.fffffff", System.Globalization.CultureInfo.InvariantCulture);

    private static string FormatFailure(string relation, TimeOnly expected, string expectedLabel, TimeOnly actual, string actualLabel)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Verification failed.");
        sb.Append(AnsiColour.Expected($"Expected {actualLabel} {relation} {expectedLabel}: "));
        sb.AppendLine(AnsiColour.Expected(FormatTime(expected)));
        sb.Append(AnsiColour.Actual($"Got {actualLabel}: "));
        sb.Append(AnsiColour.Actual(FormatTime(actual)));
        return sb.ToString();
    }
}
