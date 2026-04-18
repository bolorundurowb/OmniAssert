using System.Runtime.CompilerServices;

namespace OmniAssert;

public readonly struct DateTimeAssertions
{
    private readonly DateTime _actual;
    private readonly string _expression;

    internal DateTimeAssertions(DateTime actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    public void ToBeAfter(DateTime expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual > expected)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be after {expectedExpression ?? "expected"} ({expected:O}), but was {_actual:O}.", _expression);
    }

    public void ToBeBefore(DateTime expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual < expected)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be before {expectedExpression ?? "expected"} ({expected:O}), but was {_actual:O}.", _expression);
    }

    public void ToBeWithin(TimeSpan precision, DateTime expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (Math.Abs((_actual - expected).Ticks) <= precision.Ticks)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be within {precision} of {expectedExpression ?? "expected"} ({expected:O}), but was {_actual:O}.", _expression);
    }
}

public readonly struct DateTimeOffsetAssertions
{
    private readonly DateTimeOffset _actual;
    private readonly string _expression;

    internal DateTimeOffsetAssertions(DateTimeOffset actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    public void ToBeAfter(DateTimeOffset expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual > expected)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be after {expectedExpression ?? "expected"} ({expected:O}), but was {_actual:O}.", _expression);
    }

    public void ToBeBefore(DateTimeOffset expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual < expected)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be before {expectedExpression ?? "expected"} ({expected:O}), but was {_actual:O}.", _expression);
    }

    public void ToBeWithin(TimeSpan precision, DateTimeOffset expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (Math.Abs((_actual - expected).Ticks) <= precision.Ticks)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be within {precision} of {expectedExpression ?? "expected"} ({expected:O}), but was {_actual:O}.", _expression);
    }
}
