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

    /// <summary>Verifies that the timespan is strictly positive.</summary>
    public void ToBePositive()
    {
        if (_actual > TimeSpan.Zero)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be positive, but was {FormatValue(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the timespan is strictly negative.</summary>
    public void ToBeNegative()
    {
        if (_actual < TimeSpan.Zero)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be negative, but was {FormatValue(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the timespan is greater than <paramref name="expected"/>.</summary>
    /// <param name="expected">The lower bound that actual duration must exceed.</param>
    /// <param name="expectedExpression">The expression for the expected timespan (automatically captured).</param>
    public void ToBeGreaterThan(TimeSpan expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual > expected)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be greater than {expectedExpression ?? "expected"} ({FormatValue(expected)}), but was {FormatValue(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the timespan is less than <paramref name="expected"/>.</summary>
    /// <param name="expected">The upper bound that actual duration must be below.</param>
    /// <param name="expectedExpression">The expression for the expected timespan (automatically captured).</param>
    public void ToBeLessThan(TimeSpan expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual < expected)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be less than {expectedExpression ?? "expected"} ({FormatValue(expected)}), but was {FormatValue(_actual)}.",
            _expression);
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
}
