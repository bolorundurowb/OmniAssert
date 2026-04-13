using System.Text;

namespace OmniAssert;

/// <summary>Use <see cref="Assert.VerifyBool"/> for fluent true/false checks; <see cref="Assert.Verify(bool)"/> is for arbitrary boolean conditions (optionally rewritten by the build task).</summary>
public readonly struct BoolAssertions
{
    private readonly bool _actual;
    private readonly string _expression;

    internal BoolAssertions(bool actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

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
        VerificationFlow.Fail(msg.ToString(), _expression);
    }

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
        VerificationFlow.Fail(msg.ToString(), _expression);
    }
}
