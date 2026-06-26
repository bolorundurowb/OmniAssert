using System.Text.RegularExpressions;
using OmniAssert.Extensions.Internal;
using OmniAssert.Extensions.Security.Validators;

namespace OmniAssert.Extensions.Security;

/// <summary>Security, authentication, and cryptography assertions for <see cref="StringAssertions"/> subjects.</summary>
public static partial class StringSecurityExtensions
{
    /// <summary>
    /// Verifies that the string meets password complexity rules: at least <paramref name="minLength"/> characters with
    /// uppercase, lowercase, digit, and symbol.
    /// </summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    /// <param name="minLength">Minimum required length (default <c>8</c>).</param>
    public static void BeStrongPassword(this StringAssertions assertions, int minLength = 8)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (PasswordValidator.IsStrong(actual ?? "", minLength))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {expression} to be a strong password (min {minLength} chars, uppercase, lowercase, digit, symbol), but it was not.",
            expression);
    }

    /// <summary>Verifies that the string is a valid SHA-256 hash (64 hexadecimal characters).</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeSha256Hash(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();
        var stripped = StripHexPrefix(actual);

        if (stripped is not null && stripped.Length == 64 && IsHexString(stripped))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid SHA-256 hash (64 hex chars)", actual), expression);
    }

    /// <summary>Verifies that the string is a valid MD5 hash (32 hexadecimal characters).</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeMd5Hash(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (!string.IsNullOrEmpty(actual) && actual.Length == 32 && IsHexString(actual))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid MD5 hash (32 hex chars)", actual), expression);
    }

    /// <summary>Verifies that the string matches the hexadecimal length for <paramref name="type"/>.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    /// <param name="type">The expected hash algorithm format.</param>
    public static void BeHashedWith(this StringAssertions assertions, HashType type)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();
        var stripped = StripHexPrefix(actual);

        var expectedLength = type switch
        {
            HashType.Md5 => 32,
            HashType.Sha256 => 64,
            HashType.Sha512 => 128,
            _ => 0
        };

        if (stripped is not null && stripped.Length == expectedLength && IsHexString(stripped))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {expression} to be a valid {type} hash ({expectedLength} hex chars), but it was not.",
            expression);
    }

    /// <summary>Verifies that the string is a structurally valid JSON Web Token (three Base64Url segments).</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeJwtToken(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (JwtValidator.IsValid(actual ?? ""))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid JWT token", actual), expression);
    }

    /// <summary>Alias for <see cref="BeJwtToken"/>.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeJwt(this StringAssertions assertions) => BeJwtToken(assertions);

    /// <summary>Verifies that the string matches generic API key format (length 16–128, alphanumeric with <c>_</c> and <c>-</c>).</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeApiKey(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (IsValidApiKey(actual))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid API key", actual), expression);
    }

    /// <summary>Verifies that the string is a valid OAuth bearer token (optional <c>Bearer </c> prefix allowed).</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeOAuthToken(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (IsValidOAuthToken(actual))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid OAuth token", actual), expression);
    }

    private static string? StripHexPrefix(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            return value[2..];

        return value;
    }

    private static bool IsHexString(string value)
    {
        foreach (var c in value)
        {
            if (!IsHexChar(c))
                return false;
        }
        return true;
    }

    private static bool IsHexChar(char c) =>
        char.IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');

    private static bool IsValidApiKey(string? value)
    {
        if (string.IsNullOrEmpty(value) || value.Length < 16 || value.Length > 128)
            return false;

        if (!ApiKeyPattern().IsMatch(value))
            return false;

        var hasLetter = false;
        var hasDigit = false;
        foreach (var c in value)
        {
            if (char.IsLetter(c)) hasLetter = true;
            if (char.IsDigit(c)) hasDigit = true;
        }

        return hasLetter && hasDigit;
    }

    private static bool IsValidOAuthToken(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        var token = value;
        if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            token = token[7..];

        if (token.Length < 20)
            return false;

        return OAuthTokenPattern().IsMatch(token);
    }

    [GeneratedRegex(@"^[A-Za-z0-9_\-]+$")]
    private static partial Regex ApiKeyPattern();

    [GeneratedRegex(@"^[A-Za-z0-9\-._~+/]+=*$")]
    private static partial Regex OAuthTokenPattern();
}
