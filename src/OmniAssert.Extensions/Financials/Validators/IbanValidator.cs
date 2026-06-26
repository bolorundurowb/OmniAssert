namespace OmniAssert.Extensions.Financials.Validators;

internal static class IbanValidator
{
    internal static bool IsValid(string iban)
    {
        if (string.IsNullOrEmpty(iban) || iban.Length < 15 || iban.Length > 34)
            return false;

        var country = iban.AsSpan(0, 2);
        if (!char.IsLetter(country[0]) || !char.IsLetter(country[1]))
            return false;

        for (var i = 2; i < iban.Length; i++)
        {
            if (!char.IsLetterOrDigit(iban[i]))
                return false;
        }

        var rearranged = iban.AsSpan(4).ToString() + iban.AsSpan(0, 4).ToString();

        var numericChars = new char[rearranged.Length * 2];
        var idx = 0;
        foreach (var c in rearranged)
        {
            if (char.IsDigit(c))
            {
                numericChars[idx++] = c;
            }
            else if (char.IsLetter(c))
            {
                var val = char.ToUpper(c) - 'A' + 10;
                var s = val.ToString();
                foreach (var ch in s)
                    numericChars[idx++] = ch;
            }
            else
            {
                return false;
            }
        }

        var numericStr = new string(numericChars, 0, idx);

        var remainder = 0;
        for (var i = 0; i < numericStr.Length; i++)
        {
            remainder = (remainder * 10 + (numericStr[i] - '0')) % 97;
        }

        return remainder == 1;
    }
}
