using System.Text;

namespace OmniAssert;

/// <summary>Thrown when a verification fails outside of a soft-assertion scope.</summary>
public sealed class OmniAssertionException : Exception
{
    public OmniAssertionException(string message, AssertionCapture capture, Exception? innerException = null)
        : base(message, innerException)
    {
        Capture = capture;
    }

    public AssertionCapture Capture { get; }

    public string SourceExpression => Capture.SourceExpression;

    public IReadOnlyDictionary<string, object?>? CapturedValues => Capture.CapturedValues;

    internal static string FormatValueForMessage(object? value) => value switch
    {
        null => "null",
        string s => "\"" + s.Replace("\"", "\\\"", StringComparison.Ordinal) + "\"",
        char c => "'" + c + "'",
        IFormattable f => f.ToString(null, System.Globalization.CultureInfo.InvariantCulture),
        _ => value.ToString() ?? "null"
    };

    internal static OmniAssertionException ForBooleanFailure(in AssertionCapture capture)
    {
        var message = FormatBooleanFailure(capture);
        return new OmniAssertionException(message, capture);
    }

    internal static string FormatBooleanFailure(in AssertionCapture capture)
    {
        var sb = new StringBuilder();
        sb.Append("Verification failed. Expression evaluated to false.");
        if (!string.IsNullOrWhiteSpace(capture.SourceExpression))
        {
            sb.AppendLine();
            sb.Append("Expression: ");
            sb.Append(capture.SourceExpression);
        }

        if (capture.CapturedValues is { Count: > 0 })
        {
            sb.AppendLine();
            sb.AppendLine("Captured values:");
            foreach (var pair in capture.CapturedValues)
            {
                sb.Append("  ");
                sb.Append(pair.Key);
                sb.Append(": ");
                sb.Append(FormatValue(pair.Value));
                sb.AppendLine();
            }
        }

        return sb.ToString().TrimEnd();
    }

    private static string FormatValue(object? value) => FormatValueForMessage(value);
}
