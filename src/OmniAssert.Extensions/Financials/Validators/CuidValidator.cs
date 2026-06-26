using System.Text.RegularExpressions;

namespace OmniAssert.Extensions.Financials.Validators;

internal static partial class CuidValidator
{
    internal static bool IsValid(string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        if (value[0] != 'c')
            return false;

        if (value.Length < 24 || value.Length > 25)
            return false;

        return CuidPattern().IsMatch(value);
    }

    [GeneratedRegex("^c[0-9a-z]{23,24}$")]
    private static partial Regex CuidPattern();
}
