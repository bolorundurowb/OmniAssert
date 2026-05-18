using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>Assertions for dictionary subjects from <see cref="Assert.Verify{TKey, TValue}(IReadOnlyDictionary{TKey, TValue}, string?)"/>.</summary>
/// <typeparam name="TKey">Dictionary key type.</typeparam>
/// <typeparam name="TValue">Dictionary value type.</typeparam>
public readonly struct DictionaryAssertions<TKey, TValue>
{
    private readonly IReadOnlyDictionary<TKey, TValue> _actual;
    private readonly string _expression;

    internal DictionaryAssertions(IReadOnlyDictionary<TKey, TValue>? actual, string expression)
    {
        _actual = actual!;
        _expression = expression;
    }

    /// <summary>Verifies that the dictionary is the same instance as <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected dictionary instance.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void ToBe(IReadOnlyDictionary<TKey, TValue>? expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (ReferenceEquals(_actual, expected))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be {expectedExpression ?? "expected"} (reference equality), but they were different instances.",
            _expression);
    }

    /// <summary>Verifies that the dictionary contains <paramref name="key"/>.</summary>
    /// <param name="key">The key expected to be present.</param>
    /// <param name="keyExpression">The expression for the key (automatically captured).</param>
    public void ContainKey(TKey key, [CallerArgumentExpression(nameof(key))] string? keyExpression = null)
    {
        EnsureActualNotNull();
        if (_actual.ContainsKey(key))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to contain key {keyExpression ?? "key"} ({FormatItem(key)}), but it did not.",
            _expression);
    }

    /// <summary>Verifies that the dictionary does not contain <paramref name="key"/>.</summary>
    /// <param name="key">The key expected to be absent.</param>
    /// <param name="keyExpression">The expression for the key (automatically captured).</param>
    public void NotContainKey(TKey key, [CallerArgumentExpression(nameof(key))] string? keyExpression = null)
    {
        EnsureActualNotNull();
        if (!_actual.ContainsKey(key))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} not to contain key {keyExpression ?? "key"} ({FormatItem(key)}), but it did.",
            _expression);
    }

    /// <summary>Verifies that the dictionary contains at least one entry with <paramref name="value"/>.</summary>
    /// <param name="value">The value expected to be present.</param>
    /// <param name="valueExpression">The expression for the value (automatically captured).</param>
    public void ContainValue(TValue value, [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
    {
        EnsureActualNotNull();
        foreach (var item in _actual.Values)
        {
            if (EqualityComparer<TValue>.Default.Equals(item, value))
                return;
        }

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to contain value {valueExpression ?? "value"} ({FormatItem(value)}), but it did not.",
            _expression);
    }

    /// <summary>Verifies that the dictionary does not contain <paramref name="value"/>.</summary>
    /// <param name="value">The value expected to be absent.</param>
    /// <param name="valueExpression">The expression for the value (automatically captured).</param>
    public void NotContainValue(TValue value, [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
    {
        EnsureActualNotNull();
        foreach (var item in _actual.Values)
        {
            if (EqualityComparer<TValue>.Default.Equals(item, value))
            {
                VerificationFlow.Fail(
                    $"Verification failed: expected {_expression} not to contain value {valueExpression ?? "value"} ({FormatItem(value)}), but it did.",
                    _expression);
                return;
            }
        }
    }

    /// <summary>Verifies that <paramref name="key"/> exists and maps to <paramref name="value"/>.</summary>
    /// <param name="key">The key to lookup.</param>
    /// <param name="value">The expected value for the key.</param>
    /// <param name="keyExpression">The expression for the key (automatically captured).</param>
    /// <param name="valueExpression">The expression for the value (automatically captured).</param>
    public void HaveValue(
        TKey key,
        TValue value,
        [CallerArgumentExpression(nameof(key))] string? keyExpression = null,
        [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
    {
        EnsureActualNotNull();
        if (!_actual.TryGetValue(key, out var actualValue))
        {
            VerificationFlow.Fail(
                $"Verification failed: expected {_expression} to have value {valueExpression ?? "value"} ({FormatItem(value)}) for key {keyExpression ?? "key"} ({FormatItem(key)}), but the key was missing.",
                _expression);
            return;
        }

        if (EqualityComparer<TValue>.Default.Equals(actualValue, value))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have value {valueExpression ?? "value"} ({FormatItem(value)}) for key {keyExpression ?? "key"} ({FormatItem(key)}), but found {FormatItem(actualValue)}.",
            _expression);
    }

    /// <summary>Verifies that the dictionary is <c>null</c>.</summary>
    public void ToBeNull()
    {
        if (_actual is null)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be null, but was {OmniAssertionException.FormatValueForMessage(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the dictionary is not <c>null</c>.</summary>
    public void NotToBeNull()
    {
        if (_actual is not null)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} not to be null, but it was.",
            _expression);
    }

    /// <summary>Verifies that the dictionary has exactly <paramref name="expectedCount"/> entries.</summary>
    /// <param name="expectedCount">The expected number of entries.</param>
    /// <param name="countExpression">The expression for the expected count (automatically captured).</param>
    public void ToHaveCount(int expectedCount, [CallerArgumentExpression(nameof(expectedCount))] string? countExpression = null)
    {
        EnsureActualNotNull();
        var actualCount = _actual.Count;

        if (actualCount == expectedCount)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have count {expectedCount} ({countExpression ?? "expected"}), but had {actualCount}.",
            _expression);
    }

    private static string FormatItem<T>(T item) => OmniAssertionException.FormatValueForMessage(item!);

    private void EnsureActualNotNull()
    {
        if (_actual is not null)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} not to be null, but it was.",
            _expression);
    }
}
