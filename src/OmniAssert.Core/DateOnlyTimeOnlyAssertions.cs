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
    public void ToBe(DateOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual == expected)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be {expectedExpression ?? "expected"} ({FormatDate(expected)}), but was {FormatDate(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the date is before <paramref name="expected"/>.</summary>
    public void ToBeBefore(DateOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual < expected)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be before {expectedExpression ?? "expected"} ({FormatDate(expected)}), but was {FormatDate(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the date is after <paramref name="expected"/>.</summary>
    public void ToBeAfter(DateOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual > expected)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be after {expectedExpression ?? "expected"} ({FormatDate(expected)}), but was {FormatDate(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the date year is <paramref name="expectedYear"/>.</summary>
    public void HasYear(int expectedYear, [CallerArgumentExpression(nameof(expectedYear))] string? yearExpression = null)
    {
        if (_actual.Year == expectedYear)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have year {yearExpression ?? "expectedYear"} ({expectedYear}), but was {_actual.Year}.",
            _expression);
    }

    /// <summary>Verifies that the date month is <paramref name="expectedMonth"/>.</summary>
    public void HasMonth(int expectedMonth, [CallerArgumentExpression(nameof(expectedMonth))] string? monthExpression = null)
    {
        if (_actual.Month == expectedMonth)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have month {monthExpression ?? "expectedMonth"} ({expectedMonth}), but was {_actual.Month}.",
            _expression);
    }

    /// <summary>Verifies that the date day is <paramref name="expectedDay"/>.</summary>
    public void HasDay(int expectedDay, [CallerArgumentExpression(nameof(expectedDay))] string? dayExpression = null)
    {
        if (_actual.Day == expectedDay)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have day {dayExpression ?? "expectedDay"} ({expectedDay}), but was {_actual.Day}.",
            _expression);
    }

    private static string FormatDate(DateOnly value) => value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
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
    public void ToBe(TimeOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual == expected)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be {expectedExpression ?? "expected"} ({FormatTime(expected)}), but was {FormatTime(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the time is before <paramref name="expected"/>.</summary>
    public void ToBeBefore(TimeOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual < expected)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be before {expectedExpression ?? "expected"} ({FormatTime(expected)}), but was {FormatTime(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the time is after <paramref name="expected"/>.</summary>
    public void ToBeAfter(TimeOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual > expected)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be after {expectedExpression ?? "expected"} ({FormatTime(expected)}), but was {FormatTime(_actual)}.",
            _expression);
    }

    private static string FormatTime(TimeOnly value) => value.ToString("HH:mm:ss.fffffff", System.Globalization.CultureInfo.InvariantCulture);
}
