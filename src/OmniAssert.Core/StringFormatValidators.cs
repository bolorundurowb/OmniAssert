using System.Text.RegularExpressions;

namespace OmniAssert;

/// <summary>
/// Shared, allocation-light format validators used by <see cref="StringAssertions"/>.
/// Kept internal so the public surface stays on the assertion structs while the
/// raw predicates remain individually testable and reusable across assertion methods.
/// </summary>
internal static class StringFormatValidators
{
    // Compiled once; ordinal/invariant by construction (character-class only patterns).
    private static readonly Regex Base64Regex = new(@"^[A-Za-z0-9+/]*={0,2}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex HexStringRegex = new(@"^[0-9A-Fa-f]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex HexColorRegex = new(@"^#(?:[0-9A-Fa-f]{3}|[0-9A-Fa-f]{4}|[0-9A-Fa-f]{6}|[0-9A-Fa-f]{8})$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>True when the string has no leading or trailing whitespace. Empty strings are considered trimmed.</summary>
    internal static bool IsTrimmed(string s)
    {
        if (s.Length == 0)
            return true;

        return !char.IsWhiteSpace(s[0]) && !char.IsWhiteSpace(s[^1]);
    }

    /// <summary>True when the string is non-empty and every character is a letter or digit.</summary>
    internal static bool IsAlphanumeric(string s) => s.Length != 0 && s.All(char.IsLetterOrDigit);

    /// <summary>True when the string is non-empty and every character is a decimal digit (leading zeros preserved).</summary>
    internal static bool IsAllDigits(string s) => s.Length != 0 && s.All(char.IsDigit);

    /// <summary>True when the string contains no upper-case letters (ordinal). Digits/punctuation are allowed.</summary>
    internal static bool IsLowerCase(string s) => s.All(c => !char.IsUpper(c));

    /// <summary>True when the string contains no lower-case letters (ordinal). Digits/punctuation are allowed.</summary>
    internal static bool IsUpperCase(string s) => s.All(c => !char.IsLower(c));

    /// <summary>True when the string is valid Base64 text (canonical alphabet, length a multiple of 4, optional padding).</summary>
    internal static bool IsBase64(string s) => s.Length % 4 == 0 && Base64Regex.IsMatch(s);

    /// <summary>True when the string is non-empty and consists solely of hexadecimal digits.</summary>
    internal static bool IsHexString(string s) => HexStringRegex.IsMatch(s);

    /// <summary>True when the string is a CSS-style hex colour: <c>#RGB</c>, <c>#RGBA</c>, <c>#RRGGBB</c>, or <c>#RRGGBBAA</c>.</summary>
    internal static bool IsHexColor(string s) => HexColorRegex.IsMatch(s);
}
