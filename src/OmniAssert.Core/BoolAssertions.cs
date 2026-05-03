using System.Text;

namespace OmniAssert;

/// <summary>Fluent assertions for a <see cref="bool"/> returned from <see cref="Assert.Verify(bool)"/>.</summary>
public readonly struct BoolAssertions
{
    private readonly bool _actual;
    private readonly string _expression;
    private readonly IReadOnlyDictionary<string, object?>? _capturedValues;

    internal BoolAssertions(bool actual, string expression, IReadOnlyDictionary<string, object?>? capturedValues = null)
    {
        _actual = actual;
        _expression = expression;
        _capturedValues = capturedValues;
    }

    /// <summary>Fails when the subject is <c>false</c>; respects <see cref="AssertionScope"/> or throws <see cref="OmniAssertionException"/>.</summary>
    public void ToBeTrue()
    {
        if (_actual)
            return;

        var msg = new StringBuilder();
        msg.AppendLine("Verification failed: expected expression to be true.");
        msg.Append("Subject ");
        msg.Append(_expression);
        msg.Append(": ");
        msg.Append(AnsiColour.Actual("false"));

        // Optional operand snapshot (e.g. from advanced lowering), not from default Verify(bool) path.
        if (_capturedValues?.Count > 0)
        {
            msg.AppendLine();
            msg.AppendLine("Context:");
            foreach (var pair in _capturedValues)
            {
                msg.Append("  ");
                msg.Append(pair.Key);
                msg.Append(" = ");
                msg.Append(OmniAssertionException.FormatValueForMessage(pair.Value));
                msg.AppendLine();
            }
        }

        VerificationFlow.Fail(msg.ToString(), _expression);
    }

    /// <summary>Fails when the subject is <c>true</c>; respects <see cref="AssertionScope"/> or throws <see cref="OmniAssertionException"/>.</summary>
    public void ToBeFalse()
    {
        if (!_actual)
            return;

        var msg = new StringBuilder();
        msg.AppendLine("Verification failed: expected expression to be false.");
        msg.Append("Subject ");
        msg.Append(_expression);
        msg.Append(": ");
        msg.Append(AnsiColour.Actual("true"));

        // Optional operand snapshot (e.g. from advanced lowering), not from default Verify(bool) path.
        if (_capturedValues?.Count > 0)
        {
            msg.AppendLine();
            msg.AppendLine("Context:");
            foreach (var pair in _capturedValues)
            {
                msg.Append("  ");
                msg.Append(pair.Key);
                msg.Append(" = ");
                msg.Append(OmniAssertionException.FormatValueForMessage(pair.Value));
                msg.AppendLine();
            }
        }

        VerificationFlow.Fail(msg.ToString(), _expression);
    }
}
