using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace OmniAssert;

/// <summary>
/// Walks two object graphs reflectively (public instance properties, indexers excluded) and detects differences.
/// Handles cycles via visited reference pairs; compares sequences element-wise; treats primitives, enums, strings, and decimals as leaves.
/// </summary>
internal static class ObjectDiffWalker
{
    /// <summary>Compares <paramref name="expected"/> to <paramref name="actual"/> starting at <paramref name="rootLabel"/>.</summary>
    /// <param name="expected">Reference object graph (expected side).</param>
    /// <param name="actual">Candidate object graph (actual side).</param>
    /// <param name="rootLabel">Path label for the root (often the caller expression).</param>
    /// <returns><c>null</c> when equivalent; otherwise a result you can format for <see cref="ObjectAssertions.ToBeEquivalentTo"/> failures.</returns>
    public static ObjectDiffResult? Diff(object? expected, object? actual, string rootLabel) =>
        Diff(expected, actual, rootLabel, EquivalenceOptions.Default);

    /// <summary>Compares two graphs using the supplied <paramref name="options"/> to relax case and sequence ordering.</summary>
    public static ObjectDiffResult? Diff(object? expected, object? actual, string rootLabel, EquivalenceOptions options)
    {
        var visitedPairs = new HashSet<(object, object)>(ReferencePairComparer.Instance);
        var mismatches = new List<Mismatch>();
        Walk(expected, actual, rootLabel, visitedPairs, mismatches, options);
        return mismatches.Count == 0 ? null : new ObjectDiffResult(mismatches);
    }

    private static void Walk(object? expected, object? actual, string path, HashSet<(object, object)> visited, List<Mismatch> mismatches, EquivalenceOptions options)
    {
        if (ReferenceEquals(expected, actual))
            return;

        if (expected is null || actual is null)
        {
            mismatches.Add(new Mismatch(path, expected, actual));
            return;
        }

        var expType = expected.GetType();
        var actType = actual.GetType();
        if (expType != actType)
        {
            mismatches.Add(new Mismatch(path, expected, actual));
            return;
        }

        if (IsLeaf(expType))
        {
            if (options.IgnoreCase && expected is string es && actual is string @as)
            {
                if (!string.Equals(es, @as, StringComparison.OrdinalIgnoreCase))
                    mismatches.Add(new Mismatch(path, expected, actual));
                return;
            }

            if (!Equals(expected, actual))
                mismatches.Add(new Mismatch(path, expected, actual));
            return;
        }

        if (expected is IEnumerable expEnum and not string)
        {
            if (actual is not IEnumerable actEnum)
            {
                mismatches.Add(new Mismatch(path, expected, actual));
                return;
            }

            if (options.IgnoreCollectionOrder)
                CompareEnumerablesUnordered(expEnum, actEnum, path, mismatches, options);
            else
                CompareEnumerables(expEnum, actEnum, path, visited, mismatches, options);
            return;
        }

        var pair = (expected, actual);
        if (!visited.Add(pair))
            return;

        foreach (var prop in GetReadableProperties(expType))
        {
            object? ev;
            object? av;
            try
            {
                ev = prop.GetValue(expected);
                av = prop.GetValue(actual);
            }
            catch (TargetInvocationException)
            {
                mismatches.Add(new Mismatch(path + "." + prop.Name, expected, actual));
                continue;
            }

            Walk(ev, av, path + "." + prop.Name, visited, mismatches, options);
        }
    }

    private static void CompareEnumerables(IEnumerable expected, IEnumerable actual, string path, HashSet<(object, object)> visited, List<Mismatch> mismatches, EquivalenceOptions options)
    {
        var ee = expected.GetEnumerator();
        var ae = actual.GetEnumerator();
        var index = 0;
        while (true)
        {
            var eNext = ee.MoveNext();
            var aNext = ae.MoveNext();
            if (!eNext && !aNext)
                return;
            if (eNext != aNext)
            {
                mismatches.Add(new Mismatch(path + $"[{index}]", eNext ? ee.Current : null, aNext ? ae.Current : null));
                return;
            }

            Walk(ee.Current, ae.Current, path + $"[{index}]", visited, mismatches, options);
            index++;
        }
    }

    private static void CompareEnumerablesUnordered(IEnumerable expected, IEnumerable actual, string path, List<Mismatch> mismatches, EquivalenceOptions options)
    {
        var expectedItems = new List<object?>();
        foreach (var e in expected) expectedItems.Add(e);
        var actualItems = new List<object?>();
        foreach (var a in actual) actualItems.Add(a);

        if (expectedItems.Count != actualItems.Count)
        {
            mismatches.Add(new Mismatch(path, expectedItems.Count, actualItems.Count));
            return;
        }

        var matched = new bool[actualItems.Count];
        foreach (var expectedItem in expectedItems)
        {
            var found = false;
            for (var i = 0; i < actualItems.Count; i++)
            {
                if (matched[i])
                    continue;

                if (Diff(expectedItem, actualItems[i], "", options) is null)
                {
                    matched[i] = true;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                mismatches.Add(new Mismatch(path, expectedItem, null));
                return;
            }
        }
    }

    private static bool IsLeaf(Type t)
    {
        if (t.IsPrimitive || t.IsEnum || t == typeof(string) || t == typeof(decimal))
            return true;
        return t.IsValueType && !t.IsEnum;
    }

    private static IEnumerable<PropertyInfo> GetReadableProperties(Type t) =>
        t.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0);

    private sealed class ReferencePairComparer : IEqualityComparer<(object, object)>
    {
        public static readonly ReferencePairComparer Instance = new();

        public bool Equals((object, object) x, (object, object) y) =>
            ReferenceEquals(x.Item1, y.Item1) && ReferenceEquals(x.Item2, y.Item2);

        public int GetHashCode((object, object) obj) =>
            HashCode.Combine(RuntimeHelpers.GetHashCode(obj.Item1), RuntimeHelpers.GetHashCode(obj.Item2));
    }
}

/// <summary>One differing path when two object graphs are compared.</summary>
/// <param name="path">Dot/bracket path from the root (for example <c>Items[0].Name</c>).</param>
/// <param name="expected">Expected value at that path.</param>
/// <param name="actual">Actual value at that path.</param>
internal readonly struct Mismatch(string path, object? expected, object? actual)
{
    /// <summary>Path from the root to the mismatch.</summary>
    public string Path { get; } = path;

    /// <summary>Expected side snapshot.</summary>
    public object? Expected { get; } = expected;

    /// <summary>Actual side snapshot.</summary>
    public object? Actual { get; } = actual;
}

/// <summary>Non-empty set of <see cref="Mismatch"/> entries from <see cref="ObjectDiffWalker.Diff"/>.</summary>
internal sealed class ObjectDiffResult
{
    private readonly IReadOnlyList<Mismatch> _mismatches;

    internal ObjectDiffResult(IReadOnlyList<Mismatch> mismatches) => _mismatches = mismatches;

    /// <summary>Formats a multi-line message with expected/actual colouring when enabled.</summary>
    /// <returns>Human-readable diff text suitable for an assertion failure.</returns>
    public string FormatMessage()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Objects were not equivalent.");
        foreach (var m in _mismatches)
        {
            sb.AppendLine();
            sb.Append(AnsiColour.Expected("Expected "));
            sb.Append(m.Path);
            sb.Append(": ");
            sb.AppendLine(AnsiColour.Expected(OmniAssertionException.FormatValueForMessage(m.Expected)));
            sb.Append(AnsiColour.Actual("Got "));
            sb.Append(m.Path);
            sb.Append(": ");
            sb.Append(AnsiColour.Actual(OmniAssertionException.FormatValueForMessage(m.Actual)));
        }

        return sb.ToString().TrimEnd();
    }
}
