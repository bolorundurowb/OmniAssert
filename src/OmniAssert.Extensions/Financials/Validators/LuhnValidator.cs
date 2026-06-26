namespace OmniAssert.Extensions.Financials.Validators;

internal static class LuhnValidator
{
    internal static bool IsValid(string digits)
    {
        if (string.IsNullOrEmpty(digits))
            return false;

        var sum = 0;
        var alternate = false;

        for (var i = digits.Length - 1; i >= 0; i--)
        {
            if (!char.IsDigit(digits[i]))
                return false;

            var n = digits[i] - '0';

            if (alternate)
            {
                n *= 2;
                if (n > 9)
                    n -= 9;
            }

            sum += n;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }
}
