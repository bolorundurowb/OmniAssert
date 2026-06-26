using System.Text.RegularExpressions;
using OmniAssert.Extensions.Internal;
using OmniAssert.Extensions.Regional.Validators;

namespace OmniAssert.Extensions.Regional;

/// <summary>Regional, demographic, and business-rule assertions for <see cref="StringAssertions"/> subjects.</summary>
public static partial class StringRegionalExtensions
{
    /// <summary>Verifies that the string is a valid Nigerian mobile phone number (<c>+234</c> or leading <c>0</c>).</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeNigerianPhoneNumber(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (NigerianPhoneValidator.IsValid(actual ?? ""))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid Nigerian phone number", actual), expression);
    }

    /// <summary>Verifies that the string is a 10-digit Nigerian bank account number (format only).</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeNigerianBankAccountNumber(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (NubanValidator.IsValidFormat(actual ?? ""))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid Nigerian bank account number (10 digits)", actual), expression);
    }

    /// <summary>
    /// Verifies that the string is a valid NUBAN account number for the given 3-digit <paramref name="bankCode"/>
    /// (includes check-digit validation).
    /// </summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    /// <param name="bankCode">Three-digit Nigerian bank code.</param>
    public static void BeNigerianBankAccountNumber(this StringAssertions assertions, string bankCode)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (NubanValidator.IsValid(actual ?? "", bankCode))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {expression} to be a valid Nigerian bank account number for bank code {bankCode}, but it was not.",
            expression);
    }

    /// <summary>Verifies that the string is a valid 11-digit Nigerian Bank Verification Number (BVN).</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeNigerianBvn(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (IsValidNigerianId(actual, 11))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid Nigerian BVN (11 digits)", actual), expression);
    }

    /// <summary>Verifies that the string is a valid 11-digit Nigerian National Identification Number (NIN).</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeNigerianNin(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (IsValidNigerianId(actual, 11))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid Nigerian NIN (11 digits)", actual), expression);
    }

    /// <summary>Verifies that the string is a valid 6-digit Nigerian postal code.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeNigerianPostalCode(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (!string.IsNullOrEmpty(actual) && actual.Length == 6 && actual.All(char.IsDigit))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid Nigerian postal code (6 digits)", actual), expression);
    }

    /// <summary>Verifies that the string is a valid E.164 international phone number.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BePhoneNumber(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (IsValidE164(actual))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid E.164 phone number", actual), expression);
    }

    /// <summary>Verifies that the string is a valid postal code for <paramref name="country"/>.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    /// <param name="country">Country-specific postal format (default <see cref="CountryCode.Generic"/>).</param>
    public static void BePostalCode(this StringAssertions assertions, CountryCode country = CountryCode.Generic)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (IsValidPostalCode(actual, country))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, $"to be a valid {country} postal code", actual), expression);
    }

    /// <summary>Verifies that the string is a valid United States Social Security Number.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeSocialSecurityNumber(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (!string.IsNullOrEmpty(actual) && SsnPattern().IsMatch(actual))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid US Social Security Number", actual), expression);
    }

    private static bool IsValidNigerianId(string? value, int expectedLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length != expectedLength || !value.All(char.IsDigit))
            return false;

        var firstDigit = value[0];
        var allSame = true;
        foreach (var c in value)
        {
            if (c != firstDigit)
            {
                allSame = false;
                break;
            }
        }

        return !allSame;
    }

    private static bool IsValidE164(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        var digits = value.StartsWith('+') ? value[1..] : value;

        if (!digits.All(char.IsDigit))
            return false;

        return digits.Length >= 8 && digits.Length <= 15;
    }

    private static bool IsValidPostalCode(string? value, CountryCode country)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        return country switch
        {
            CountryCode.US => UsPostalPattern().IsMatch(value),
            CountryCode.GB => GbPostalPattern().IsMatch(value),
            CountryCode.CA => CaPostalPattern().IsMatch(value),
            CountryCode.NG => !string.IsNullOrEmpty(value) && value.Length == 6 && value.All(char.IsDigit),
            CountryCode.Generic => GenericPostalPattern().IsMatch(value),
            _ => false
        };
    }

    [GeneratedRegex(@"^(?!000|666|9\d{2})\d{3}-?(?!00)\d{2}-?(?!0000)\d{4}$")]
    private static partial Regex SsnPattern();

    [GeneratedRegex(@"^\d{5}(-\d{4})?$")]
    private static partial Regex UsPostalPattern();

    [GeneratedRegex(@"^[A-Z]{1,2}\d[A-Z\d]?\s*\d[A-Z]{2}$", RegexOptions.IgnoreCase)]
    private static partial Regex GbPostalPattern();

    [GeneratedRegex(@"^[A-Za-z]\d[A-Za-z]\s?\d[A-Za-z]\d$")]
    private static partial Regex CaPostalPattern();

    [GeneratedRegex(@"^[A-Za-z0-9][A-Za-z0-9\s\-]{1,8}[A-Za-z0-9]$")]
    private static partial Regex GenericPostalPattern();
}

/// <summary>Country codes used by <see cref="StringRegionalExtensions.BePostalCode"/>.</summary>
public enum CountryCode
{
    /// <summary>United States ZIP code (<c>#####</c> or <c>#####-####</c>).</summary>
    US,

    /// <summary>United Kingdom postcode.</summary>
    GB,

    /// <summary>Canadian postal code.</summary>
    CA,

    /// <summary>Nigerian 6-digit postal code.</summary>
    NG,

    /// <summary>Generic alphanumeric postal format (3–10 characters).</summary>
    Generic
}
