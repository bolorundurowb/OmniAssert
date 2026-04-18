using System.Runtime.CompilerServices;

namespace OmniAssert;

public readonly struct TypeAssertions
{
    private readonly object? _actual;
    private readonly string _expression;

    internal TypeAssertions(object? actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    public void ToBeOfType<T>()
    {
        if (_actual is T && _actual.GetType() == typeof(T))
            return;

        var actualType = _actual?.GetType().Name ?? "null";
        VerificationFlow.Fail($"Verification failed: expected {_expression} to be of type {typeof(T).Name}, but was {actualType}.", _expression);
    }

    public void ToBeAssignableTo<T>()
    {
        if (_actual is T)
            return;

        var actualType = _actual?.GetType().Name ?? "null";
        VerificationFlow.Fail($"Verification failed: expected {_expression} to be assignable to {typeof(T).Name}, but was {actualType}.", _expression);
    }
}
