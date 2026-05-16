using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>Assertions for <see cref="IEnumerable{T}"/> subjects from <see cref="Assert.Verify{T}(IEnumerable{T}, string?)"/>.</summary>
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
    public void ToBe(IEnumerable<T>? expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (ReferenceEquals(_actual, expected))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be {expectedExpression ?? "expected"} (reference equality), but they were different instances.",
            _expression);
    }

    /// <summary>Verifies that the collection contains the specified <paramref name="item"/>.</summary>
    /// <param name="item">The item expected to be in the collection.</param>
    /// <param name="itemExpression">The expression for the item (automatically captured).</param>
    public void ToContain(T item, [CallerArgumentExpression(nameof(item))] string? itemExpression = null)
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

    /// <summary>Verifies that the collection is empty.</summary>
    public void ToBeEmpty()
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
    public void NotToBeEmpty()
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
    public void NotToContain(T item, [CallerArgumentExpression(nameof(item))] string? itemExpression = null)
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

    /// <summary>Verifies that the collection has the expected number of items.</summary>
    /// <param name="expectedCount">The expected count.</param>
    /// <param name="countExpression">The expression for the expected count (automatically captured).</param>
    public void HasCount(int expectedCount, [CallerArgumentExpression(nameof(expectedCount))] string? countExpression = null)
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

    /// <summary>Verifies that the collection has the expected number of items (alias for <see cref="HasCount"/>).</summary>
    /// <param name="expectedCount">The expected count.</param>
    /// <param name="countExpression">The expression for the expected count (automatically captured).</param>
    public void ToHaveCount(int expectedCount, [CallerArgumentExpression(nameof(expectedCount))] string? countExpression = null) =>
        HasCount(expectedCount, countExpression);

    /// <summary>Verifies that the collection count is greater than <paramref name="minimumCount"/>.</summary>
    /// <param name="minimumCount">The exclusive lower bound for the collection count.</param>
    /// <param name="countExpression">The expression for the minimum count (automatically captured).</param>
    public void HasCountGreaterThan(int minimumCount, [CallerArgumentExpression(nameof(minimumCount))] string? countExpression = null)
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
    public void HasCountLessThan(int maximumCount, [CallerArgumentExpression(nameof(maximumCount))] string? countExpression = null)
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
    public void ToBeUnique()
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
    public void HasUniqueCount(int expectedUniqueCount, [CallerArgumentExpression(nameof(expectedUniqueCount))] string? countExpression = null)
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
    public void ToBeInAscendingOrder()
    {
        EnsureActualNotNull();
        VerifyOrdering(ascending: true);
    }

    /// <summary>Verifies that the collection is in descending order (non-increasing) using <see cref="Comparer{T}.Default"/>.</summary>
    public void ToBeInDescendingOrder()
    {
        EnsureActualNotNull();
        VerifyOrdering(ascending: false);
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

    /// <summary>
    /// Multiset equivalence: same cardinality per distinct element (order ignored). Uses default equality for <typeparamref name="T"/>.
    /// </summary>
    /// <param name="expected">Expected multiset of elements.</param>
    /// <param name="expectedExpression">The expression for the expected collection (automatically captured).</param>
    public void ToBeEquivalentTo(IEnumerable<T> expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
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
                if (!EqualityComparer<T>.Default.Equals(actualItem, expectedItem))
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

    private static string FormatItem(T item) => OmniAssertionException.FormatValueForMessage(item!);

    private void EnsureActualNotNull()
    {
        if (_actual is not null)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} not to be null, but it was.",
            _expression);
    }
}
