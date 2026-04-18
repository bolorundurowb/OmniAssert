using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>Provides assertions for <see cref="DateTime"/> subjects.</summary>
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

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be after {expectedExpression ?? "expected"} ({expected:O}), but was {_actual:O}.", _expression);
    }

    /// <summary>Verifies that the date/time is before the <paramref name="expected"/> date/time.</summary>
    /// <param name="expected">The date/time to compare against.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBeBefore(DateTime expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual < expected)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be before {expectedExpression ?? "expected"} ({expected:O}), but was {_actual:O}.", _expression);
    }

    /// <summary>Verifies that the date/time is within the specified <paramref name="precision"/> of the <paramref name="expected"/> date/time.</summary>
    /// <param name="precision">The maximum allowed difference.</param>
    /// <param name="expected">The expected date/time.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBeWithin(TimeSpan precision, DateTime expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (Math.Abs((_actual - expected).Ticks) <= precision.Ticks)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be within {precision} of {expectedExpression ?? "expected"} ({expected:O}), but was {_actual:O}.", _expression);
    }
}

/// <summary>Provides assertions for <see cref="DateTimeOffset"/> subjects.</summary>
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

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be after {expectedExpression ?? "expected"} ({expected:O}), but was {_actual:O}.", _expression);
    }

    /// <summary>Verifies that the date/time offset is before the <paramref name="expected"/> date/time offset.</summary>
    /// <param name="expected">The date/time offset to compare against.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBeBefore(DateTimeOffset expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual < expected)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be before {expectedExpression ?? "expected"} ({expected:O}), but was {_actual:O}.", _expression);
    }

    /// <summary>Verifies that the date/time offset is within the specified <paramref name="precision"/> of the <paramref name="expected"/> date/time offset.</summary>
    /// <param name="precision">The maximum allowed difference.</param>
    /// <param name="expected">The expected date/time offset.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBeWithin(TimeSpan precision, DateTimeOffset expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (Math.Abs((_actual - expected).Ticks) <= precision.Ticks)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be within {precision} of {expectedExpression ?? "expected"} ({expected:O}), but was {_actual:O}.", _expression);
    }
}
