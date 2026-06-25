using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>Assertions for <see cref="IEnumerable{T}"/> subjects from <see cref="Ensure.Must{T}(IEnumerable{T}, string?)"/>.</summary>
/// <typeparam name="T">Element type.</typeparam>
public readonly struct CollectionAssertions<T>
{
    private readonly IEnumerable<T> _actual;
    private readonly string _expression;

    internal CollectionAssertions(IEnumerable<T>? actual, string expression)
    {
        _actual = actual!;
        _expression = expression;
    }

    /// <summary>Verifies that the collection is the same instance as <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected collection instance.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void Be(IEnumerable<T>? expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (ReferenceEquals(_actual, expected))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be {expectedExpression ?? "expected"} (reference equality), but they were different instances.",
            _expression);
    }

    /// <summary>
    /// Verifies that the collection does not reference the same instance as the specified <paramref name="expected"/> collection.
    /// </summary>
    /// <param name="expected">The expected collection instance.</param>
    /// <param name="expectedExpression">The expression for the expected value (automatically captured).</param>
    public void NotBe(IEnumerable<T>? expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (!ReferenceEquals(_actual, expected))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} not to be {expectedExpression ?? "expected"} (reference equality), but they were the same instance.",
            _expression);
    }

    /// <summary>Verifies that the collection contains the specified <paramref name="item"/>.</summary>
    /// <param name="item">The item expected to be in the collection.</param>
    /// <param name="itemExpression">The expression for the item (automatically captured).</param>
    public void Contain(T item, [CallerArgumentExpression(nameof(item))] string? itemExpression = null)
    {
        EnsureActualNotNull();
        if (_actual is ICollection<T> list && list.Contains(item))
            return;

        foreach (var x in _actual)
        {
            if (EqualityComparer<T>.Default.Equals(x, item))
                return;
        }

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to contain {itemExpression ?? "item"} ({FormatItem(item)}).",
            _expression);
    }

    /// <summary>Verifies that the collection contains at least one element satisfying <paramref name="predicate"/>.</summary>
    /// <param name="predicate">The condition that at least one item must meet.</param>
    /// <param name="predicateExpression">The expression for the predicate (automatically captured).</param>
    public void Contain(Func<T, bool> predicate, [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null)
    {
        EnsureActualNotNull();
        foreach (var item in _actual)
        {
            if (predicate(item))
                return;
        }

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to contain an element matching {predicateExpression ?? "predicate"}, but no such element was found.",
            _expression);
    }

    /// <summary>Verifies that the collection is empty.</summary>
    public void BeEmpty()
    {
        EnsureActualNotNull();
        if (_actual is ICollection<T> c && c.Count == 0)
            return;

        using var e = _actual.GetEnumerator();
        if (!e.MoveNext())
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be empty, but it had at least one element.",
            _expression);
    }

    /// <summary>Verifies that the collection is not empty.</summary>
    public void NotBeEmpty()
    {
        EnsureActualNotNull();
        if (_actual is ICollection<T> c && c.Count > 0)
            return;

        using var e = _actual.GetEnumerator();
        if (e.MoveNext())
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} not to be empty, but it had no elements.",
            _expression);
    }

    /// <summary>Verifies that the collection does not contain the specified <paramref name="item"/>.</summary>
    /// <param name="item">The item that should not be in the collection.</param>
    /// <param name="itemExpression">The expression for the item (automatically captured).</param>
    public void NotContain(T item, [CallerArgumentExpression(nameof(item))] string? itemExpression = null)
    {
        EnsureActualNotNull();
        var found = false;
        if (_actual is ICollection<T> list)
        {
            found = list.Contains(item);
        }
        else
        {
            foreach (var x in _actual)
            {
                if (EqualityComparer<T>.Default.Equals(x, item))
                {
                    found = true;
                    break;
                }
            }
        }

        if (!found)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} not to contain {itemExpression ?? "item"} ({FormatItem(item)}), but it did.",
            _expression);
    }

    /// <summary>Verifies that the collection contains no elements satisfying <paramref name="predicate"/>.</summary>
    /// <param name="predicate">The condition that no item should meet.</param>
    /// <param name="predicateExpression">The expression for the predicate (automatically captured).</param>
    public void NotContain(Func<T, bool> predicate, [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null)
    {
        EnsureActualNotNull();
        var index = 0;
        foreach (var item in _actual)
        {
            if (predicate(item))
            {
                VerificationFlow.Fail(
                    $"Verification failed: expected {_expression} to contain no element matching {predicateExpression ?? "predicate"}, but element at index {index} ({FormatItem(item)}) matched.",
                    _expression);
                return;
            }
            index++;
        }
    }

    /// <summary>Verifies that the collection has the expected number of items.</summary>
    /// <param name="expectedCount">The expected count.</param>
    /// <param name="countExpression">The expression for the expected count (automatically captured).</param>
    public void HaveCount(int expectedCount, [CallerArgumentExpression(nameof(expectedCount))] string? countExpression = null)
    {
        EnsureActualNotNull();
        var actualCount = 0;
        if (_actual is ICollection<T> c)
        {
            actualCount = c.Count;
        }
        else
        {
            foreach (var _ in _actual) actualCount++;
        }

        if (actualCount == expectedCount)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have count {expectedCount} ({countExpression ?? "expected"}), but had {actualCount}.",
            _expression);
    }

    /// <summary>Verifies that the collection count is greater than <paramref name="minimumCount"/>.</summary>
    /// <param name="minimumCount">The exclusive lower bound for the collection count.</param>
    /// <param name="countExpression">The expression for the minimum count (automatically captured).</param>
    public void HaveCountGreaterThan(int minimumCount, [CallerArgumentExpression(nameof(minimumCount))] string? countExpression = null)
    {
        EnsureActualNotNull();
        var actualCount = GetActualCount();
        if (actualCount > minimumCount)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have count greater than {minimumCount} ({countExpression ?? "minimumCount"}), but had {actualCount}.",
            _expression);
    }

    /// <summary>Verifies that the collection count is less than <paramref name="maximumCount"/>.</summary>
    /// <param name="maximumCount">The exclusive upper bound for the collection count.</param>
    /// <param name="countExpression">The expression for the maximum count (automatically captured).</param>
    public void HaveCountLessThan(int maximumCount, [CallerArgumentExpression(nameof(maximumCount))] string? countExpression = null)
    {
        EnsureActualNotNull();
        var actualCount = GetActualCount();
        if (actualCount < maximumCount)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have count less than {maximumCount} ({countExpression ?? "maximumCount"}), but had {actualCount}.",
            _expression);
    }

    /// <summary>Verifies that all elements in the collection are unique.</summary>
    public void BeUnique()
    {
        EnsureActualNotNull();
        var seen = new HashSet<T>();
        var index = 0;
        foreach (var item in _actual)
        {
            if (!seen.Add(item))
            {
                VerificationFlow.Fail(
                    $"Verification failed: expected {_expression} to contain unique elements, but found duplicate {FormatItem(item)} at index {index}.",
                    _expression);
                return;
            }
            index++;
        }
    }

    /// <summary>Verifies that the collection contains exactly <paramref name="expectedUniqueCount"/> distinct elements.</summary>
    /// <param name="expectedUniqueCount">The expected number of distinct elements.</param>
    /// <param name="countExpression">The expression for the expected unique count (automatically captured).</param>
    public void HaveUniqueCount(int expectedUniqueCount, [CallerArgumentExpression(nameof(expectedUniqueCount))] string? countExpression = null)
    {
        EnsureActualNotNull();
        var uniqueCount = new HashSet<T>(_actual).Count;
        if (uniqueCount == expectedUniqueCount)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have unique count {expectedUniqueCount} ({countExpression ?? "expectedUniqueCount"}), but had {uniqueCount}.",
            _expression);
    }

    /// <summary>Verifies that the collection is in ascending order (non-decreasing) using <see cref="Comparer{T}.Default"/>.</summary>
    public void BeInAscendingOrder()
    {
        EnsureActualNotNull();
        VerifyOrdering(ascending: true);
    }

    /// <summary>Verifies that the collection is in ascending order (non-decreasing) by the key returned by <paramref name="keySelector"/>.</summary>
    /// <typeparam name="TKey">The type of the key used for ordering.</typeparam>
    /// <param name="keySelector">A function that extracts the comparison key from each element.</param>
    /// <param name="keySelectorExpression">The expression for the key selector (automatically captured).</param>
    public void BeInAscendingOrder<TKey>(Func<T, TKey> keySelector,
        [CallerArgumentExpression(nameof(keySelector))] string? keySelectorExpression = null)
    {
        EnsureActualNotNull();
        VerifyOrderingByKey(keySelector, ascending: true, keySelectorExpression ?? "keySelector");
    }

    /// <summary>Verifies that the collection is in descending order (non-increasing) using <see cref="Comparer{T}.Default"/>.</summary>
    public void BeInDescendingOrder()
    {
        EnsureActualNotNull();
        VerifyOrdering(ascending: false);
    }

    /// <summary>Verifies that the collection is in descending order (non-increasing) by the key returned by <paramref name="keySelector"/>.</summary>
    /// <typeparam name="TKey">The type of the key used for ordering.</typeparam>
    /// <param name="keySelector">A function that extracts the comparison key from each element.</param>
    /// <param name="keySelectorExpression">The expression for the key selector (automatically captured).</param>
    public void BeInDescendingOrder<TKey>(Func<T, TKey> keySelector,
        [CallerArgumentExpression(nameof(keySelector))] string? keySelectorExpression = null)
    {
        EnsureActualNotNull();
        VerifyOrderingByKey(keySelector, ascending: false, keySelectorExpression ?? "keySelector");
    }

    /// <summary>Verifies that all elements in the collection satisfy the given <paramref name="predicate"/>.</summary>
    /// <param name="predicate">The condition that every item must meet.</param>
    /// <param name="predicateExpression">The expression for the predicate (automatically captured).</param>
    public void AllSatisfy(Func<T, bool> predicate, [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null)
    {
        EnsureActualNotNull();
        var index = 0;
        foreach (var item in _actual)
        {
            if (!predicate(item))
            {
                VerificationFlow.Fail(
                    $"Verification failed: expected all elements in {_expression} to satisfy {predicateExpression ?? "predicate"}, but element at index {index} ({FormatItem(item)}) did not.",
                    _expression);
                return;
            }
            index++;
        }
    }

    /// <summary>Verifies that at least one element in the collection satisfies the given <paramref name="predicate"/>.</summary>
    /// <param name="predicate">The condition that at least one item must meet.</param>
    /// <param name="predicateExpression">The expression for the predicate (automatically captured).</param>
    public void AnySatisfy(Func<T, bool> predicate, [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null)
    {
        EnsureActualNotNull();
        foreach (var item in _actual)
        {
            if (predicate(item))
                return;
        }

        VerificationFlow.Fail(
            $"Verification failed: expected at least one element in {_expression} to satisfy {predicateExpression ?? "predicate"}, but none did.",
            _expression);
    }

    /// <summary>Verifies that no element in the collection satisfies the given <paramref name="predicate"/>.</summary>
    /// <param name="predicate">The condition that no item should meet.</param>
    /// <param name="predicateExpression">The expression for the predicate (automatically captured).</param>
    public void NoneSatisfy(Func<T, bool> predicate, [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null)
    {
        EnsureActualNotNull();
        var index = 0;
        foreach (var item in _actual)
        {
            if (predicate(item))
            {
                VerificationFlow.Fail(
                    $"Verification failed: expected no element in {_expression} to satisfy {predicateExpression ?? "predicate"}, but element at index {index} ({FormatItem(item)}) did.",
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
    public void HaveCountMatching(int expectedCount, Func<T, bool> predicate,
        [CallerArgumentExpression(nameof(expectedCount))] string? countExpression = null,
        [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null)
    {
        EnsureActualNotNull();
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
    public void BeEquivalentTo(IEnumerable<T> expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        EnsureActualNotNull();
        var actualList = _actual.ToList();
        var expectedList = expected.ToList();

        if (actualList.Count != expectedList.Count)
        {
            VerificationFlow.Fail(
                $"Verification failed: expected {_expression} to be equivalent to {expectedExpression ?? "expected"} (count {expectedList.Count}), but had count {actualList.Count}.",
                _expression);
            return;
        }

        var actualCounts = GetElementCounts(actualList);
        var expectedCounts = GetElementCounts(expectedList);

        foreach (var (expectedItem, expectedCount) in expectedCounts)
        {
            var found = false;
            foreach (var (actualItem, actualCount) in actualCounts)
            {
                if (!AreEquivalent(actualItem, expectedItem))
                    continue;

                found = true;
                if (actualCount == expectedCount)
                    break;

                VerificationFlow.Fail(
                    $"Verification failed: expected {_expression} to be equivalent to {expectedExpression ?? "expected"}, but element {FormatItem(expectedItem)} had {actualCount} occurrences instead of {expectedCount}.",
                    _expression);
                return;
            }

            if (found)
                continue;

            VerificationFlow.Fail(
                $"Verification failed: expected {_expression} to be equivalent to {expectedExpression ?? "expected"}, but element {FormatItem(expectedItem)} had 0 occurrences instead of {expectedCount}.",
                _expression);
            return;
        }
    }

    /// <summary>
    /// Sequence equivalence: same elements in the same order. Uses default equality for <typeparamref name="T"/>.
    /// </summary>
    /// <param name="expected">Expected sequence of elements.</param>
    /// <param name="expectedExpression">The expression for the expected collection (automatically captured).</param>
    public void BeSequenceEqual(IEnumerable<T> expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        EnsureActualNotNull();
        var actualList = _actual.ToList();
        var expectedList = expected.ToList();

        if (actualList.Count != expectedList.Count)
        {
            VerificationFlow.Fail(
                $"Verification failed: expected {_expression} to be sequence-equal to {expectedExpression ?? "expected"} (count {expectedList.Count}), but had count {actualList.Count}.",
                _expression);
            return;
        }

        for (int i = 0; i < actualList.Count; i++)
        {
            if (!AreEquivalent(actualList[i], expectedList[i]))
            {
                VerificationFlow.Fail(
                    $"Verification failed: expected {_expression} to be sequence-equal to {expectedExpression ?? "expected"}, but element at index {i} was {FormatItem(actualList[i])} instead of {FormatItem(expectedList[i])}.",
                    _expression);
                return;
            }
        }
    }

    /// <summary>
    /// Verifies that the collection contains the specified elements in the given order,
    /// not necessarily consecutively. Uses default equality for <typeparamref name="T"/>.
    /// </summary>
    /// <param name="expected">The sequence of items expected to appear in order.</param>
    /// <param name="expectedExpression">The expression for the expected items (automatically captured).</param>
    public void ContainInOrder(IEnumerable<T> expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        EnsureActualNotNull();
        var expectedList = expected.ToList();
        if (expectedList.Count == 0)
            return;

        var actualList = _actual.ToList();
        var expectedIndex = 0;

        for (int actualIndex = 0; actualIndex < actualList.Count && expectedIndex < expectedList.Count; actualIndex++)
        {
            if (AreEquivalent(actualList[actualIndex], expectedList[expectedIndex]))
            {
                expectedIndex++;
            }
        }

        if (expectedIndex == expectedList.Count)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to contain {expectedExpression ?? "expected"} in order, but did not find {FormatItem(expectedList[expectedIndex])} after the previously matched items.",
            _expression);
    }

    private static bool AreEquivalent(T a, T b)
    {
        if (EqualityComparer<T>.Default.Equals(a, b))
            return true;

        if (a is null || b is null)
            return false;

        return ObjectDiffWalker.Diff(a, b, "") == null;
    }

    private static List<(T Item, int Count)> GetElementCounts(IEnumerable<T> source)
    {
        var counts = new List<(T Item, int Count)>();
        foreach (var item in source)
        {
            var index = -1;
            for (var i = 0; i < counts.Count; i++)
            {
                if (!AreEquivalent(counts[i].Item, item))
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

    private int GetActualCount()
    {
        if (_actual is ICollection<T> c)
            return c.Count;

        var count = 0;
        foreach (var _ in _actual)
            count++;
        return count;
    }

    private void VerifyOrdering(bool ascending)
    {
        using var enumerator = _actual.GetEnumerator();
        if (!enumerator.MoveNext())
            return;

        var comparer = Comparer<T>.Default;
        var previous = enumerator.Current;
        var index = 1;

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
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
                    $"Verification failed: expected {_expression} to be in {(ascending ? "ascending" : "descending")} order, but element at index {index - 1} ({FormatItem(previous)}) and index {index} ({FormatItem(current)}) were out of order.",
                    _expression);
                return;
            }

            previous = current;
            index++;
        }
    }

    private void VerifyOrderingByKey<TKey>(Func<T, TKey> keySelector, bool ascending, string keySelectorLabel)
    {
        using var enumerator = _actual.GetEnumerator();
        if (!enumerator.MoveNext())
            return;

        var comparer = Comparer<TKey>.Default;
        var previous = enumerator.Current;
        var previousKey = keySelector(previous);
        var index = 1;

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
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
                    $"Verification failed: expected {_expression} to be in {(ascending ? "ascending" : "descending")} order by {keySelectorLabel}, but element at index {index - 1} ({FormatItem(previous)}) and index {index} ({FormatItem(current)}) were out of order.",
                    _expression);
                return;
            }

            previous = current;
            previousKey = currentKey;
            index++;
        }
    }

    private static string FormatItem(T item) => OmniAssertionException.FormatValueForMessage(item!);

    private void EnsureActualNotNull()
    {
        if (_actual is not null)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} not to be null, but it was.",
            _expression);
    }
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBe(IEnumerable<T>? expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null) => Be(expected, expectedExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToContain(T item, [CallerArgumentExpression(nameof(item))] string? itemExpression = null) => Contain(item, itemExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeEmpty() => BeEmpty();
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void NotToBeEmpty() => NotBeEmpty();
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void NotToContain(T item, [CallerArgumentExpression(nameof(item))] string? itemExpression = null) => NotContain(item, itemExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToHaveCount(int expectedCount, [CallerArgumentExpression(nameof(expectedCount))] string? countExpression = null) => HaveCount(expectedCount, countExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToHaveCountGreaterThan(int minimumCount, [CallerArgumentExpression(nameof(minimumCount))] string? countExpression = null) => HaveCountGreaterThan(minimumCount, countExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToHaveCountLessThan(int maximumCount, [CallerArgumentExpression(nameof(maximumCount))] string? countExpression = null) => HaveCountLessThan(maximumCount, countExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeUnique() => BeUnique();
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToHaveUniqueCount(int expectedUniqueCount, [CallerArgumentExpression(nameof(expectedUniqueCount))] string? countExpression = null) => HaveUniqueCount(expectedUniqueCount, countExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeInAscendingOrder() => BeInAscendingOrder();
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeInDescendingOrder() => BeInDescendingOrder();
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToHaveCountMatching(int expectedCount, Func<T, bool> predicate,
        [CallerArgumentExpression(nameof(expectedCount))] string? countExpression = null,
        [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null) => HaveCountMatching(expectedCount, predicate, countExpression, predicateExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeEquivalentTo(IEnumerable<T> expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null) => BeEquivalentTo(expected, expectedExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeSequenceEqual(IEnumerable<T> expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null) => BeSequenceEqual(expected, expectedExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToContainInOrder(IEnumerable<T> expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null) => ContainInOrder(expected, expectedExpression);
}
