using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>Generic assertions for object subjects.</summary>
public readonly struct ObjectAssertions
{
    private readonly object? _actual;
    private readonly string _expression;

    internal ObjectAssertions(object? actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    /// <summary>Verifies that the actual object is of the specified type.</summary>
    public void ToBeOfType<T>()
    {
        if (_actual is T && _actual.GetType() == typeof(T))
            return;

        var actualType = _actual?.GetType().Name ?? "null";
        VerificationFlow.Fail($"Verification failed: expected {_expression} to be of type {typeof(T).Name}, but was {actualType}.", _expression);
    }

    /// <summary>Verifies that the actual object is assignable to the specified type.</summary>
    public void ToBeAssignableTo<T>()
    {
        if (_actual is T)
            return;

        var actualType = _actual?.GetType().Name ?? "null";
        VerificationFlow.Fail($"Verification failed: expected {_expression} to be assignable to {typeof(T).Name}, but was {actualType}.", _expression);
    }

    /// <summary>Compares the actual object to the expected object by walking public properties and reports a structured diff on mismatch.</summary>
    /// <param name="expected">The expected object state.</param>
    public void ToBeEquivalentTo(object? expected)
    {
        var diff = ObjectDiffWalker.Diff(expected, _actual, _expression);
        if (diff is null)
            return;

        var message = diff.FormatMessage();
        var capture = new AssertionCapture(_expression, null);
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
