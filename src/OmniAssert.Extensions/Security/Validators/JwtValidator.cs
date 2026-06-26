namespace OmniAssert.Extensions.Security.Validators;

internal static class JwtValidator
{
    internal static bool IsValid(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        var parts = token.Split('.');
        if (parts.Length != 3)
            return false;

        foreach (var part in parts)
        {
            if (string.IsNullOrEmpty(part))
                return false;
            if (!IsValidBase64Url(part))
                return false;
        }

        try
        {
            var headerJson = DecodeBase64Url(parts[0]);
            if (!headerJson.Contains("\"alg\""))
                return false;
        }
        catch
        {
            return false;
        }

        try
        {
            var payloadJson = DecodeBase64Url(parts[1]);
            if (!payloadJson.TrimStart().StartsWith("{"))
                return false;
        }
        catch
        {
            return false;
        }

        return true;
    }

    private static bool IsValidBase64Url(string value)
    {
        foreach (var c in value)
        {
            if (!char.IsLetterOrDigit(c) && c != '-' && c != '_')
                return false;
        }
        return true;
    }

    private static string DecodeBase64Url(string value)
    {
        var padded = value.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4)
        {
            case 2: padded += "=="; break;
            case 3: padded += "="; break;
        }
        var bytes = Convert.FromBase64String(padded);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}
