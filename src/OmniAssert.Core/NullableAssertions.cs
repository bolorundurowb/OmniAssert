namespace OmniAssert;

/// <summary>Assertions for <see cref="Nullable{T}"/> value types from the nullable value <see cref="Assert"/> entry points.</summary>
/// <typeparam name="T">The underlying non-nullable struct.</typeparam>
public readonly struct NullableValueAssertions<T> where T : struct
{
    private readonly T? _actual;
    private readonly string _expression;

    internal NullableValueAssertions(T? actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    /// <summary>Asserts the nullable has no value; fails when it has a value.</summary>
    public void BeNull()
    {
        if (!_actual.HasValue)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be null, but was {FormatValue(_actual.Value)}.", _expression);
    }

    /// <summary>Asserts the nullable has a value; fails when it is null.</summary>
    public void NotBeNull()
    {
        if (_actual.HasValue)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} not to be null.", _expression);
    }

    private static string FormatValue(T value) => OmniAssertionException.FormatValueForMessage(value!);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeNull() => BeNull();
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void NotToBeNull() => NotBeNull();
}

/// <summary>Assertions for nullable reference types from the nullable reference <see cref="Assert"/> entry points.</summary>
/// <typeparam name="T">The underlying non-nullable class.</typeparam>
public readonly struct NullableReferenceAssertions<T> where T : class
{
    private readonly T? _actual;
    private readonly string _expression;

    internal NullableReferenceAssertions(T? actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    /// <summary>Asserts the reference is <c>null</c>; fails when it has a value.</summary>
    public void BeNull()
    {
        if (_actual is null)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} to be null, but was {FormatValue(_actual)}.", _expression);
    }

    /// <summary>Asserts the reference is not <c>null</c>; fails when it is null.</summary>
    public void NotBeNull()
    {
        if (_actual is not null)
            return;

        VerificationFlow.Fail($"Verification failed: expected {_expression} not to be null.", _expression);
    }

    private static string FormatValue(T? value) => OmniAssertionException.FormatValueForMessage(value!);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeNull() => BeNull();
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void NotToBeNull() => NotBeNull();
}
