using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace OmniAssert;

/// <summary>Provides assertions for string subjects.</summary>
public readonly struct StringAssertions
{
    private readonly string? _actual;
    private readonly string _expression;

    internal StringAssertions(string? actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    /// <summary>Verifies that the string is equal to the <paramref name="expected"/> string using ordinal comparison.</summary>
    /// <param name="expected">The expected string.</param>
    /// <param name="expectedExpression">The expression for the expected string (automatically captured).</param>
    public void ToBe(string? expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (string.Equals(_actual, expected, StringComparison.Ordinal))
            return;

        var msg = FormatStrings("to be", expected, expectedExpression ?? "expected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the string is not equal to the <paramref name="unexpected"/> string using ordinal comparison.</summary>
    /// <param name="unexpected">The unexpected string.</param>
    /// <param name="unexpectedExpression">The expression for the unexpected string (automatically captured).</param>
    public void NotToBe(string? unexpected, [CallerArgumentExpression(nameof(unexpected))] string? unexpectedExpression = null)
    {
        if (!string.Equals(_actual, unexpected, StringComparison.Ordinal))
            return;

        var msg = FormatStrings("not to be", unexpected, unexpectedExpression ?? "unexpected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the string contains the specified <paramref name="substring"/> using ordinal comparison.</summary>
    /// <param name="substring">The substring expected to be present.</param>
    /// <param name="substringExpression">The expression for the substring (automatically captured).</param>
    public void ToContain(string substring, [CallerArgumentExpression(nameof(substring))] string? substringExpression = null)
    {
        if (_actual is not null && substring.Length > 0 && _actual.Contains(substring, StringComparison.Ordinal))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to contain {substringExpression ?? "substring"} ({Quote(substring)}), but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string is empty.</summary>
    public void ToBeEmpty()
    {
        if (string.IsNullOrEmpty(_actual))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be empty, but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string is null or empty.</summary>
    public void ToBeNullOrEmpty()
    {
        if (string.IsNullOrEmpty(_actual))
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be null or empty, but was {Quote(_actual)}.", _expression);
    }

    /// <summary>Verifies that the string is null, empty, or consists only of white-space characters.</summary>
    public void ToBeNullOrWhiteSpace()
    {
        if (string.IsNullOrWhiteSpace(_actual))
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be null or white space, but was {Quote(_actual)}.", _expression);
    }

    /// <summary>Verifies that the string is null.</summary>
    public void ToBeNull()
    {
        if (_actual is null)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be null, but was {Quote(_actual)}.", _expression);
    }

    /// <summary>Verifies that the string is not null.</summary>
    public void NotToBeNull()
    {
        if (_actual is not null)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} not to be null.", _expression);
    }

    /// <summary>Verifies that the string starts with the specified <paramref name="prefix"/>.</summary>
    /// <param name="prefix">The expected prefix.</param>
    /// <param name="comparison">The string comparison culture/options.</param>
    /// <param name="prefixExpression">The expression for the prefix (automatically captured).</param>
    public void ToStartWith(string prefix, StringComparison comparison = StringComparison.Ordinal, [CallerArgumentExpression(nameof(prefix))] string? prefixExpression = null)
    {
        if (_actual is not null && _actual.StartsWith(prefix, comparison))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to start with {prefixExpression ?? "prefix"} ({Quote(prefix)}), but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string ends with the specified <paramref name="suffix"/>.</summary>
    /// <param name="suffix">The expected suffix.</param>
    /// <param name="comparison">The string comparison culture/options.</param>
    /// <param name="suffixExpression">The expression for the suffix (automatically captured).</param>
    public void ToEndWith(string suffix, StringComparison comparison = StringComparison.Ordinal, [CallerArgumentExpression(nameof(suffix))] string? suffixExpression = null)
    {
        if (_actual is not null && _actual.EndsWith(suffix, comparison))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to end with {suffixExpression ?? "suffix"} ({Quote(suffix)}), but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string matches the specified regular expression.</summary>
    /// <param name="regexPattern">The regular expression pattern.</param>
    /// <param name="options">The regex options.</param>
    /// <param name="patternExpression">The expression for the pattern (automatically captured).</param>
    public void ToMatch(string regexPattern, RegexOptions options = RegexOptions.None, [CallerArgumentExpression(nameof(regexPattern))] string? patternExpression = null)
    {
        if (_actual is not null && Regex.IsMatch(_actual, regexPattern, options))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to match regex {patternExpression ?? "pattern"} ({Quote(regexPattern)}), but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string is equal to the <paramref name="expected"/> string using the specified <paramref name="comparison"/>.</summary>
    /// <param name="expected">The expected string.</param>
    /// <param name="comparison">The string comparison culture/options.</param>
    /// <param name="expectedExpression">The expression for the expected string (automatically captured).</param>
    public void ToBe(string? expected, StringComparison comparison, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (string.Equals(_actual, expected, comparison))
            return;

        var msg = FormatStrings("to be", expected, expectedExpression ?? "expected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the string contains the specified <paramref name="substring"/> using the specified <paramref name="comparison"/>.</summary>
    /// <param name="substring">The substring expected to be present.</param>
    /// <param name="comparison">The string comparison culture/options.</param>
    /// <param name="substringExpression">The expression for the substring (automatically captured).</param>
    public void ToContain(string substring, StringComparison comparison, [CallerArgumentExpression(nameof(substring))] string? substringExpression = null)
    {
        if (_actual is not null && substring.Length > 0 && _actual.Contains(substring, comparison))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to contain {substringExpression ?? "substring"} ({Quote(substring)}), but was {Quote(_actual)}.",
            _expression);
    }

    private static string FormatStrings(string relation, string? expected, string expectedLabel, string? actual, string actualLabel)
    {
        if (relation == "not to be")
        {
            var sbNot = new StringBuilder();
            sbNot.AppendLine("Verification failed.");
            sbNot.Append("Expected ");
            sbNot.Append(actualLabel);
            sbNot.Append(" not to be ");
            sbNot.Append(AnsiColour.Expected(Quote(expected)));
            sbNot.Append(", but it was.");
            return sbNot.ToString();
        }

        if (expected == null || actual == null)
        {
            var sbFallback = new StringBuilder();
            sbFallback.AppendLine("Verification failed.");
            sbFallback.Append(AnsiColour.Expected("Expected "));
            sbFallback.Append(expectedLabel);
            sbFallback.Append(": ");
            sbFallback.AppendLine(AnsiColour.Expected(Quote(expected)));
            sbFallback.Append(AnsiColour.Actual("Got "));
            sbFallback.Append(actualLabel);
            sbFallback.Append(": ");
            sbFallback.Append(AnsiColour.Actual(Quote(actual)));
            return sbFallback.ToString();
        }

        int firstDiff = 0;
        int minLen = Math.Min(expected.Length, actual.Length);
        while (firstDiff < minLen && expected[firstDiff] == actual[firstDiff])
        {
            firstDiff++;
        }

        var (visibleExpected, _) = ToVisibleString(expected, firstDiff);
        var (visibleActual, actualOffset) = ToVisibleString(actual, firstDiff);

        var sb = new StringBuilder();
        sb.AppendLine("Verification failed.");
        sb.Append("  Expected: ");
        sb.AppendLine(AnsiColour.Expected($"\"{visibleExpected}\""));
        sb.Append("  Got:      ");
        sb.AppendLine(AnsiColour.Actual($"\"{visibleActual}\""));

        // "  Got:      " is 12 chars, +1 for opening quote
        int caretPadding = 12 + 1 + actualOffset;
        sb.Append(new string(' ', caretPadding));
        sb.Append('^');
        sb.Append(" expected ");
        sb.Append(DescribeChar(expected, firstDiff));
        sb.Append(", got ");
        sb.Append(DescribeChar(actual, firstDiff));
        sb.Append(" at position ");
        sb.Append(firstDiff);
        sb.AppendLine();

        sb.Append("  Lengths: ");
        sb.Append(expected.Length);
        sb.Append(" vs ");
        sb.Append(actual.Length);
        sb.Append(" chars");

        return sb.ToString();
    }

    private static (string Visible, int OffsetAtFirstDiff) ToVisibleString(string s, int firstDiff)
    {
        var sb = new StringBuilder();
        int offset = 0;
        for (int i = 0; i < s.Length; i++)
        {
            if (i == firstDiff) offset = sb.Length;
            StringFormatter.AppendVisible(sb, s[i]);
        }
        if (firstDiff >= s.Length) offset = sb.Length;
        return (sb.ToString(), offset);
    }

    private static string DescribeChar(string s, int index)
    {
        if (index >= s.Length) return "end of string";
        return $"'{StringFormatter.EscapeChar(s[index])}'";
    }

    private static string Quote(string? s) => StringFormatter.Quote(s);
}
