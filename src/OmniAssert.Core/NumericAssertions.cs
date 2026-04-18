using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace OmniAssert;

public readonly struct NumericAssertions<T> where T : INumber<T>
{
    private readonly T _actual;
    private readonly string _expression;

    internal NumericAssertions(T actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    public void ToBe(T expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual == expected)
            return;

        var msg = FormatPair("to be", expected, expectedExpression ?? "expected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    public void NotToBe(T unexpected, [CallerArgumentExpression(nameof(unexpected))] string? unexpectedExpression = null)
    {
        if (_actual != unexpected)
            return;

        var msg = FormatPair("not to be", unexpected, unexpectedExpression ?? "unexpected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    public void ToBeGreaterThan(T expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual > expected)
            return;

        var msg = FormatFailure($"to be greater than {expectedExpression ?? "expected"} ({FormatValue(expected)})", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    public void ToBeGreaterThanOrEqualTo(T expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual >= expected)
            return;

        var msg = FormatFailure($"to be greater than or equal to {expectedExpression ?? "expected"} ({FormatValue(expected)})", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    public void ToBeLessThan(T expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual < expected)
            return;

        var msg = FormatFailure($"to be less than {expectedExpression ?? "expected"} ({FormatValue(expected)})", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    public void ToBeLessThanOrEqualTo(T expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual <= expected)
            return;

        var msg = FormatFailure($"to be less than or equal to {expectedExpression ?? "expected"} ({FormatValue(expected)})", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    public void ToBeInRange(T min, T max, [CallerArgumentExpression(nameof(min))] string? minExpr = null, [CallerArgumentExpression(nameof(max))] string? maxExpr = null)
    {
        if (_actual >= min && _actual <= max)
            return;

        var msg = FormatFailure($"to be in range [{minExpr ?? "min"}({FormatValue(min)}), {maxExpr ?? "max"}({FormatValue(max)})]", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

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
        sb.Append(AnsiColour.Actual($"Actual {actualLabel}: "));
        sb.Append(AnsiColour.Actual(FormatValue(actual)));
        return sb.ToString();
    }

    private static string FormatFailure(string expectedDesc, T actual, string actualLabel)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Verification failed.");
        sb.Append(AnsiColour.Expected($"Expected {actualLabel} {expectedDesc}."));
        sb.AppendLine();
        sb.Append(AnsiColour.Actual($"Actual {actualLabel}: "));
        sb.Append(AnsiColour.Actual(FormatValue(actual)));
        return sb.ToString();
    }

    private static string FormatValue(T value) => value.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
}
