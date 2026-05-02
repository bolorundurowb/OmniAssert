using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace OmniAssert;

/// <summary>Reflective property graph comparison with cycle detection.</summary>
public static class ObjectDiffWalker
{
    public static ObjectDiffResult? Diff(object? expected, object? actual, string rootLabel)
    {
        var visitedPairs = new HashSet<(object, object)>(ReferencePairComparer.Instance);
        var mismatches = new List<Mismatch>();
        Walk(expected, actual, rootLabel, visitedPairs, mismatches);
        return mismatches.Count == 0 ? null : new ObjectDiffResult(mismatches);
    }

    private static void Walk(object? expected, object? actual, string path, HashSet<(object, object)> visited, List<Mismatch> mismatches)
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

            CompareEnumerables(expEnum, actEnum, path, visited, mismatches);
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

            Walk(ev, av, path + "." + prop.Name, visited, mismatches);
        }
    }

    private static void CompareEnumerables(IEnumerable expected, IEnumerable actual, string path, HashSet<(object, object)> visited, List<Mismatch> mismatches)
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

            Walk(ee.Current, ae.Current, path + $"[{index}]", visited, mismatches);
            index++;
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

public readonly struct Mismatch(string path, object? expected, object? actual)
{
    public string Path { get; } = path;
    public object? Expected { get; } = expected;
    public object? Actual { get; } = actual;
}

public sealed class ObjectDiffResult
{
    private readonly IReadOnlyList<Mismatch> _mismatches;

    internal ObjectDiffResult(IReadOnlyList<Mismatch> mismatches) => _mismatches = mismatches;

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
