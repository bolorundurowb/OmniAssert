using OmniAssert.Extensions.Financials.Validators;
using OmniAssert.Extensions.Internal;

namespace OmniAssert.Extensions.Financials;

/// <summary>Financial and industry-identifier assertions for <see cref="StringAssertions"/> subjects.</summary>
public static class StringFinancialExtensions
{
    /// <summary>
    /// Verifies that the string is a valid GUID. When <paramref name="format"/> is specified, the string must match
    /// that layout exactly (see <see cref="GuidFormat"/>).
    /// </summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    /// <param name="format">Optional required GUID string layout.</param>
    public static void BeGuid(this StringAssertions assertions, GuidFormat? format = null)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (IsValidGuidString(actual, format))
            return;

        var formatHint = format is null ? "" : $" in {format} format";
        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, $"to be a valid GUID string{formatHint}", actual), expression);
    }

    /// <summary>Verifies that the string is a valid ULID (26 Crockford base32 characters).</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeUlid(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (UlidValidator.IsValid(actual ?? ""))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid ULID", actual), expression);
    }

    /// <summary>Verifies that the string is a valid CUID v1 identifier.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeCuid(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (CuidValidator.IsValid(actual ?? ""))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid CUID", actual), expression);
    }

    /// <summary>Verifies that the string is a valid ISBN-10 or ISBN-13 (including check digit).</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeIsbn(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (IsbnValidator.IsValid(actual ?? ""))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid ISBN", actual), expression);
    }

    /// <summary>Verifies that the string is a valid 15-digit IMEI (Luhn check included).</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeImei(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (IsValidImei(actual))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid IMEI", actual), expression);
    }

    /// <summary>Verifies that the string is a valid credit card number (13–19 digits, Luhn-valid).</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeCreditCard(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (IsValidCreditCard(actual))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid credit card number", actual), expression);
    }

    /// <summary>Alias for <see cref="BeCreditCard"/>.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeCreditCardNumber(this StringAssertions assertions) => BeCreditCard(assertions);

    /// <summary>Verifies that the string is a valid IBAN (mod-97 validation).</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeIban(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (IbanValidator.IsValid(actual ?? ""))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid IBAN", actual), expression);
    }

    private static bool IsValidGuidString(string? value, GuidFormat? format)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (!Guid.TryParse(value, out var guid))
            return false;

        if (format is null)
            return true;

        var formatString = format.Value switch
        {
            GuidFormat.N => "N",
            GuidFormat.D => "D",
            GuidFormat.B => "B",
            GuidFormat.P => "P",
            GuidFormat.X => "X",
            _ => "D"
        };

        return string.Equals(guid.ToString(formatString), value, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsValidImei(string? value)
    {
        if (string.IsNullOrEmpty(value) || value.Length != 15 || !value.All(char.IsDigit))
            return false;

        return LuhnValidator.IsValid(value);
    }

    private static bool IsValidCreditCard(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var digits = new string(value.Where(char.IsDigit).ToArray());

        if (digits.Length < 13 || digits.Length > 19)
            return false;

        return LuhnValidator.IsValid(digits);
    }
}
