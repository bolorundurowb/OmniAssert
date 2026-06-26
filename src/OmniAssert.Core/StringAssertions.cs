using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace OmniAssert;

/// <summary>Assertions for <see cref="string"/> (or null) subjects from <see cref="Ensure.Must(string?, string?)"/>.</summary>
public readonly struct StringAssertions : IAssertionContext<string?>
{
    private readonly string? _actual;
    private readonly string _expression;

    internal StringAssertions(string? actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    string? IAssertionContext<string?>.Subject => _actual;
    string IAssertionContext<string?>.Expression => _expression;

    /// <summary>Verifies that the string is equal to the <paramref name="expected"/> string using ordinal comparison.</summary>
    /// <param name="expected">The expected string.</param>
    /// <param name="expectedExpression">The expression for the expected string (automatically captured).</param>
    public void Be(string? expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (string.Equals(_actual, expected, StringComparison.Ordinal))
            return;

        var msg = FormatStrings("to be", expected, expectedExpression ?? "expected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the string is not equal to the <paramref name="unexpected"/> string using ordinal comparison.</summary>
    /// <param name="unexpected">The unexpected string.</param>
    /// <param name="unexpectedExpression">The expression for the unexpected string (automatically captured).</param>
    public void NotBe(string? unexpected, [CallerArgumentExpression(nameof(unexpected))] string? unexpectedExpression = null)
    {
        if (!string.Equals(_actual, unexpected, StringComparison.Ordinal))
            return;

        var msg = FormatStrings("not to be", unexpected, unexpectedExpression ?? "unexpected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the string contains the specified <paramref name="substring"/> using ordinal comparison.</summary>
    /// <param name="substring">The substring expected to be present.</param>
    /// <param name="substringExpression">The expression for the substring (automatically captured).</param>
    public void Contain(string substring, [CallerArgumentExpression(nameof(substring))] string? substringExpression = null)
    {
        if (_actual is not null && _actual.Contains(substring, StringComparison.Ordinal))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to contain {substringExpression ?? "substring"} ({Quote(substring)}), but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string does not contain the specified <paramref name="substring"/> using ordinal comparison.</summary>
    /// <param name="substring">The substring expected to be absent.</param>
    /// <param name="substringExpression">The expression for the substring (automatically captured).</param>
    public void NotContain(string substring, [CallerArgumentExpression(nameof(substring))] string? substringExpression = null)
    {
        if (_actual is null || !_actual.Contains(substring, StringComparison.Ordinal))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} not to contain {substringExpression ?? "substring"} ({Quote(substring)}), but it did.",
            _expression);
    }

    /// <summary>Verifies that the string is empty.</summary>
    public void BeEmpty()
    {
        if (string.IsNullOrEmpty(_actual))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be empty, but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string is not null or empty.</summary>
    public void NotBeEmpty()
    {
        if (!string.IsNullOrEmpty(_actual))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} not to be empty, but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string has exactly <paramref name="expectedLength"/> characters.</summary>
    /// <param name="expectedLength">The expected string length.</param>
    /// <param name="lengthExpression">The expression for the expected length (automatically captured).</param>
    public void HaveLength(int expectedLength, [CallerArgumentExpression(nameof(expectedLength))] string? lengthExpression = null)
    {
        if (_actual is not null && _actual.Length == expectedLength)
            return;

        var actualLengthText = _actual is null ? "null" : _actual.Length.ToString();
        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have length {expectedLength} ({lengthExpression ?? "expectedLength"}), but had length {actualLengthText}.",
            _expression);
    }

    /// <summary>Verifies that the string length is greater than <paramref name="minimumLength"/>.</summary>
    /// <param name="minimumLength">The exclusive lower bound for the string length.</param>
    /// <param name="lengthExpression">The expression for the minimum length (automatically captured).</param>
    public void HaveLengthGreaterThan(int minimumLength, [CallerArgumentExpression(nameof(minimumLength))] string? lengthExpression = null)
    {
        if (_actual is not null && _actual.Length > minimumLength)
            return;

        var actualLengthText = _actual is null ? "null" : _actual.Length.ToString();
        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have length greater than {minimumLength} ({lengthExpression ?? "minimumLength"}), but had length {actualLengthText}.",
            _expression);
    }

    /// <summary>Verifies that the string length is less than <paramref name="maximumLength"/>.</summary>
    /// <param name="maximumLength">The exclusive upper bound for the string length.</param>
    /// <param name="lengthExpression">The expression for the maximum length (automatically captured).</param>
    public void HaveLengthLessThan(int maximumLength, [CallerArgumentExpression(nameof(maximumLength))] string? lengthExpression = null)
    {
        if (_actual is not null && _actual.Length < maximumLength)
            return;

        var actualLengthText = _actual is null ? "null" : _actual.Length.ToString();
        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have length less than {maximumLength} ({lengthExpression ?? "maximumLength"}), but had length {actualLengthText}.",
            _expression);
    }

    /// <summary>Verifies that the string is null or empty.</summary>
    public void BeNullOrEmpty()
    {
        if (string.IsNullOrEmpty(_actual))
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be null or empty, but was {Quote(_actual)}.", _expression);
    }

    /// <summary>Verifies that the string is not null or empty.</summary>
    public void NotBeNullOrEmpty() => NotBeEmpty();

    /// <summary>Verifies that the string is null, empty, or consists only of whitespace characters.</summary>
    public void BeNullOrWhiteSpace()
    {
        if (string.IsNullOrWhiteSpace(_actual))
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be null or whitespace, but was {Quote(_actual)}.", _expression);
    }

    /// <summary>Verifies that the string is not null, empty, or consists only of whitespace characters.</summary>
    public void NotBeNullOrWhiteSpace()
    {
        if (!string.IsNullOrWhiteSpace(_actual))
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} not to be null or whitespace, but was {Quote(_actual)}.", _expression);
    }

    /// <summary>Verifies that the string consists only of whitespace characters (and is not null or empty).</summary>
    public void BeWhiteSpace()
    {
        if (!string.IsNullOrEmpty(_actual) && string.IsNullOrWhiteSpace(_actual))
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be whitespace, but was {Quote(_actual)}.", _expression);
    }

    /// <summary>Verifies that the string does not consist only of whitespace characters.</summary>
    public void NotBeWhiteSpace()
    {
        if (string.IsNullOrEmpty(_actual) || !string.IsNullOrWhiteSpace(_actual))
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} not to be whitespace, but was {Quote(_actual)}.", _expression);
    }


    /// <summary>Verifies that the string is null.</summary>
    public void BeNull()
    {
        if (_actual is null)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be null, but was {Quote(_actual)}.", _expression);
    }

    /// <summary>Verifies that the string is not null.</summary>
    public void NotBeNull()
    {
        if (_actual is not null)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} not to be null.", _expression);
    }

    /// <summary>Verifies that the string starts with the specified <paramref name="prefix"/>.</summary>
    /// <param name="prefix">The expected prefix.</param>
    /// <param name="comparison">The string comparison culture/options.</param>
    /// <param name="prefixExpression">The expression for the prefix (automatically captured).</param>
    public void StartWith(string prefix, StringComparison comparison = StringComparison.Ordinal, [CallerArgumentExpression(nameof(prefix))] string? prefixExpression = null)
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
    public void EndWith(string suffix, StringComparison comparison = StringComparison.Ordinal, [CallerArgumentExpression(nameof(suffix))] string? suffixExpression = null)
    {
        if (_actual is not null && _actual.EndsWith(suffix, comparison))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to end with {suffixExpression ?? "suffix"} ({Quote(suffix)}), but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string does not end with the specified <paramref name="suffix"/>.</summary>
    /// <param name="suffix">The suffix that should not be at the end.</param>
    /// <param name="comparison">The string comparison culture/options.</param>
    /// <param name="suffixExpression">The expression for the suffix (automatically captured).</param>
    public void NotEndWith(string suffix, StringComparison comparison = StringComparison.Ordinal, [CallerArgumentExpression(nameof(suffix))] string? suffixExpression = null)
    {
        if (_actual is null || !_actual.EndsWith(suffix, comparison))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} not to end with {suffixExpression ?? "suffix"} ({Quote(suffix)}), but it did.",
            _expression);
    }

    /// <summary>Verifies that the string matches the specified regular expression.</summary>
    /// <param name="regexPattern">The regular expression pattern.</param>
    /// <param name="options">The regex options.</param>
    /// <param name="patternExpression">The expression for the pattern (automatically captured).</param>
    public void Match(string regexPattern, RegexOptions options = RegexOptions.None, [CallerArgumentExpression(nameof(regexPattern))] string? patternExpression = null)
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
    public void Be(string? expected, StringComparison comparison, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (string.Equals(_actual, expected, comparison))
            return;

        var msg = FormatStrings("to be", expected, expectedExpression ?? "expected", _actual, _expression);
        VerificationFlow.Fail(msg, _expression);
    }

    /// <summary>Verifies that the string is equal to the <paramref name="expected"/> string using ordinal case-insensitive comparison.</summary>
    /// <param name="expected">The expected string.</param>
    /// <param name="expectedExpression">The expression for the expected string (automatically captured).</param>
    public void BeIgnoringCase(string? expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null) =>
        Be(expected, StringComparison.OrdinalIgnoreCase, expectedExpression);

    /// <summary>Verifies that the string is equal to one of the provided expected values using ordinal comparison.</summary>
    /// <param name="expected">Allowed expected string values.</param>
    public void BeOneOf(params string?[] expected)
    {
        if (expected is not null)
        {
            foreach (var candidate in expected)
            {
                if (string.Equals(_actual, candidate, StringComparison.Ordinal))
                    return;
            }
        }

        var expectedList = expected is null || expected.Length == 0
            ? "[]"
            : $"[{string.Join(", ", expected.Select(Quote))}]";
        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be one of {expectedList}, but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string contains the specified <paramref name="substring"/> using the specified <paramref name="comparison"/>.</summary>
    /// <param name="substring">The substring expected to be present.</param>
    /// <param name="comparison">The string comparison culture/options.</param>
    /// <param name="substringExpression">The expression for the substring (automatically captured).</param>
    public void Contain(string substring, StringComparison comparison, [CallerArgumentExpression(nameof(substring))] string? substringExpression = null)
    {
        if (_actual is not null && _actual.Contains(substring, comparison))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to contain {substringExpression ?? "substring"} ({Quote(substring)}), but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string does not contain the specified <paramref name="substring"/> using the specified <paramref name="comparison"/>.</summary>
    /// <param name="substring">The substring expected to be absent.</param>
    /// <param name="comparison">The string comparison culture/options.</param>
    /// <param name="substringExpression">The expression for the substring (automatically captured).</param>
    public void NotContain(string substring, StringComparison comparison, [CallerArgumentExpression(nameof(substring))] string? substringExpression = null)
    {
        if (_actual is null || !_actual.Contains(substring, comparison))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} not to contain {substringExpression ?? "substring"} ({Quote(substring)}), but it did.",
            _expression);
    }

    /// <summary>Verifies that the string has no leading or trailing whitespace (internal whitespace is allowed).</summary>
    public void BeTrimmed()
    {
        if (_actual is not null && StringFormatValidators.IsTrimmed(_actual))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be trimmed (no leading or trailing whitespace), but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string length is within the inclusive range [<paramref name="minLength"/>, <paramref name="maxLength"/>].</summary>
    /// <param name="minLength">The minimum inclusive length.</param>
    /// <param name="maxLength">The maximum inclusive length.</param>
    /// <param name="minExpression">The expression for the minimum length (automatically captured).</param>
    /// <param name="maxExpression">The expression for the maximum length (automatically captured).</param>
    public void HaveLengthBetween(int minLength, int maxLength,
        [CallerArgumentExpression(nameof(minLength))] string? minExpression = null,
        [CallerArgumentExpression(nameof(maxLength))] string? maxExpression = null)
    {
        if (_actual is not null && _actual.Length >= minLength && _actual.Length <= maxLength)
            return;

        var actualLengthText = _actual is null ? "null" : _actual.Length.ToString();
        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have length between {minExpression ?? "minLength"} ({minLength}) and {maxExpression ?? "maxLength"} ({maxLength}), but had length {actualLengthText}.",
            _expression);
    }

    /// <summary>Verifies that the string is non-empty and consists only of letters and digits.</summary>
    public void BeAlphanumeric()
    {
        if (_actual is not null && StringFormatValidators.IsAlphanumeric(_actual))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be alphanumeric (letters and digits only), but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string contains no upper-case letters (ordinal). Digits and punctuation are permitted.</summary>
    public void BeLowerCase()
    {
        if (_actual is not null && StringFormatValidators.IsLowerCase(_actual))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be lower case, but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string contains no lower-case letters (ordinal). Digits and punctuation are permitted.</summary>
    public void BeUpperCase()
    {
        if (_actual is not null && StringFormatValidators.IsUpperCase(_actual))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be upper case, but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string is non-empty and consists only of decimal digits (leading zeros are preserved).</summary>
    public void BeNumeric()
    {
        if (_actual is not null && StringFormatValidators.IsAllDigits(_actual))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be numeric (digits only), but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string is valid Base64 text (canonical alphabet, length a multiple of four, optional padding).</summary>
    public void BeBase64()
    {
        if (_actual is not null && StringFormatValidators.IsBase64(_actual))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be a Base64 string, but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string is non-empty and consists solely of hexadecimal digits (<c>0-9</c>, <c>a-f</c>, <c>A-F</c>).</summary>
    public void BeHexString()
    {
        if (_actual is not null && StringFormatValidators.IsHexString(_actual))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be a hexadecimal string, but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string is a CSS-style hex colour: <c>#RGB</c>, <c>#RGBA</c>, <c>#RRGGBB</c>, or <c>#RRGGBBAA</c>.</summary>
    public void BeHexColor()
    {
        if (_actual is not null && StringFormatValidators.IsHexColor(_actual))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be a hex colour (e.g. #RGB, #RRGGBB, or #RRGGBBAA), but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>
    /// Verifies that the string is a valid ISO 8601 date/time. Parsing uses <see cref="System.Globalization.CultureInfo.InvariantCulture"/>
    /// with round-trip semantics, so the result is culture-independent.
    /// </summary>
    public void BeIso8601()
    {
        if (_actual is not null &&
            DateTime.TryParse(_actual, System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.RoundtripKind, out _))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be an ISO 8601 date/time string, but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>
    /// Verifies that the string matches the exact date/time <paramref name="format"/>. Parsing is performed with
    /// <see cref="System.Globalization.CultureInfo.InvariantCulture"/> and exact matching, so results are culture-independent.
    /// </summary>
    /// <param name="format">The exact .NET date/time format string (for example <c>yyyy-MM-dd</c>).</param>
    /// <param name="formatExpression">The expression for the format (automatically captured).</param>
    public void BeDateString(string format, [CallerArgumentExpression(nameof(format))] string? formatExpression = null)
    {
        if (_actual is not null &&
            DateTime.TryParseExact(_actual, format, System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out _))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be a date string matching format {formatExpression ?? "format"} ({Quote(format)}), but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string is a rooted (absolute) path via <see cref="System.IO.Path.IsPathRooted(string)"/>.</summary>
    /// <remarks>Path rooting rules are OS-specific; this is a syntactic check only and does not touch the file system.</remarks>
    public void BeAbsolutePath()
    {
        if (_actual is not null && System.IO.Path.IsPathRooted(_actual))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be an absolute (rooted) path, but was {Quote(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the string is a relative (non-rooted) path via <see cref="System.IO.Path.IsPathRooted(string)"/>.</summary>
    /// <remarks>Path rooting rules are OS-specific; this is a syntactic check only and does not touch the file system.</remarks>
    public void BeRelativePath()
    {
        if (_actual is not null && !System.IO.Path.IsPathRooted(_actual))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be a relative path, but was {Quote(_actual)}.",
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
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBe(string? expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null) => Be(expected, expectedExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void NotToBe(string? unexpected, [CallerArgumentExpression(nameof(unexpected))] string? unexpectedExpression = null) => NotBe(unexpected, unexpectedExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToContain(string substring, [CallerArgumentExpression(nameof(substring))] string? substringExpression = null) => Contain(substring, substringExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void NotToContain(string substring, [CallerArgumentExpression(nameof(substring))] string? substringExpression = null) => NotContain(substring, substringExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeEmpty() => BeEmpty();
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void NotToBeEmpty() => NotBeEmpty();
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToHaveLength(int expectedLength, [CallerArgumentExpression(nameof(expectedLength))] string? lengthExpression = null) => HaveLength(expectedLength, lengthExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToHaveLengthGreaterThan(int minimumLength, [CallerArgumentExpression(nameof(minimumLength))] string? lengthExpression = null) => HaveLengthGreaterThan(minimumLength, lengthExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToHaveLengthLessThan(int maximumLength, [CallerArgumentExpression(nameof(maximumLength))] string? lengthExpression = null) => HaveLengthLessThan(maximumLength, lengthExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeNullOrEmpty() => BeNullOrEmpty();
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void NotToBeNullOrEmpty() => NotBeNullOrEmpty();
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeNullOrWhiteSpace() => BeNullOrWhiteSpace();
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void NotToBeNullOrWhiteSpace() => NotBeNullOrWhiteSpace();
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeWhiteSpace() => BeWhiteSpace();
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void NotToBeWhiteSpace() => NotBeWhiteSpace();
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeNull() => BeNull();
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void NotToBeNull() => NotBeNull();
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToStartWith(string prefix, StringComparison comparison = StringComparison.Ordinal, [CallerArgumentExpression(nameof(prefix))] string? prefixExpression = null) => StartWith(prefix, comparison, prefixExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToEndWith(string suffix, StringComparison comparison = StringComparison.Ordinal, [CallerArgumentExpression(nameof(suffix))] string? suffixExpression = null) => EndWith(suffix, comparison, suffixExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void NotToEndWith(string suffix, StringComparison comparison = StringComparison.Ordinal, [CallerArgumentExpression(nameof(suffix))] string? suffixExpression = null) => NotEndWith(suffix, comparison, suffixExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToMatch(string regexPattern, RegexOptions options = RegexOptions.None, [CallerArgumentExpression(nameof(regexPattern))] string? patternExpression = null) => Match(regexPattern, options, patternExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeIgnoringCase(string? expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null) => BeIgnoringCase(expected, expectedExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeOneOf(params string?[] expected) => BeOneOf(expected);
}
