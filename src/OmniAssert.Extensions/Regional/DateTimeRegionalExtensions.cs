using OmniAssert.Extensions.Internal;

namespace OmniAssert.Extensions.Regional;

/// <summary>Regional date and time business-rule assertions.</summary>
public static class DateTimeRegionalExtensions
{
    /// <summary>
    /// Verifies that the date of birth represents someone at least <paramref name="age"/> years old, computed against
    /// the UTC calendar date.
    /// </summary>
    /// <param name="assertions">The <see cref="DateTimeAssertions"/> chain from <see cref="Ensure.Must(DateTime, string?)"/>.</param>
    /// <param name="age">Minimum age in years (default <c>18</c>).</param>
    public static void BeAdult(this DateTimeAssertions assertions, int age = 18)
    {
        var (actual, expression) = ((IAssertionContext<DateTime>)assertions).Unwrap();
        var today = DateTime.UtcNow.Date;
        var computedAge = today.Year - actual.Year;
        if (actual.Date > today.AddYears(-computedAge))
            computedAge--;

        if (computedAge >= age)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {expression} to represent an adult (age >= {age}), but computed age was {computedAge}.",
            expression);
    }

    /// <summary>
    /// Verifies that the date of birth represents someone at least <paramref name="age"/> years old, computed against
    /// the UTC calendar date.
    /// </summary>
    /// <param name="assertions">The <see cref="DateOnlyAssertions"/> chain from <see cref="Ensure.Must(DateOnly, string?)"/>.</param>
    /// <param name="age">Minimum age in years (default <c>18</c>).</param>
    public static void BeAdult(this DateOnlyAssertions assertions, int age = 18)
    {
        var (actual, expression) = ((IAssertionContext<DateOnly>)assertions).Unwrap();
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var computedAge = today.Year - actual.Year;
        if (actual > today.AddYears(-computedAge))
            computedAge--;

        if (computedAge >= age)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {expression} to represent an adult (age >= {age}), but computed age was {computedAge}.",
            expression);
    }

    /// <summary>Verifies that the date's month and day match today's local calendar date.</summary>
    /// <param name="assertions">The <see cref="DateTimeAssertions"/> chain from <see cref="Ensure.Must(DateTime, string?)"/>.</param>
    public static void BeBirthday(this DateTimeAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<DateTime>)assertions).Unwrap();
        var today = DateTime.Today;

        if (actual.Month == today.Month && actual.Day == today.Day)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {expression} to be today's date (birthday), but was {actual:yyyy-MM-dd}.",
            expression);
    }

    /// <summary>Verifies that the date equals today's local calendar date.</summary>
    /// <param name="assertions">The <see cref="DateOnlyAssertions"/> chain from <see cref="Ensure.Must(DateOnly, string?)"/>.</param>
    public static void BeBirthday(this DateOnlyAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<DateOnly>)assertions).Unwrap();
        var today = DateOnly.FromDateTime(DateTime.Today);

        if (actual == today)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {expression} to be today's date (birthday), but was {actual:yyyy-MM-dd}.",
            expression);
    }

    /// <summary>
    /// Verifies that the time falls on a weekday within business hours (<c>09:00</c>–<c>17:00</c>).
    /// Weekend detection uses the current date in <paramref name="timeZone"/> or local time when omitted.
    /// </summary>
    /// <param name="assertions">The <see cref="TimeOnlyAssertions"/> chain from <see cref="Ensure.Must(TimeOnly, string?)"/>.</param>
    /// <param name="timeZone">Optional time zone for weekday evaluation.</param>
    public static void BeBusinessHours(this TimeOnlyAssertions assertions, TimeZoneInfo? timeZone = null)
    {
        var (actual, expression) = ((IAssertionContext<TimeOnly>)assertions).Unwrap();

        var now = timeZone is not null
            ? TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZone)
            : DateTime.Now;

        var dayOfWeek = now.DayOfWeek;
        if (dayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            VerificationFlow.Fail(
                $"Verification failed: expected {expression} to be during business hours, but it is a weekend ({dayOfWeek}).",
                expression);
            return;
        }

        var open = new TimeOnly(9, 0);
        var close = new TimeOnly(17, 0);

        if (actual >= open && actual < close)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {expression} to be during business hours (09:00-17:00), but was {actual:HH:mm}.",
            expression);
    }

    /// <summary>
    /// Verifies that the date-time falls on a weekday within business hours (<c>09:00</c>–<c>17:00</c>).
    /// </summary>
    /// <param name="assertions">The <see cref="DateTimeAssertions"/> chain from <see cref="Ensure.Must(DateTime, string?)"/>.</param>
    /// <param name="timeZone">Optional time zone used to interpret <paramref name="assertions"/> before checking hours.</param>
    public static void BeBusinessHours(this DateTimeAssertions assertions, TimeZoneInfo? timeZone = null)
    {
        var (actual, expression) = ((IAssertionContext<DateTime>)assertions).Unwrap();

        var adjusted = timeZone is not null
            ? TimeZoneInfo.ConvertTime(actual, timeZone)
            : actual;

        var dayOfWeek = adjusted.DayOfWeek;
        if (dayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            VerificationFlow.Fail(
                $"Verification failed: expected {expression} to be during business hours, but it is a weekend ({dayOfWeek}).",
                expression);
            return;
        }

        var time = adjusted.TimeOfDay;
        if (time >= new TimeSpan(9, 0, 0) && time < new TimeSpan(17, 0, 0))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {expression} to be during business hours (09:00-17:00), but was {adjusted:HH:mm}.",
            expression);
    }
}
