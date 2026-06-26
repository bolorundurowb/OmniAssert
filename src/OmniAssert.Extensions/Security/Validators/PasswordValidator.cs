namespace OmniAssert.Extensions.Security.Validators;

internal static class PasswordValidator
{
    private const string Symbols = "!@#$%^&*(),.?\":{}|<>_\\-+=[]\\\\;/";

    internal static bool IsStrong(string password, int minLength)
    {
        if (string.IsNullOrEmpty(password) || password.Length < minLength)
            return false;

        var hasUpper = false;
        var hasLower = false;
        var hasDigit = false;
        var hasSymbol = false;

        foreach (var c in password)
        {
            if (char.IsUpper(c)) hasUpper = true;
            else if (char.IsLower(c)) hasLower = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else if (Symbols.Contains(c)) hasSymbol = true;
        }

        return hasUpper && hasLower && hasDigit && hasSymbol;
    }
}
