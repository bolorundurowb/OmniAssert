using System.Collections;
using System.Text;

namespace OmniAssert;

/// <summary>
/// Thrown when a verification fails and no <see cref="AssertionScope"/> is collecting failures.
/// Exposes structured <see cref="Capture"/> data (expression text and optional operand snapshots).
/// </summary>
/// <param name="message">Full human-readable failure text.</param>
/// <param name="capture">Expression label and optional operand map for diagnostics.</param>
/// <param name="innerException">Optional inner exception when this failure wraps another error.</param>
public sealed class OmniAssertionException(string message, AssertionCapture capture, Exception? innerException = null)
    : Exception(message, innerException)
{
    /// <summary>Structured data attached to this failure.</summary>
    public AssertionCapture Capture { get; } = capture;

    /// <summary>Shorthand for <see cref="AssertionCapture.SourceExpression"/>.</summary>
    public string SourceExpression => Capture.SourceExpression;

    /// <summary>Shorthand for <see cref="AssertionCapture.CapturedValues"/> when operand snapshots exist; otherwise <c>null</c>.</summary>
    public IReadOnlyDictionary<string, object?>? CapturedValues => Capture.CapturedValues;

    internal static string FormatValueForMessage(object? value) => value switch
    {
        null => "null",
        string s => StringFormatter.Quote(s),
        char c => "'" + StringFormatter.EscapeChar(c) + "'",
        IEnumerable en => FormatCollection(en),
        IFormattable f => f.ToString(null, System.Globalization.CultureInfo.InvariantCulture),
        _ => value.ToString() ?? "null"
    };

    private static string FormatCollection(IEnumerable en)
    {
        var sb = new StringBuilder();
        sb.Append('[');
        var first = true;
        var count = 0;
        foreach (var item in en)
        {
            if (count >= 10)
            {
                sb.Append(", ...");
                break;
            }
            if (!first) sb.Append(", ");
            sb.Append(FormatValueForMessage(item));
            first = false;
            count++;
        }
        sb.Append(']');
        return sb.ToString();
    }

    /// <summary>Builds an exception for a failed boolean <see cref="Ensure.Expression"/> path.</summary>
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
