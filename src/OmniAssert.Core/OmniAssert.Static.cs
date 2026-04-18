using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>Entry points for OmniAssert verifications.</summary>
public static partial class Assert
{
    /// <summary>
    /// Verifies that a boolean condition holds. Prefer this form in user code; the OmniAssert build task rewrites call sites to capture operand values.
    /// </summary>
    public static void Verify(bool condition, [CallerArgumentExpression(nameof(condition))] string? expression = null)
    {
        var capture = new AssertionCapture(expression ?? "condition", null);
        VerifyBoolean(condition, in capture);
    }

    /// <summary>
    /// Invoked from rewritten code with structured captures. Not intended for direct use.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void VerifyBoolean(bool result, in AssertionCapture capture)
    {
        if (result)
            return;

        var ex = OmniAssertionException.ForBooleanFailure(in capture);
        var ctx = AssertionScope.Current;
        if (ctx is not null)
        {
            ctx.Failures.Add(ex);
            return;
        }

        throw ex;
    }

    /// <summary>Begins verifying a boolean subject with <see cref="BoolAssertions.ToBeTrue"/> / <see cref="BoolAssertions.ToBeFalse"/>.</summary>
    public static BoolAssertions VerifyBool(bool actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying an integer subject.</summary>
    public static IntAssertions Verify(int actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a string subject.</summary>
    public static StringAssertions Verify(string? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a collection subject.</summary>
    public static CollectionAssertions<T> Verify<T>(IEnumerable<T> actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying an enum subject.</summary>
    public static EnumAssertions<T> Verify<T>(T actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) where T : struct, Enum =>
        new(actual, expression ?? "actual");

    /// <summary>Compares two objects by walking public properties and reports a structured diff on mismatch.</summary>
    public static void VerifyEquivalent(object? expected, object? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null)
    {
        var diff = ObjectDiffWalker.Diff(expected, actual, expression ?? "actual");
        if (diff is null)
            return;

        var message = diff.FormatMessage();
        var capture = new AssertionCapture(expression ?? "actual", null);
        var ex = new OmniAssertionException(message, capture);
        var ctx = AssertionScope.Current;
        if (ctx is not null)
        {
            ctx.Failures.Add(ex);
            return;
        }

        throw ex;
    }
}
