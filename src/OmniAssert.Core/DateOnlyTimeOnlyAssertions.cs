using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>Assertions for <see cref="DateOnly"/> subjects from <see cref="Ensure.Must(DateOnly, string?)"/>.</summary>
public readonly struct DateOnlyAssertions : IAssertionContext<DateOnly>
{
    private readonly DateOnly _actual;
    private readonly string _expression;

    internal DateOnlyAssertions(DateOnly actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    DateOnly IAssertionContext<DateOnly>.Subject => _actual;
    string IAssertionContext<DateOnly>.Expression => _expression;

    /// <summary>Verifies that the date is equal to <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected date value.</param>
    /// <param name="expectedExpression">The expression for the expected date (automatically captured).</param>
    public void Be(DateOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual == expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date is before <paramref name="expected"/>.</summary>
    /// <param name="expected">The date that the actual value must be earlier than.</param>
    /// <param name="expectedExpression">The expression for the expected date (automatically captured).</param>
    public void BeBefore(DateOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual < expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be before", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date is after <paramref name="expected"/>.</summary>
    /// <param name="expected">The date that the actual value must be later than.</param>
    /// <param name="expectedExpression">The expression for the expected date (automatically captured).</param>
    public void BeAfter(DateOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual > expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be after", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date year matches <paramref name="expectedYear"/>.</summary>
    /// <param name="expectedYear">The expected year component.</param>
    /// <param name="yearExpression">The expression for the expected year (automatically captured).</param>
    public void HaveYear(int expectedYear, [CallerArgumentExpression(nameof(expectedYear))] string? yearExpression = null)
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
    public void HaveMonth(int expectedMonth, [CallerArgumentExpression(nameof(expectedMonth))] string? monthExpression = null)
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
    public void HaveDay(int expectedDay, [CallerArgumentExpression(nameof(expectedDay))] string? dayExpression = null)
    {
        if (_actual.Day == expectedDay)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have day {dayExpression ?? "expectedDay"} ({expectedDay}), but was {_actual.Day}.",
            _expression);
    }

    /// <summary>Verifies that the date is today's date according to the local system clock (<see cref="DateTime.Today"/>).</summary>
    /// <remarks>Uses the local clock, so results can shift across midnight or time-zone boundaries.</remarks>
    public void BeToday()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        if (_actual == today)
            return;

        VerificationFlow.Fail(FormatSingle($"to be today ({FormatDate(today)})", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date is yesterday's date according to the local system clock.</summary>
    /// <remarks>Uses the local clock, so results can shift across midnight or time-zone boundaries.</remarks>
    public void BeYesterday()
    {
        var yesterday = DateOnly.FromDateTime(DateTime.Today).AddDays(-1);
        if (_actual == yesterday)
            return;

        VerificationFlow.Fail(FormatSingle($"to be yesterday ({FormatDate(yesterday)})", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date is tomorrow's date according to the local system clock.</summary>
    /// <remarks>Uses the local clock, so results can shift across midnight or time-zone boundaries.</remarks>
    public void BeTomorrow()
    {
        var tomorrow = DateOnly.FromDateTime(DateTime.Today).AddDays(1);
        if (_actual == tomorrow)
            return;

        VerificationFlow.Fail(FormatSingle($"to be tomorrow ({FormatDate(tomorrow)})", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date falls on a weekday (Monday through Friday).</summary>
    public void BeWeekday()
    {
        var day = _actual.DayOfWeek;
        if (day != DayOfWeek.Saturday && day != DayOfWeek.Sunday)
            return;

        VerificationFlow.Fail(FormatSingle($"to be a weekday, but was {day}", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date falls on a weekend (Saturday or Sunday).</summary>
    public void BeWeekend()
    {
        var day = _actual.DayOfWeek;
        if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
            return;

        VerificationFlow.Fail(FormatSingle($"to be a weekend, but was {day}", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date's year is a leap year.</summary>
    public void BeLeapYear()
    {
        if (DateTime.IsLeapYear(_actual.Year))
            return;

        VerificationFlow.Fail(FormatSingle($"to fall in a leap year, but {_actual.Year} is not", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the date is within <paramref name="days"/> days (inclusive) of <paramref name="anchor"/>.</summary>
    /// <param name="days">The maximum allowed difference in days (non-negative).</param>
    /// <param name="anchor">The reference date.</param>
    /// <param name="anchorExpression">The expression for the anchor (automatically captured).</param>
    public void BeWithinDays(int days, DateOnly anchor, [CallerArgumentExpression(nameof(anchor))] string? anchorExpression = null)
    {
        if (days < 0)
        {
            VerificationFlow.Fail(
                $"Verification failed: expected days to be non-negative, but was {days}.",
                _expression);
            return;
        }

        if (Math.Abs(_actual.DayNumber - anchor.DayNumber) <= days)
            return;

        VerificationFlow.Fail(
            FormatSingle($"to be within {days} day(s) of {anchorExpression ?? "anchor"} ({FormatDate(anchor)})", _actual, _expression),
            _expression);
    }

    private static string FormatDate(DateOnly value) => value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

    private static string FormatSingle(string expectedDesc, DateOnly actual, string actualLabel)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Verification failed.");
        sb.Append(AnsiColour.Expected($"Expected {actualLabel} {expectedDesc}."));
        sb.AppendLine();
        sb.Append(AnsiColour.Actual($"Got {actualLabel}: "));
        sb.Append(AnsiColour.Actual(FormatDate(actual)));
        return sb.ToString();
    }

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
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBe(DateOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null) => Be(expected, expectedExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeBefore(DateOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null) => BeBefore(expected, expectedExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeAfter(DateOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null) => BeAfter(expected, expectedExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToHaveYear(int expectedYear, [CallerArgumentExpression(nameof(expectedYear))] string? yearExpression = null) => HaveYear(expectedYear, yearExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToHaveMonth(int expectedMonth, [CallerArgumentExpression(nameof(expectedMonth))] string? monthExpression = null) => HaveMonth(expectedMonth, monthExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToHaveDay(int expectedDay, [CallerArgumentExpression(nameof(expectedDay))] string? dayExpression = null) => HaveDay(expectedDay, dayExpression);
}

/// <summary>Assertions for <see cref="TimeOnly"/> subjects from <see cref="Ensure.Must(TimeOnly, string?)"/>.</summary>
public readonly struct TimeOnlyAssertions : IAssertionContext<TimeOnly>
{
    private readonly TimeOnly _actual;
    private readonly string _expression;

    internal TimeOnlyAssertions(TimeOnly actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    TimeOnly IAssertionContext<TimeOnly>.Subject => _actual;
    string IAssertionContext<TimeOnly>.Expression => _expression;

    /// <summary>Verifies that the time is equal to <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected time value.</param>
    /// <param name="expectedExpression">The expression for the expected time (automatically captured).</param>
    public void Be(TimeOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual == expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the time is before <paramref name="expected"/>.</summary>
    /// <param name="expected">The time that the actual value must be earlier than.</param>
    /// <param name="expectedExpression">The expression for the expected time (automatically captured).</param>
    public void BeBefore(TimeOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual < expected)
            return;

        VerificationFlow.Fail(FormatFailure("to be before", expected, expectedExpression ?? "expected", _actual, _expression), _expression);
    }

    /// <summary>Verifies that the time is after <paramref name="expected"/>.</summary>
    /// <param name="expected">The time that the actual value must be later than.</param>
    /// <param name="expectedExpression">The expression for the expected time (automatically captured).</param>
    public void BeAfter(TimeOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
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
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBe(TimeOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null) => Be(expected, expectedExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeBefore(TimeOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null) => BeBefore(expected, expectedExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeAfter(TimeOnly expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null) => BeAfter(expected, expectedExpression);
}
