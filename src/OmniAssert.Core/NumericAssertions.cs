using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace OmniAssert;

/// <summary>Provides assertions for numeric types.</summary>
/// <typeparam name="T">A numeric type that implements <see cref="INumber{T}"/>.</typeparam>
public readonly struct NumericAssertions<T> where T : INumber<T>
{
    private readonly T _actual;
    private readonly string _expression;

    internal NumericAssertions(T actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    /// <summary>Verifies that the numeric value is equal to the <paramref name="expected"/> value.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBe(T expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual == expected)
            return;

        var msg = FormatPair("to be", expected, expectedExpression ?? "expected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the numeric value is not equal to the <paramref name="unexpected"/> value.</summary>
    /// <param name="unexpected">The value that the actual value should not be.</param>
    /// <param name="unexpectedExpression">The expression for the unexpected value (automatically captured).</param>
    public void NotToBe(T unexpected, [CallerArgumentExpression(nameof(unexpected))] string? unexpectedExpression = null)
    {
        if (_actual != unexpected)
            return;

        var msg = FormatPair("not to be", unexpected, unexpectedExpression ?? "unexpected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the numeric value is greater than the <paramref name="expected"/> value.</summary>
    /// <param name="expected">The value to compare against.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBeGreaterThan(T expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual > expected)
            return;

        var msg = FormatFailure($"to be greater than {expectedExpression ?? "expected"} ({FormatValue(expected)})", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the numeric value is greater than or equal to the <paramref name="expected"/> value.</summary>
    /// <param name="expected">The value to compare against.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBeGreaterThanOrEqualTo(T expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual >= expected)
            return;

        var msg = FormatFailure($"to be greater than or equal to {expectedExpression ?? "expected"} ({FormatValue(expected)})", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the numeric value is less than the <paramref name="expected"/> value.</summary>
    /// <param name="expected">The value to compare against.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBeLessThan(T expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual < expected)
            return;

        var msg = FormatFailure($"to be less than {expectedExpression ?? "expected"} ({FormatValue(expected)})", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the numeric value is less than or equal to the <paramref name="expected"/> value.</summary>
    /// <param name="expected">The value to compare against.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBeLessThanOrEqualTo(T expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual <= expected)
            return;

        var msg = FormatFailure($"to be less than or equal to {expectedExpression ?? "expected"} ({FormatValue(expected)})", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the numeric value is within the specified range [min, max].</summary>
    /// <param name="min">The minimum inclusive value.</param>
    /// <param name="max">The maximum inclusive value.</param>
    /// <param name="minExpr">The expression for the minimum value (automatically captured).</param>
    /// <param name="maxExpr">The expression for the maximum value (automatically captured).</param>
    public void ToBeInRange(T min, T max, [CallerArgumentExpression(nameof(min))] string? minExpr = null, [CallerArgumentExpression(nameof(max))] string? maxExpr = null)
    {
        if (_actual >= min && _actual <= max)
            return;

        var msg = FormatFailure($"to be in range [{minExpr ?? "min"}({FormatValue(min)}), {maxExpr ?? "max"}({FormatValue(max)})]", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the numeric value is approximately equal to the <paramref name="expected"/> value within the given <paramref name="precision"/>.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="precision">The maximum allowed difference.</param>
    /// <param name="expectedExpr">The expression for the expected value (automatically captured).</param>
    public void ToBeApproximately(T expected, T precision, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
    {
        if (T.Abs(_actual - expected) <= precision)
            return;

        var msg = FormatFailure($"to be approximately {expectedExpr ?? "expected"} ({FormatValue(expected)}) within precision {FormatValue(precision)}", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    private static string FormatPair(string relation, T expected, string expectedLabel, T actual, string actualLabel)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Verification failed.");
        sb.Append(AnsiColour.Expected($"Expected {actualLabel} {relation} {expectedLabel}: "));
        sb.AppendLine(AnsiColour.Expected(FormatValue(expected)));
        sb.Append(AnsiColour.Actual($"Got {actualLabel}: "));
        sb.Append(AnsiColour.Actual(FormatValue(actual)));
        return sb.ToString();
    }

    private static string FormatFailure(string expectedDesc, T actual, string actualLabel)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Verification failed.");
        sb.Append(AnsiColour.Expected($"Expected {actualLabel} {expectedDesc}."));
        sb.AppendLine();
        sb.Append(AnsiColour.Actual($"Got {actualLabel}: "));
        sb.Append(AnsiColour.Actual(FormatValue(actual)));
        return sb.ToString();
    }

    private static string FormatValue(T value) => value.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
}
