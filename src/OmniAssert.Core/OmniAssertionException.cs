using System.Text;

namespace OmniAssert;

/// <summary>
/// Thrown when a verification fails and no <see cref="AssertionScope"/> is collecting failures. Carries
/// <see cref="Capture"/> so callers can read <see cref="AssertionCapture.SourceExpression"/> and optional operands.
/// </summary>
public sealed class OmniAssertionException(string message, AssertionCapture capture, Exception? innerException = null)
    : Exception(message, innerException)
{
    public AssertionCapture Capture { get; } = capture;

    public string SourceExpression => Capture.SourceExpression;

    public IReadOnlyDictionary<string, object?>? CapturedValues => Capture.CapturedValues;

    internal static string FormatValueForMessage(object? value) => value switch
    {
        null => "null",
        string s => StringFormatter.Quote(s),
        char c => "'" + StringFormatter.EscapeChar(c) + "'",
        IFormattable f => f.ToString(null, System.Globalization.CultureInfo.InvariantCulture),
        _ => value.ToString() ?? "null"
    };

    /// <summary>Builds an exception for a failed <see cref="Assert.VerifyBoolean"/> / <see cref="Assert.VerifyExpression"/> path.</summary>
    internal static OmniAssertionException ForBooleanFailure(in AssertionCapture capture)
    {
        var message = FormatBooleanFailure(capture);
        return new OmniAssertionException(message, capture);
    }

    /// <summary>Formats the multi-line message listing the expression and any <see cref="AssertionCapture.CapturedValues"/>.</summary>
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
