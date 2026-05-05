namespace OmniAssert;

/// <summary>ANSI escape sequences for colouring expected vs actual values in failure output.</summary>
/// <remarks>Colour is suppressed when the <c>NO_COLOR</c> environment variable is set, when <c>TERM=dumb</c>, or on non-Windows hosts without a <c>TERM</c> variable.</remarks>
internal static class AnsiColour
{
    /// <summary>Resets SGR attributes to the terminal default.</summary>
    public const string Reset = "\u001b[0m";

    /// <summary>Red foreground (used for “actual” values).</summary>
    public const string Red = "\u001b[31m";

    /// <summary>Green foreground (used for “expected” values).</summary>
    public const string Green = "\u001b[32m";

    /// <summary>Dim intensity.</summary>
    public const string Dim = "\u001b[2m";

    /// <summary><c>true</c> when the library will emit colour codes (subject to environment rules).</summary>
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

    /// <summary>Wraps <paramref name="text"/> in the “expected” colour when <see cref="UseColour"/> is true.</summary>
    public static string Expected(string text) => Wrap(Green, text);

    /// <summary>Wraps <paramref name="text"/> in the “actual” colour when <see cref="UseColour"/> is true.</summary>
    public static string Actual(string text) => Wrap(Red, text);

    /// <summary>Wraps <paramref name="text"/> with <paramref name="code"/> when <see cref="UseColour"/> is true.</summary>
    public static string Wrap(string code, string text)
    {
        if (!UseColour || string.IsNullOrEmpty(text))
            return text;
        return code + text + Reset;
    }
}
