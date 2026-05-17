using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;

namespace OmniAssert;

/// <summary>
/// Assertions for <see cref="ReadOnlySpan{T}"/>, <see cref="Span{T}"/>,
/// <see cref="ReadOnlyMemory{T}"/>, and <see cref="Memory{T}"/> subjects.
/// </summary>
/// <remarks>
/// This is a <c>ref struct</c> so it can hold a <see cref="ReadOnlySpan{T}"/> directly without allocation.
/// All four memory-slice types share this single assertion type via their respective
/// <c>Verify()</c> extension-method overloads.
/// </remarks>
/// <typeparam name="T">Element type.</typeparam>
public readonly ref struct SpanAssertions<T>
{
    private readonly ReadOnlySpan<T> _actual;
    private readonly string _expression;

    internal SpanAssertions(ReadOnlySpan<T> actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    /// <summary>Verifies that the span is sequence-equal to <paramref name="expected"/> (no allocation).</summary>
    /// <param name="expected">The expected element sequence.</param>
    /// <param name="expectedExpression">The expression for the expected span (automatically captured).</param>
    public void ToEqual(ReadOnlySpan<T> expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (SpanSequenceEqual(_actual, expected))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to equal {expectedExpression ?? "expected"}. " +
            $"Actual length: {_actual.Length}, expected length: {expected.Length}.",
            _expression);
    }

    /// <summary>Verifies that the span is not sequence-equal to <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The element sequence the span must not equal.</param>
    /// <param name="unexpectedExpression">The expression for the unexpected span (automatically captured).</param>
    public void NotToEqual(ReadOnlySpan<T> unexpected, [CallerArgumentExpression(nameof(unexpected))] string? unexpectedExpression = null)
    {
        if (!SpanSequenceEqual(_actual, unexpected))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} not to equal {unexpectedExpression ?? "unexpected"}, but the sequences were identical (length {_actual.Length}).",
            _expression);
    }

    /// <summary>Verifies that the span is empty (length == 0).</summary>
    public void ToBeEmpty()
    {
        if (_actual.IsEmpty)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be empty, but it had length {_actual.Length}.",
            _expression);
    }

    /// <summary>Verifies that the span is not empty (length &gt; 0).</summary>
    public void NotToBeEmpty()
    {
        if (!_actual.IsEmpty)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} not to be empty, but it was.",
            _expression);
    }

    /// <summary>Verifies that the span has exactly <paramref name="expectedLength"/> elements.</summary>
    /// <param name="expectedLength">The expected number of elements.</param>
    /// <param name="lengthExpression">The expression for the expected length (automatically captured).</param>
    public void HasLength(int expectedLength, [CallerArgumentExpression(nameof(expectedLength))] string? lengthExpression = null)
    {
        if (_actual.Length == expectedLength)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have length {expectedLength} ({lengthExpression ?? "expectedLength"}), but had length {_actual.Length}.",
            _expression);
    }

    /// <summary>Verifies that the span has exactly <paramref name="expectedCount"/> elements (alias for <see cref="HasLength"/>).</summary>
    /// <param name="expectedCount">The expected number of elements.</param>
    /// <param name="countExpression">The expression for the expected count (automatically captured).</param>
    public void HasCount(int expectedCount, [CallerArgumentExpression(nameof(expectedCount))] string? countExpression = null) =>
        HasLength(expectedCount, countExpression);

    /// <summary>Verifies that the span has exactly <paramref name="expectedCount"/> elements (alias for <see cref="HasLength"/>).</summary>
    /// <param name="expectedCount">The expected number of elements.</param>
    /// <param name="countExpression">The expression for the expected count (automatically captured).</param>
    public void ToHaveCount(int expectedCount, [CallerArgumentExpression(nameof(expectedCount))] string? countExpression = null) =>
        HasLength(expectedCount, countExpression);

    /// <summary>Verifies that the span count is greater than <paramref name="minimumCount"/>.</summary>
    /// <param name="minimumCount">The exclusive lower bound for the span count.</param>
    /// <param name="countExpression">The expression for the minimum count (automatically captured).</param>
    public void HasCountGreaterThan(int minimumCount, [CallerArgumentExpression(nameof(minimumCount))] string? countExpression = null)
    {
        if (_actual.Length > minimumCount)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have count greater than {minimumCount} ({countExpression ?? "minimumCount"}), but had {_actual.Length}.",
            _expression);
    }

    /// <summary>Verifies that the span count is less than <paramref name="maximumCount"/>.</summary>
    /// <param name="maximumCount">The exclusive upper bound for the span count.</param>
    /// <param name="countExpression">The expression for the maximum count (automatically captured).</param>
    public void HasCountLessThan(int maximumCount, [CallerArgumentExpression(nameof(maximumCount))] string? countExpression = null)
    {
        if (_actual.Length < maximumCount)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have count less than {maximumCount} ({countExpression ?? "maximumCount"}), but had {_actual.Length}.",
            _expression);
    }

    /// <summary>Verifies that the span contains <paramref name="item"/>.</summary>
    /// <param name="item">The element expected to be present.</param>
    /// <param name="itemExpression">The expression for the item (automatically captured).</param>
    public void ToContain(T item, [CallerArgumentExpression(nameof(item))] string? itemExpression = null)
    {
        var comparer = EqualityComparer<T>.Default;
        foreach (var element in _actual)
        {
            if (comparer.Equals(element, item))
                return;
        }

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to contain {itemExpression ?? "item"} ({OmniAssertionException.FormatValueForMessage(item!)}), but it did not.",
            _expression);
    }

    /// <summary>Verifies that the span contains at least one element satisfying <paramref name="predicate"/>.</summary>
    /// <param name="predicate">The condition that at least one item must meet.</param>
    /// <param name="predicateExpression">The expression for the predicate (automatically captured).</param>
    public void ToContain(Func<T, bool> predicate, [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null)
    {
        foreach (var item in _actual)
        {
            if (predicate(item))
                return;
        }

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to contain an element matching {predicateExpression ?? "predicate"}, but no such element was found.",
            _expression);
    }

    /// <summary>Verifies that the span does not contain <paramref name="item"/>.</summary>
    /// <param name="item">The element that must not be present.</param>
    /// <param name="itemExpression">The expression for the item (automatically captured).</param>
    public void NotToContain(T item, [CallerArgumentExpression(nameof(item))] string? itemExpression = null)
    {
        var comparer = EqualityComparer<T>.Default;
        var index = 0;
        foreach (var element in _actual)
        {
            if (comparer.Equals(element, item))
            {
                VerificationFlow.Fail(
                    $"Verification failed: expected {_expression} not to contain {itemExpression ?? "item"} ({OmniAssertionException.FormatValueForMessage(item!)}), but found it at index {index}.",
                    _expression);
                return;
            }
            index++;
        }
    }

    /// <summary>Verifies that the span contains no elements satisfying <paramref name="predicate"/>.</summary>
    /// <param name="predicate">The condition that no item should meet.</param>
    /// <param name="predicateExpression">The expression for the predicate (automatically captured).</param>
    public void NotToContain(Func<T, bool> predicate, [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null)
    {
        var index = 0;
        foreach (var item in _actual)
        {
            if (predicate(item))
            {
                VerificationFlow.Fail(
                    $"Verification failed: expected {_expression} to contain no element matching {predicateExpression ?? "predicate"}, but element at index {index} ({OmniAssertionException.FormatValueForMessage(item!)}) matched.",
                    _expression);
                return;
            }
            index++;
        }
    }

    /// <summary>Verifies that the span starts with the element sequence in <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected prefix sequence.</param>
    /// <param name="expectedExpression">The expression for the expected span (automatically captured).</param>
    public void ToStartWith(ReadOnlySpan<T> expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (expected.Length > _actual.Length)
        {
            VerificationFlow.Fail(
                $"Verification failed: expected {_expression} to start with {expectedExpression ?? "expected"} (length {expected.Length}), but actual length {_actual.Length} is shorter.",
                _expression);
            return;
        }

        if (SpanSequenceEqual(_actual.Slice(0, expected.Length), expected))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to start with {expectedExpression ?? "expected"}, but it did not.",
            _expression);
    }

    /// <summary>Verifies that the span ends with the element sequence in <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected suffix sequence.</param>
    /// <param name="expectedExpression">The expression for the expected span (automatically captured).</param>
    public void ToEndWith(ReadOnlySpan<T> expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (expected.Length > _actual.Length)
        {
            VerificationFlow.Fail(
                $"Verification failed: expected {_expression} to end with {expectedExpression ?? "expected"} (length {expected.Length}), but actual length {_actual.Length} is shorter.",
                _expression);
            return;
        }

        if (SpanSequenceEqual(_actual.Slice(_actual.Length - expected.Length), expected))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to end with {expectedExpression ?? "expected"}, but it did not.",
            _expression);
    }

    /// <summary>Verifies that all elements in the span are unique.</summary>
    public void ToBeUnique()
    {
        var seen = new HashSet<T>();
        var index = 0;
        foreach (var item in _actual)
        {
            if (!seen.Add(item))
            {
                VerificationFlow.Fail(
                    $"Verification failed: expected {_expression} to contain unique elements, but found duplicate {OmniAssertionException.FormatValueForMessage(item!)} at index {index}.",
                    _expression);
                return;
            }
            index++;
        }
    }

    /// <summary>Verifies that the span contains exactly <paramref name="expectedUniqueCount"/> distinct elements.</summary>
    /// <param name="expectedUniqueCount">The expected number of distinct elements.</param>
    /// <param name="countExpression">The expression for the expected unique count (automatically captured).</param>
    public void HasUniqueCount(int expectedUniqueCount, [CallerArgumentExpression(nameof(expectedUniqueCount))] string? countExpression = null)
    {
        var set = new HashSet<T>();
        foreach (var item in _actual) set.Add(item);
        var uniqueCount = set.Count;
        if (uniqueCount == expectedUniqueCount)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have unique count {expectedUniqueCount} ({countExpression ?? "expectedUniqueCount"}), but had {uniqueCount}.",
            _expression);
    }

    /// <summary>Verifies that the span is in ascending order (non-decreasing) using <see cref="Comparer{T}.Default"/>.</summary>
    public void ToBeInAscendingOrder() => VerifyOrdering(ascending: true);

    /// <summary>Verifies that the span is in descending order (non-increasing) using <see cref="Comparer{T}.Default"/>.</summary>
    public void ToBeInDescendingOrder() => VerifyOrdering(ascending: false);

    /// <summary>Verifies that the span is in ascending order (non-decreasing) by the key returned by <paramref name="keySelector"/>.</summary>
    /// <typeparam name="TKey">The type of the key used for ordering.</typeparam>
    /// <param name="keySelector">A function that extracts the comparison key from each element.</param>
    /// <param name="keySelectorExpression">The expression for the key selector (automatically captured).</param>
    public void ToBeInAscendingOrder<TKey>(Func<T, TKey> keySelector,
        [CallerArgumentExpression(nameof(keySelector))] string? keySelectorExpression = null) =>
        VerifyOrderingByKey(keySelector, ascending: true, keySelectorExpression ?? "keySelector");

    /// <summary>Verifies that the span is in descending order (non-increasing) by the key returned by <paramref name="keySelector"/>.</summary>
    /// <typeparam name="TKey">The type of the key used for ordering.</typeparam>
    /// <param name="keySelector">A function that extracts the comparison key from each element.</param>
    /// <param name="keySelectorExpression">The expression for the key selector (automatically captured).</param>
    public void ToBeInDescendingOrder<TKey>(Func<T, TKey> keySelector,
        [CallerArgumentExpression(nameof(keySelector))] string? keySelectorExpression = null) =>
        VerifyOrderingByKey(keySelector, ascending: false, keySelectorExpression ?? "keySelector");

    /// <summary>Verifies that all elements in the span satisfy the given <paramref name="predicate"/>.</summary>
    /// <param name="predicate">The condition that every item must meet.</param>
    /// <param name="predicateExpression">The expression for the predicate (automatically captured).</param>
    public void AllSatisfy(Func<T, bool> predicate, [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null)
    {
        var index = 0;
        foreach (var item in _actual)
        {
            if (!predicate(item))
            {
                VerificationFlow.Fail(
                    $"Verification failed: expected all elements in {_expression} to satisfy {predicateExpression ?? "predicate"}, but element at index {index} ({OmniAssertionException.FormatValueForMessage(item!)}) did not.",
                    _expression);
                return;
            }
            index++;
        }
    }

    /// <summary>Verifies that at least one element in the span satisfies the given <paramref name="predicate"/>.</summary>
    /// <param name="predicate">The condition that at least one item must meet.</param>
    /// <param name="predicateExpression">The expression for the predicate (automatically captured).</param>
    public void AnySatisfy(Func<T, bool> predicate, [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null)
    {
        foreach (var item in _actual)
        {
            if (predicate(item))
                return;
        }

        VerificationFlow.Fail(
            $"Verification failed: expected at least one element in {_expression} to satisfy {predicateExpression ?? "predicate"}, but none did.",
            _expression);
    }

    /// <summary>Verifies that no element in the span satisfies the given <paramref name="predicate"/>.</summary>
    /// <param name="predicate">The condition that no item should meet.</param>
    /// <param name="predicateExpression">The expression for the predicate (automatically captured).</param>
    public void NoneSatisfy(Func<T, bool> predicate, [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null)
    {
        var index = 0;
        foreach (var item in _actual)
        {
            if (predicate(item))
            {
                VerificationFlow.Fail(
                    $"Verification failed: expected no element in {_expression} to satisfy {predicateExpression ?? "predicate"}, but element at index {index} ({OmniAssertionException.FormatValueForMessage(item!)}) did.",
                    _expression);
                return;
            }
            index++;
        }
    }

    /// <summary>Verifies that exactly <paramref name="expectedCount"/> elements satisfy the given <paramref name="predicate"/>.</summary>
    /// <param name="expectedCount">The expected number of matching elements.</param>
    /// <param name="predicate">The condition to test each item against.</param>
    /// <param name="countExpression">The expression for the expected count (automatically captured).</param>
    /// <param name="predicateExpression">The expression for the predicate (automatically captured).</param>
    public void HasCountMatching(int expectedCount, Func<T, bool> predicate,
        [CallerArgumentExpression(nameof(expectedCount))] string? countExpression = null,
        [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null)
    {
        var actualCount = 0;
        foreach (var item in _actual)
        {
            if (predicate(item))
                actualCount++;
        }

        if (actualCount == expectedCount)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have {expectedCount} ({countExpression ?? "expectedCount"}) elements matching {predicateExpression ?? "predicate"}, but found {actualCount}.",
            _expression);
    }

    /// <summary>
    /// Multiset equivalence: same cardinality per distinct element (order ignored). Uses default equality for <typeparamref name="T"/>.
    /// </summary>
    /// <param name="expected">Expected multiset of elements.</param>
    /// <param name="expectedExpression">The expression for the expected collection (automatically captured).</param>
    public void ToBeEquivalentTo(ReadOnlySpan<T> expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual.Length != expected.Length)
        {
            VerificationFlow.Fail(
                $"Verification failed: expected {_expression} to be equivalent to {expectedExpression ?? "expected"} (length {expected.Length}), but had length {_actual.Length}.",
                _expression);
            return;
        }

        var actualCounts = GetElementCounts(_actual);
        var expectedCounts = GetElementCounts(expected);

        foreach (var (expectedItem, expectedCount) in expectedCounts)
        {
            var found = false;
            foreach (var (actualItem, actualCount) in actualCounts)
            {
                if (!EqualityComparer<T>.Default.Equals(actualItem, expectedItem))
                    continue;

                found = true;
                if (actualCount == expectedCount)
                    break;

                VerificationFlow.Fail(
                    $"Verification failed: expected {_expression} to be equivalent to {expectedExpression ?? "expected"}, but element {OmniAssertionException.FormatValueForMessage(expectedItem!)} had {actualCount} occurrences instead of {expectedCount}.",
                    _expression);
                return;
            }

            if (found)
                continue;

            VerificationFlow.Fail(
                $"Verification failed: expected {_expression} to be equivalent to {expectedExpression ?? "expected"}, but element {OmniAssertionException.FormatValueForMessage(expectedItem!)} had 0 occurrences instead of {expectedCount}.",
                _expression);
            return;
        }
    }

    /// <summary>
    /// Multiset equivalence: same cardinality per distinct element (order ignored). Uses default equality for <typeparamref name="T"/>.
    /// </summary>
    /// <param name="expected">Expected multiset of elements.</param>
    /// <param name="expectedExpression">The expression for the expected collection (automatically captured).</param>
    public void ToBeEquivalentTo(IEnumerable<T> expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        var expectedList = expected.ToList();
        if (_actual.Length != expectedList.Count)
        {
            VerificationFlow.Fail(
                $"Verification failed: expected {_expression} to be equivalent to {expectedExpression ?? "expected"} (count {expectedList.Count}), but had count {_actual.Length}.",
                _expression);
            return;
        }

        var actualCounts = GetElementCounts(_actual);
        var expectedCounts = GetElementCounts(expectedList);

        foreach (var (expectedItem, expectedCount) in expectedCounts)
        {
            var found = false;
            foreach (var (actualItem, actualCount) in actualCounts)
            {
                if (!EqualityComparer<T>.Default.Equals(actualItem, expectedItem))
                    continue;

                found = true;
                if (actualCount == expectedCount)
                    break;

                VerificationFlow.Fail(
                    $"Verification failed: expected {_expression} to be equivalent to {expectedExpression ?? "expected"}, but element {OmniAssertionException.FormatValueForMessage(expectedItem!)} had {actualCount} occurrences instead of {expectedCount}.",
                    _expression);
                return;
            }

            if (found)
                continue;

            VerificationFlow.Fail(
                $"Verification failed: expected {_expression} to be equivalent to {expectedExpression ?? "expected"}, but element {OmniAssertionException.FormatValueForMessage(expectedItem!)} had 0 occurrences instead of {expectedCount}.",
                _expression);
            return;
        }
    }

    private void VerifyOrdering(bool ascending)
    {
        if (_actual.Length <= 1)
            return;

        var comparer = Comparer<T>.Default;
        var previous = _actual[0];
        for (var i = 1; i < _actual.Length; i++)
        {
            var current = _actual[i];
            int comparison;
            try
            {
                comparison = comparer.Compare(previous, current);
            }
            catch (Exception ex)
            {
                VerificationFlow.Fail(
                    $"Verification failed: expected {_expression} to be in {(ascending ? "ascending" : "descending")} order, but values of type {typeof(T).Name} could not be compared: {ex.Message}",
                    _expression);
                return;
            }

            var inOrder = ascending ? comparison <= 0 : comparison >= 0;
            if (!inOrder)
            {
                VerificationFlow.Fail(
                    $"Verification failed: expected {_expression} to be in {(ascending ? "ascending" : "descending")} order, but element at index {i - 1} ({OmniAssertionException.FormatValueForMessage(previous!)}) and index {i} ({OmniAssertionException.FormatValueForMessage(current!)}) were out of order.",
                    _expression);
                return;
            }

            previous = current;
        }
    }

    private void VerifyOrderingByKey<TKey>(Func<T, TKey> keySelector, bool ascending, string keySelectorLabel)
    {
        if (_actual.Length <= 1)
            return;

        var comparer = Comparer<TKey>.Default;
        var previous = _actual[0];
        var previousKey = keySelector(previous);
        for (var i = 1; i < _actual.Length; i++)
        {
            var current = _actual[i];
            var currentKey = keySelector(current);
            int comparison;
            try
            {
                comparison = comparer.Compare(previousKey, currentKey);
            }
            catch (Exception ex)
            {
                VerificationFlow.Fail(
                    $"Verification failed: expected {_expression} to be in {(ascending ? "ascending" : "descending")} order by {keySelectorLabel}, but keys of type {typeof(TKey).Name} could not be compared: {ex.Message}",
                    _expression);
                return;
            }

            var inOrder = ascending ? comparison <= 0 : comparison >= 0;
            if (!inOrder)
            {
                VerificationFlow.Fail(
                    $"Verification failed: expected {_expression} to be in {(ascending ? "ascending" : "descending")} order by {keySelectorLabel}, but element at index {i - 1} ({OmniAssertionException.FormatValueForMessage(previous!)}) and index {i} ({OmniAssertionException.FormatValueForMessage(current!)}) were out of order.",
                    _expression);
                return;
            }

            previous = current;
            previousKey = currentKey;
        }
    }

    private static List<(T Item, int Count)> GetElementCounts(ReadOnlySpan<T> source)
    {
        var counts = new List<(T Item, int Count)>();
        foreach (var item in source)
        {
            var index = -1;
            for (var i = 0; i < counts.Count; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(counts[i].Item, item))
                    continue;

                index = i;
                break;
            }

            if (index < 0)
            {
                counts.Add((item, 1));
                continue;
            }

            counts[index] = (counts[index].Item, counts[index].Count + 1);
        }

        return counts;
    }

    private static List<(T Item, int Count)> GetElementCounts(IEnumerable<T> source)
    {
        var counts = new List<(T Item, int Count)>();
        foreach (var item in source)
        {
            var index = -1;
            for (var i = 0; i < counts.Count; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(counts[i].Item, item))
                    continue;

                index = i;
                break;
            }

            if (index < 0)
            {
                counts.Add((item, 1));
                continue;
            }

            counts[index] = (counts[index].Item, counts[index].Count + 1);
        }

        return counts;
    }

    private static bool SpanSequenceEqual(ReadOnlySpan<T> a, ReadOnlySpan<T> b)
    {
        if (a.Length != b.Length)
            return false;
        var comparer = EqualityComparer<T>.Default;
        for (var i = 0; i < a.Length; i++)
        {
            if (!comparer.Equals(a[i], b[i]))
                return false;
        }
        return true;
    }
}
