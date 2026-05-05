using System.Text;

namespace OmniAssert;

internal static class StringFormatter
{
    public static string Quote(string? s)
    {
        if (s == null) return "null";
        var sb = new StringBuilder();
        sb.Append('"');
        foreach (char c in s) AppendVisible(sb, c);
        sb.Append('"');
        return sb.ToString();
    }

    public static void AppendVisible(StringBuilder sb, char c)
    {
        switch (c)
        {
            case '\"': sb.Append("\\\""); break;
            case '\\': sb.Append("\\\\"); break;
            case '\0': sb.Append("\\0"); break;
            case '\a': sb.Append("\\a"); break;
            case '\b': sb.Append("\\b"); break;
            case '\f': sb.Append("\\f"); break;
            case '\n': sb.Append("\\n"); break;
            case '\r': sb.Append("\\r"); break;
            case '\t': sb.Append("\\t"); break;
            case '\v': sb.Append("\\v"); break;
            default:
                if (char.IsControl(c) || IsZeroWidth(c))
                {
                    sb.Append($"\\u{(int)c:X4}");
                }
                else
                {
                    sb.Append(c);
                }
                break;
        }
    }

    public static string EscapeChar(char c) => c switch
    {
        '\'' => "\\'",
        '\\' => "\\\\",
        '\0' => "\\0",
        '\a' => "\\a",
        '\b' => "\\b",
        '\f' => "\\f",
        '\n' => "\\n",
        '\r' => "\\r",
        '\t' => "\\t",
        '\v' => "\\v",
        _ when char.IsControl(c) || IsZeroWidth(c) => $"\\u{(int)c:X4}",
        _ => c.ToString()
    };

    public static bool IsZeroWidth(char c) => c switch
    {
        '\u200B' or '\u200C' or '\u200D' or '\uFEFF' => true,
        _ => false
    };
}
