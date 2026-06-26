namespace OmniAssert.Extensions.Financials.Validators;

internal static class UlidValidator
{
    private static readonly char[] InvalidChars = ['I', 'L', 'O', 'U', 'i', 'l', 'o', 'u'];

    internal static bool IsValid(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length != 26)
            return false;

        foreach (var c in value)
        {
            if (!IsCrockfordBase32(c))
                return false;
        }

        return true;
    }

    private static bool IsCrockfordBase32(char c)
    {
        if (char.IsDigit(c))
            return true;

        var upper = char.ToUpper(c);
        if (upper < 'A' || upper > 'Z')
            return false;

        foreach (var invalid in InvalidChars)
        {
            if (char.ToUpper(invalid) == upper)
                return false;
        }

        return true;
    }
}
