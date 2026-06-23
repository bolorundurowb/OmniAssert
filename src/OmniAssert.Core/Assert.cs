using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>Legacy entry point. Use <see cref="Ensure"/> instead.</summary>
[Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
public static partial class Assert
{
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static BoolAssertions Verify(this bool actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static void VerifyExpression(bool condition,
        [CallerArgumentExpression(nameof(condition))] string? expression = null) =>
        Ensure.VerifyExpression(condition, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static void VerifyExpression(bool condition, AssertionCapture capture) =>
        Ensure.VerifyExpression(condition, capture);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static NumericAssertions<double> Verify(this double actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static NumericAssertions<float> Verify(this float actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static NumericAssertions<long> Verify(this long actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static NumericAssertions<decimal> Verify(this decimal actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static NumericAssertions<int> Verify(this int actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static NumericAssertions<short> Verify(this short actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static NumericAssertions<byte> Verify(this byte actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static NumericAssertions<uint> Verify(this uint actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static NumericAssertions<ulong> Verify(this ulong actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static NumericAssertions<ushort> Verify(this ushort actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static NumericAssertions<sbyte> Verify(this sbyte actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static NumericAssertions<BigInteger> Verify(this BigInteger actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static GuidAssertions Verify(this Guid actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static StringAssertions Verify(this string? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static UriAssertions Verify(this Uri? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static CollectionAssertions<T> Verify<T>(this IEnumerable<T>? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static DictionaryAssertions<TKey, TValue> Verify<TKey, TValue>(this Dictionary<TKey, TValue>? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static DictionaryAssertions<TKey, TValue> Verify<TKey, TValue>(this IDictionary<TKey, TValue>? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static DictionaryAssertions<TKey, TValue> Verify<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue>? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static EnumAssertions<T> Verify<T>(this T actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) where T : struct, Enum =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static DateTimeAssertions Verify(this DateTime actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static DateTimeOffsetAssertions Verify(this DateTimeOffset actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static TimeSpanAssertions Verify(this TimeSpan actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static DateOnlyAssertions Verify(this DateOnly actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static TimeOnlyAssertions Verify(this TimeOnly actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static SpanAssertions<T> Verify<T>(this ReadOnlySpan<T> actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static SpanAssertions<T> Verify<T>(this Span<T> actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static SpanAssertions<T> Verify<T>(this ReadOnlyMemory<T> actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static SpanAssertions<T> Verify<T>(this Memory<T> actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public static ObjectAssertions Verify(this object? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Ensure.Must(actual, expression);

    [Obsolete("Use Ensure.Succeed() instead.", false)]
    public static void Succeed() => Ensure.Succeed();

    [Obsolete("Use Ensure.Fail() instead.", false)]
    public static void Fail(string? message = null) => Ensure.Fail(message);
}
