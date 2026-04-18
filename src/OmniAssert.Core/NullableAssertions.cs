using System.Runtime.CompilerServices;

namespace OmniAssert;

public readonly struct NullableValueAssertions<T> where T : struct
{
    private readonly T? _actual;
    private readonly string _expression;

    internal NullableValueAssertions(T? actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    public void ToBeNull()
    {
        if (!_actual.HasValue)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be null, but was {FormatValue(_actual.Value)}.", _expression);
    }

    public void NotToBeNull()
    {
        if (_actual.HasValue)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} not to be null.", _expression);
    }

    private static string FormatValue(T value) => OmniAssertionException.FormatValueForMessage(value!);
}


public readonly struct NullableReferenceAssertions<T> where T : class
{
    private readonly T? _actual;
    private readonly string _expression;

    internal NullableReferenceAssertions(T? actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    public void ToBeNull()
    {
        if (_actual is null)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be null, but was {FormatValue(_actual)}.", _expression);
    }

    public void NotToBeNull()
    {
        if (_actual is not null)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} not to be null.", _expression);
    }

    private static string FormatValue(T? value) => OmniAssertionException.FormatValueForMessage(value!);
}
