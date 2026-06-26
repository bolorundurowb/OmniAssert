namespace OmniAssert.Extensions.Regional.Validators;

internal static class NubanValidator
{
    internal static bool IsValid(string accountNumber, string bankCode)
    {
        if (string.IsNullOrEmpty(accountNumber) || accountNumber.Length != 10 || !accountNumber.All(char.IsDigit))
            return false;

        if (string.IsNullOrEmpty(bankCode) || bankCode.Length != 3 || !bankCode.All(char.IsDigit))
            return false;

        var combined = accountNumber + bankCode;
        var sum = 0;

        for (var i = 0; i < combined.Length; i++)
        {
            var weight = (i % 9) + 1;
            sum += (combined[i] - '0') * weight;
        }

        return sum % 10 == 0;
    }

    internal static bool IsValidFormat(string accountNumber)
    {
        return !string.IsNullOrEmpty(accountNumber)
               && accountNumber.Length == 10
               && accountNumber.All(char.IsDigit);
    }
}
