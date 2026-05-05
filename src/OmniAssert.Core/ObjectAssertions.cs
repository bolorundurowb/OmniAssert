using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>Assertions for arbitrary <see cref="object"/> (or boxed) subjects from <see cref="Assert.Verify(object?, string?)"/>.</summary>
public readonly struct ObjectAssertions
{
    private readonly object? _actual;
    private readonly string _expression;

    internal ObjectAssertions(object? actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    /// <summary>Asserts the runtime type is exactly <typeparamref name="T"/> (not a derived type).</summary>
    public void ToBeOfType<T>()
    {
        if (_actual is T && _actual.GetType() == typeof(T))
            return;

        var actualType = _actual?.GetType().Name ?? "null";
        VerificationFlow.Fail($"Verification failed: expected {_expression} to be of type {typeof(T).Name}, but was {actualType}.", _expression);
    }

    /// <summary>Asserts the subject is assignable to <typeparamref name="T"/> (including derived instances).</summary>
    public void ToBeAssignableTo<T>()
    {
        if (_actual is T)
            return;

        var actualType = _actual?.GetType().Name ?? "null";
        VerificationFlow.Fail($"Verification failed: expected {_expression} to be assignable to {typeof(T).Name}, but was {actualType}.", _expression);
    }

    /// <summary>
    /// Deep structural comparison: walks public instance properties and sequences, then reports a coloured diff on mismatch.
    /// </summary>
    /// <param name="expected">Expected graph (order of enumerable elements must match).</param>
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
