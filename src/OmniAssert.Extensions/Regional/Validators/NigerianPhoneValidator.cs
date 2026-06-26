namespace OmniAssert.Extensions.Regional.Validators;

internal static class NigerianPhoneValidator
{
    private static readonly string[] MobilePrefixes =
    [
        "701", "702", "703", "704", "705", "706", "707", "708", "709",
        "801", "802", "803", "804", "805", "806", "807", "808", "809",
        "810", "811", "812", "813", "814", "815", "816", "817", "818", "819",
        "901", "902", "903", "904", "905", "906", "907", "908", "909",
        "912", "913"
    ];

    internal static bool IsValid(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        var normalized = phoneNumber.Replace(" ", "").Replace("-", "");

        if (normalized.StartsWith("+234"))
        {
            normalized = "0" + normalized[4..];
        }
        else if (normalized.StartsWith("234") && normalized.Length == 13)
        {
            normalized = "0" + normalized[3..];
        }

        if (!normalized.StartsWith("0") || normalized.Length != 11)
            return false;

        if (!normalized.All(char.IsDigit))
            return false;

        var prefix = normalized[1..4];
        foreach (var p in MobilePrefixes)
        {
            if (p == prefix)
                return true;
        }

        return false;
    }
}
