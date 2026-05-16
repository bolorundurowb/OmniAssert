using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>
/// Entry point for fluent assertions. Overloads capture the caller’s expression text via
/// <see cref="CallerArgumentExpressionAttribute"/> so failures name the value or expression under test.
/// </summary>
/// <remarks>
/// <para>Use <see cref="AssertionScope"/> to defer failures until the scope is disposed (soft asserts).</para>
/// <para><see cref="VerifyExpression(bool, string?)"/> can be rewritten at compile time when the optional OmniAssert Roslyn generator and interceptor MSBuild properties are enabled—see the README.</para>
/// </remarks>
public static partial class Assert
{
    /// <summary>Begins verifying a boolean subject; chain <see cref="BoolAssertions.ToBeTrue"/> or <see cref="BoolAssertions.ToBeFalse"/>.</summary>
    /// <param name="actual">The boolean value under test.</param>
    /// <param name="expression">Caller expression text for failures (compiler-supplied via <c>[CallerArgumentExpression]</c>).</param>
    /// <returns>Fluent assertions for the boolean.</returns>
    public static BoolAssertions Verify(bool actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");


    /// <summary>
    /// Verifies that a boolean condition is true. If the condition is false, it generates an assertion failure
    /// with diagnostic information about the evaluated expression.
    /// </summary>
    /// <param name="condition">The boolean value to be verified.</param>
    /// <param name="expression">The caller’s expression text, supplied automatically by the compiler.</param>
    /// <exception cref="OmniAssertionException">Thrown when the condition is false and no enclosing <see cref="AssertionScope"/> is collecting failures.</exception>
    public static void VerifyExpression(bool condition,
        [CallerArgumentExpression(nameof(condition))] string? expression = null) =>
        VerifyExpression(condition, new AssertionCapture(expression ?? "condition", null));

    /// <summary>
    /// Verifies that <paramref name="condition"/> is true using an explicit <see cref="AssertionCapture"/> (source text and optional operand snapshots).
    /// Intended for boolean-expression lowering in tooling; ordinary call sites should use <see cref="VerifyExpression(bool, string?)"/>.
    /// </summary>
    /// <exception cref="OmniAssertionException">Thrown when <paramref name="condition"/> is false and no enclosing <see cref="AssertionScope"/> is collecting failures.</exception>
    public static void VerifyExpression(bool condition, AssertionCapture capture)
    {
        if (condition)
            return;

        VerificationFlow.Fail(OmniAssertionException.ForBooleanFailure(in capture));
    }

    /// <summary>Begins verifying a double subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<double> Verify(double actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a float subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<float> Verify(float actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a long subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<long> Verify(long actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a decimal subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<decimal> Verify(decimal actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying an integer subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<int> Verify(int actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a short subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<short> Verify(short actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a byte subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<byte> Verify(byte actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a uint subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<uint> Verify(uint actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a ulong subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<ulong> Verify(ulong actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a ushort subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<ushort> Verify(ushort actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying an sbyte subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<sbyte> Verify(sbyte actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a BigInteger subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<BigInteger> Verify(BigInteger actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a GUID subject.</summary>
    /// <param name="actual">The GUID value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="GuidAssertions"/> object to continue the verification.</returns>
    public static GuidAssertions Verify(Guid actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a string subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="StringAssertions"/> object to continue the verification.</returns>
    public static StringAssertions Verify(string? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a URI subject.</summary>
    /// <param name="actual">The URI value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="UriAssertions"/> object to continue the verification.</returns>
    public static UriAssertions Verify(Uri? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a collection subject.</summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="actual">The collection to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="CollectionAssertions{T}"/> object to continue the verification.</returns>
    public static CollectionAssertions<T> Verify<T>(IEnumerable<T>? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual!, expression ?? "actual");

    /// <summary>Begins verifying a dictionary subject.</summary>
    /// <typeparam name="TKey">The dictionary key type.</typeparam>
    /// <typeparam name="TValue">The dictionary value type.</typeparam>
    /// <param name="actual">The dictionary to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="DictionaryAssertions{TKey, TValue}"/> object to continue the verification.</returns>
    public static DictionaryAssertions<TKey, TValue> Verify<TKey, TValue>(Dictionary<TKey, TValue>? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Verify((IReadOnlyDictionary<TKey, TValue>?)actual, expression);

    /// <summary>Begins verifying a dictionary subject.</summary>
    /// <typeparam name="TKey">The dictionary key type.</typeparam>
    /// <typeparam name="TValue">The dictionary value type.</typeparam>
    /// <param name="actual">The dictionary to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="DictionaryAssertions{TKey, TValue}"/> object to continue the verification.</returns>
    public static DictionaryAssertions<TKey, TValue> Verify<TKey, TValue>(IDictionary<TKey, TValue>? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual is null ? null : new ReadOnlyDictionaryWrapper<TKey, TValue>(actual), expression ?? "actual");

    /// <summary>Begins verifying a read-only dictionary subject.</summary>
    /// <typeparam name="TKey">The dictionary key type.</typeparam>
    /// <typeparam name="TValue">The dictionary value type.</typeparam>
    /// <param name="actual">The dictionary to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="DictionaryAssertions{TKey, TValue}"/> object to continue the verification.</returns>
    public static DictionaryAssertions<TKey, TValue> Verify<TKey, TValue>(IReadOnlyDictionary<TKey, TValue>? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual!, expression ?? "actual");

    /// <summary>Verifies that a file exists at <paramref name="path"/> and returns file assertions.</summary>
    /// <param name="path">The file path to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="FileAssertions"/> object to continue file verification.</returns>
    public static FileAssertions FileExists(string path, [CallerArgumentExpression(nameof(path))] string? expression = null)
    {
        if (!File.Exists(path))
        {
            VerificationFlow.Fail(
                $"Verification failed: expected file {expression ?? "path"} ({StringFormatter.Quote(path)}) to exist, but it does not.",
                expression ?? "path");
        }

        return new(path, expression ?? "path");
    }

    /// <summary>Verifies that a directory exists at <paramref name="path"/> and returns directory assertions.</summary>
    /// <param name="path">The directory path to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="DirectoryAssertions"/> object to continue directory verification.</returns>
    public static DirectoryAssertions DirectoryExists(string path, [CallerArgumentExpression(nameof(path))] string? expression = null)
    {
        if (!Directory.Exists(path))
        {
            VerificationFlow.Fail(
                $"Verification failed: expected directory {expression ?? "path"} ({StringFormatter.Quote(path)}) to exist, but it does not.",
                expression ?? "path");
        }

        return new(path, expression ?? "path");
    }

    /// <summary>Begins verifying an enum subject.</summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="EnumAssertions{T}"/> object to continue the verification.</returns>
    public static EnumAssertions<T> Verify<T>(T actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) where T : struct, Enum =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a nullable value type or reference type subject.</summary>
    /// <typeparam name="T">The underlying reference type.</typeparam>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NullableReferenceAssertions{T}"/> object to continue the verification.</returns>
    public static NullableReferenceAssertions<T> VerifyNullable<T>(T? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) where T : class =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a nullable value type subject.</summary>
    /// <typeparam name="T">The underlying value type.</typeparam>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NullableValueAssertions{T}"/> object to continue the verification.</returns>
    public static NullableValueAssertions<T> VerifyNullable<T>(T? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) where T : struct =>
        new(actual, expression ?? "actual");

    /// <summary>Verifies that the given action throws an exception of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The expected exception type.</typeparam>
    /// <param name="action">The action that is expected to throw.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>An <see cref="ExceptionAssertions{T}"/> object to continue verifying the exception.</returns>
    public static ExceptionAssertions<T> Throws<T>(Action action, [CallerArgumentExpression(nameof(action))] string? expression = null) where T : Exception
    {
        try
        {
            action();
        }
        catch (T ex)
        {
            return new ExceptionAssertions<T>(ex, expression ?? "action");
        }
        catch (Exception ex)
        {
            VerificationFlow.Fail($"Verification failed: expected {expression ?? "action"} to throw {typeof(T).Name}, but it threw {ex.GetType().Name}.", expression ?? "action");
        }

        VerificationFlow.Fail($"Verification failed: expected {expression ?? "action"} to throw {typeof(T).Name}, but it did not throw.", expression ?? "action");
        throw new UnreachableException();
    }

    /// <summary>Verifies that the given action does not throw any exception.</summary>
    /// <param name="action">The action that should not throw.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    public static void NotThrow(Action action, [CallerArgumentExpression(nameof(action))] string? expression = null)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            VerificationFlow.Fail($"Verification failed: expected {expression ?? "action"} not to throw, but it threw {ex.GetType().Name}: {ex.Message}", expression ?? "action");
        }
    }

    /// <summary>Verifies that the given asynchronous action throws an exception of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The expected exception type.</typeparam>
    /// <param name="action">The asynchronous action that is expected to throw.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A task representing the asynchronous operation, containing <see cref="ExceptionAssertions{T}"/> to continue verifying the exception.</returns>
    public static async Task<ExceptionAssertions<T>> ThrowsAsync<T>(Func<Task> action, [CallerArgumentExpression(nameof(action))] string? expression = null) where T : Exception
    {
        try
        {
            await action();
        }
        catch (T ex)
        {
            return new ExceptionAssertions<T>(ex, expression ?? "action");
        }
        catch (Exception ex)
        {
            VerificationFlow.Fail($"Verification failed: expected {expression ?? "action"} to throw {typeof(T).Name}, but it threw {ex.GetType().Name}.", expression ?? "action");
        }

        VerificationFlow.Fail($"Verification failed: expected {expression ?? "action"} to throw {typeof(T).Name}, but it did not throw.", expression ?? "action");
        throw new UnreachableException();
    }

    /// <summary>Verifies that the given asynchronous action does not throw any exception.</summary>
    /// <param name="action">The asynchronous action that should not throw.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task NotThrowAsync(Func<Task> action, [CallerArgumentExpression(nameof(action))] string? expression = null)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            VerificationFlow.Fail($"Verification failed: expected {expression ?? "action"} not to throw, but it threw {ex.GetType().Name}: {ex.Message}", expression ?? "action");
        }
    }

    /// <summary>Verifies that the given asynchronous action completes within the specified time span.</summary>
    /// <param name="timeout">The maximum allowed duration.</param>
    /// <param name="action">The asynchronous action to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task CompleteWithin(TimeSpan timeout, Func<Task> action, [CallerArgumentExpression(nameof(action))] string? expression = null)
    {
        var task = action();
        var completedTask = await Task.WhenAny(task, Task.Delay(timeout));
        if (completedTask == task)
        {
            await task; // propagate any exceptions
            return;
        }

        VerificationFlow.Fail($"Verification failed: expected {expression ?? "action"} to complete within {timeout}, but it timed out.", expression ?? "action");
    }

    /// <summary>Begins verifying a DateTime subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="DateTimeAssertions"/> object to continue the verification.</returns>
    public static DateTimeAssertions Verify(DateTime actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a DateTimeOffset subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="DateTimeOffsetAssertions"/> object to continue the verification.</returns>
    public static DateTimeOffsetAssertions Verify(DateTimeOffset actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a TimeSpan subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="TimeSpanAssertions"/> object to continue the verification.</returns>
    public static TimeSpanAssertions Verify(TimeSpan actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a DateOnly subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="DateOnlyAssertions"/> object to continue the verification.</returns>
    public static DateOnlyAssertions Verify(DateOnly actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a TimeOnly subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="TimeOnlyAssertions"/> object to continue the verification.</returns>
    public static TimeOnlyAssertions Verify(TimeOnly actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying an object subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>An <see cref="ObjectAssertions"/> object to continue the verification.</returns>
    public static ObjectAssertions Verify(object? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    private sealed class ReadOnlyDictionaryWrapper<TKey, TValue>(IDictionary<TKey, TValue> inner) : IReadOnlyDictionary<TKey, TValue>
    {
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => inner.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => inner.GetEnumerator();
        public int Count => inner.Count;
        public bool ContainsKey(TKey key) => inner.ContainsKey(key);
        public bool TryGetValue(TKey key, out TValue value) => inner.TryGetValue(key, out value!);
        public TValue this[TKey key] => inner[key];
        public IEnumerable<TKey> Keys => inner.Keys;
        public IEnumerable<TValue> Values => inner.Values;
    }
}

