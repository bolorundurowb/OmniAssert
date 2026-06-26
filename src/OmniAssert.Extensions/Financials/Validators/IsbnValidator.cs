namespace OmniAssert.Extensions.Financials.Validators;

internal static class IsbnValidator
{
    internal static bool IsValid(string isbn)
    {
        if (string.IsNullOrEmpty(isbn))
            return false;

        var cleaned = isbn.Replace("-", "").Replace(" ", "");

        return cleaned.Length switch
        {
            10 => IsValidIsbn10(cleaned),
            13 => IsValidIsbn13(cleaned),
            _ => false
        };
    }

    private static bool IsValidIsbn10(string isbn)
    {
        var sum = 0;
        for (var i = 0; i < 9; i++)
        {
            if (!char.IsDigit(isbn[i]))
                return false;
            sum += (isbn[i] - '0') * (10 - i);
        }

        var last = isbn[9];
        if (last == 'X' || last == 'x')
        {
            sum += 10;
        }
        else if (char.IsDigit(last))
        {
            sum += last - '0';
        }
        else
        {
            return false;
        }

        return sum % 11 == 0;
    }

    private static bool IsValidIsbn13(string isbn)
    {
        if (isbn[0] != '9' || (isbn[1] != '7' && isbn[1] != '8' && isbn[1] != '9'))
            return false;

        if (!isbn.All(char.IsDigit))
            return false;

        var sum = 0;
        for (var i = 0; i < 12; i++)
        {
            var digit = isbn[i] - '0';
            sum += i % 2 == 0 ? digit : digit * 3;
        }

        var check = (10 - sum % 10) % 10;
        return check == isbn[12] - '0';
    }
}
