namespace OmniAssert;

/// <summary>ANSI SGR codes for terminal highlighting. Respects NO_COLOR.</summary>
public static class AnsiColour
{
    public const string Reset = "\u001b[0m";
    public const string Red = "\u001b[31m";
    public const string Green = "\u001b[32m";
    public const string Dim = "\u001b[2m";

    public static bool UseColour { get; } = ComputeUseColour();

    private static bool ComputeUseColour()
    {
        var noColor = Environment.GetEnvironmentVariable("NO_COLOR");
        if (!string.IsNullOrEmpty(noColor))
            return false;
        if (Environment.GetEnvironmentVariable("TERM") == "dumb")
            return false;
        if (OperatingSystem.IsWindows())
            return true;
        return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TERM"));
    }

    public static string Expected(string text) => Wrap(Green, text);

    public static string Actual(string text) => Wrap(Red, text);

    public static string Wrap(string code, string text)
    {
        if (!UseColour || string.IsNullOrEmpty(text))
            return text;
        return code + text + Reset;
    }
}
