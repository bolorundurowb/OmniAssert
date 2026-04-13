using System.Runtime.CompilerServices;

namespace OmniAssert;

public readonly struct CollectionAssertions<T>
{
    private readonly IEnumerable<T> _actual;
    private readonly string _expression;

    internal CollectionAssertions(IEnumerable<T> actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

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

    private static string FormatItem(T item) => OmniAssertionException.FormatValueForMessage(item!);
}
