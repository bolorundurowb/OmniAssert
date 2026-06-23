using System.Runtime.CompilerServices;
using System.Text;

namespace OmniAssert;

/// <summary>Fluent assertions for a <see cref="bool"/> returned from <see cref="Assert.Verify(bool)"/>.</summary>
/// <remarks>Failures respect an enclosing <see cref="AssertionScope"/> when present; otherwise they throw <see cref="OmniAssertionException"/>.</remarks>
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

    /// <summary>Fails when the subject is <c>false</c>.</summary>
    /// <exception cref="OmniAssertionException">Thrown when the subject is false and no <see cref="AssertionScope"/> is collecting failures.</exception>
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
        AppendOperandContext(msg);
        VerificationFlow.Fail(msg.ToString(), _expression);
    }

    /// <summary>Fails when the subject is <c>true</c>.</summary>
    /// <exception cref="OmniAssertionException">Thrown when the subject is true and no <see cref="AssertionScope"/> is collecting failures.</exception>
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
        AppendOperandContext(msg);
        VerificationFlow.Fail(msg.ToString(), _expression);
    }

    /// <summary>Verifies that the subject is equal to the <paramref name="expected"/> boolean value.</summary>
    /// <param name="expected">The expected boolean value.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBe(bool expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual == expected)
            return;

        var msg = new StringBuilder();
        msg.AppendLine($"Verification failed: expected {_expression} to be {expectedExpression ?? "expected"} ({AnsiColour.Expected(expected.ToString().ToLowerInvariant())}), but was {AnsiColour.Actual(_actual.ToString().ToLowerInvariant())}.");
        AppendOperandContext(msg);
        VerificationFlow.Fail(msg.ToString(), _expression);
    }

    /// <summary>Operand snapshots from lowered <c>VerifyExpression</c>; unused on the default <c>Verify(bool)</c> path.</summary>
    private void AppendOperandContext(StringBuilder msg)
    {
        if (_capturedValues is not { Count: > 0 })
            return;

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
}
