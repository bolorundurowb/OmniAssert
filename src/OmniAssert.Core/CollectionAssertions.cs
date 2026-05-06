using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>Assertions for <see cref="IEnumerable{T}"/> subjects from <see cref="Assert.Verify{T}(IEnumerable{T}, string?)"/>.</summary>
/// <typeparam name="T">Element type.</typeparam>
public readonly struct CollectionAssertions<T>
{
    private readonly IEnumerable<T> _actual;
    private readonly string _expression;

    internal CollectionAssertions(IEnumerable<T> actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    /// <summary>Verifies that the collection contains the specified <paramref name="item"/>.</summary>
    /// <param name="item">The item expected to be in the collection.</param>
    /// <param name="itemExpression">The expression for the item (automatically captured).</param>
    public void ToContain(T item, [CallerArgumentExpression(nameof(item))] string? itemExpression = null)
    {
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
    public void HasCountGreaterThan(int minimumCount, [CallerArgumentExpression(nameof(minimumCount))] string? countExpression = null)
    {
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
        var uniqueCount = new HashSet<T>(_actual).Count;
        if (uniqueCount == expectedUniqueCount)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to have unique count {expectedUniqueCount} ({countExpression ?? "expectedUniqueCount"}), but had {uniqueCount}.",
            _expression);
    }

    /// <summary>Verifies that all elements in the collection satisfy the given <paramref name="predicate"/>.</summary>
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
        var actualList = _actual.ToList();
        var expectedList = expected.ToList();

        if (actualList.Count != expectedList.Count)
        {
            VerificationFlow.Fail(
                $"Verification failed: expected {_expression} to be equivalent to {expectedExpression ?? "expected"} (count {expectedList.Count}), but had count {actualList.Count}.",
                _expression);
            return;
        }

        var actualMap = actualList.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        var expectedMap = expectedList.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

        foreach (var kvp in expectedMap)
        {
            if (!actualMap.TryGetValue(kvp.Key, out var actualCount) || actualCount != kvp.Value)
            {
                VerificationFlow.Fail(
                    $"Verification failed: expected {_expression} to be equivalent to {expectedExpression ?? "expected"}, but element {FormatItem(kvp.Key)} had {actualCount} occurrences instead of {kvp.Value}.",
                    _expression);
                return;
            }
        }
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

    private static string FormatItem(T item) => OmniAssertionException.FormatValueForMessage(item!);
}
