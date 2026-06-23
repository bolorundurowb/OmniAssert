using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>
/// Primary entry point for fluent assertions. Overloads capture the caller’s expression text via
/// <see cref="CallerArgumentExpressionAttribute"/> so failures name the value or expression under test.
/// </summary>
/// <remarks>
/// <para>Use <see cref="AssertionScope"/> to defer failures until the scope is disposed (soft asserts).</para>
/// <para><see cref="VerifyExpression(bool, string?)"/> call sites are enhanced at compile time by the bundled Roslyn generator unless <c>OmniAssertDisableVerifyInterceptors</c> is set to <c>true</c>—see the README.</para>
/// </remarks>
public static partial class Ensure
{
    /// <summary>Begins verifying a boolean subject; chain <see cref="BoolAssertions.BeTrue"/> or <see cref="BoolAssertions.BeFalse"/>.</summary>
    /// <param name="actual">The boolean value under test.</param>
    /// <param name="expression">Caller expression text for failures (compiler-supplied via <c>[CallerArgumentExpression]</c>).</param>
    /// <returns>Fluent assertions for the boolean.</returns>
    public static BoolAssertions Must(this bool actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");


    /// <summary>
    /// Verifies that a boolean condition is true. If the condition is false, it generates an assertion failure
    /// with diagnostic information about the evaluated expression.
    /// </summary>
    /// <param name="condition">The boolean value to be verified.</param>
    /// <param name="expression">The caller’s expression text, supplied automatically by the compiler.</param>
    /// <exception cref="OmniAssertionException">Thrown when the condition is false and no enclosing <see cref="AssertionScope"/> is collecting failures.</exception>
    public static void VerifyExpression(this bool condition,
        [CallerArgumentExpression(nameof(condition))] string? expression = null) =>
        VerifyExpression(condition, new AssertionCapture(expression ?? "condition", null));

    /// <summary>
    /// Verifies that <paramref name="condition"/> is true using an explicit <see cref="AssertionCapture"/> (source text and optional operand snapshots).
    /// Intended for boolean-expression lowering in tooling; ordinary call sites should use <see cref="VerifyExpression(bool, string?)"/>.
    /// </summary>
    /// <exception cref="OmniAssertionException">Thrown when <paramref name="condition"/> is false and no enclosing <see cref="AssertionScope"/> is collecting failures.</exception>
    public static void VerifyExpression(this bool condition, AssertionCapture capture)
    {
        if (condition)
            return;

        VerificationFlow.Fail(OmniAssertionException.ForBooleanFailure(in capture));
    }

    /// <summary>Begins verifying a double subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<double> Must(this double actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a float subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<float> Must(this float actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a long subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<long> Must(this long actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a decimal subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<decimal> Must(this decimal actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying an integer subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<int> Must(this int actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a short subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<short> Must(this short actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a byte subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<byte> Must(this byte actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a uint subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<uint> Must(this uint actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a ulong subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<ulong> Must(this ulong actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a ushort subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<ushort> Must(this ushort actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying an sbyte subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<sbyte> Must(this sbyte actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a BigInteger subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NumericAssertions{T}"/> object to continue the verification.</returns>
    public static NumericAssertions<BigInteger> Must(this BigInteger actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a GUID subject.</summary>
    /// <param name="actual">The GUID value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="GuidAssertions"/> object to continue the verification.</returns>
    public static GuidAssertions Must(this Guid actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a string subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="StringAssertions"/> object to continue the verification.</returns>
    public static StringAssertions Must(this string? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a URI subject.</summary>
    /// <param name="actual">The URI value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="UriAssertions"/> object to continue the verification.</returns>
    public static UriAssertions Must(this Uri? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a collection subject.</summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="actual">The collection to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="CollectionAssertions{T}"/> object to continue the verification.</returns>
    public static CollectionAssertions<T> Must<T>(this IEnumerable<T>? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual!, expression ?? "actual");

    /// <summary>Begins verifying a dictionary subject.</summary>
    /// <typeparam name="TKey">The dictionary key type.</typeparam>
    /// <typeparam name="TValue">The dictionary value type.</typeparam>
    /// <param name="actual">The dictionary to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="DictionaryAssertions{TKey, TValue}"/> object to continue the verification.</returns>
    public static DictionaryAssertions<TKey, TValue> Must<TKey, TValue>(this Dictionary<TKey, TValue>? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        Must((IReadOnlyDictionary<TKey, TValue>?)actual, expression);

    /// <summary>Begins verifying a dictionary subject.</summary>
    /// <typeparam name="TKey">The dictionary key type.</typeparam>
    /// <typeparam name="TValue">The dictionary value type.</typeparam>
    /// <param name="actual">The dictionary to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="DictionaryAssertions{TKey, TValue}"/> object to continue the verification.</returns>
    public static DictionaryAssertions<TKey, TValue> Must<TKey, TValue>(this IDictionary<TKey, TValue>? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual is null ? null : new ReadOnlyDictionaryWrapper<TKey, TValue>(actual), expression ?? "actual");

    /// <summary>Begins verifying a read-only dictionary subject.</summary>
    /// <typeparam name="TKey">The dictionary key type.</typeparam>
    /// <typeparam name="TValue">The dictionary value type.</typeparam>
    /// <param name="actual">The dictionary to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="DictionaryAssertions{TKey, TValue}"/> object to continue the verification.</returns>
    public static DictionaryAssertions<TKey, TValue> Must<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue>? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual!, expression ?? "actual");

    /// <summary>Verifies that a file exists at <paramref name="path"/> and returns file assertions.</summary>
    /// <param name="path">The file path to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="FileAssertions"/> object to continue file verification.</returns>
    public static FileAssertions FileExists(this string path, [CallerArgumentExpression(nameof(path))] string? expression = null)
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
    public static DirectoryAssertions DirectoryExists(this string path, [CallerArgumentExpression(nameof(path))] string? expression = null)
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
    public static EnumAssertions<T> Must<T>(this T actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) where T : struct, Enum =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a nullable value type or reference type subject.</summary>
    /// <typeparam name="T">The underlying reference type.</typeparam>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NullableReferenceAssertions{T}"/> object to continue the verification.</returns>
    public static NullableReferenceAssertions<T> VerifyNullable<T>(this T? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) where T : class =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a nullable value type subject.</summary>
    /// <typeparam name="T">The underlying value type.</typeparam>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="NullableValueAssertions{T}"/> object to continue the verification.</returns>
    public static NullableValueAssertions<T> VerifyNullable<T>(this T? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) where T : struct =>
        new(actual, expression ?? "actual");

    /// <summary>Verifies that the given action throws an exception of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The expected exception type.</typeparam>
    /// <param name="action">The action that is expected to throw.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>An <see cref="ExceptionAssertions{T}"/> object to continue verifying the exception.</returns>
    public static ExceptionAssertions<T> Throws<T>(this Action action, [CallerArgumentExpression(nameof(action))] string? expression = null) where T : Exception
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

    /// <summary>Verifies that the given function throws an exception of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The expected exception type.</typeparam>
    /// <param name="func">The function that is expected to throw (its return value is ignored).</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>An <see cref="ExceptionAssertions{T}"/> object to continue verifying the exception.</returns>
    public static ExceptionAssertions<T> Throws<T>(this Func<object?> func, [CallerArgumentExpression(nameof(func))] string? expression = null) where T : Exception
    {
        try
        {
            func();
        }
        catch (T ex)
        {
            return new ExceptionAssertions<T>(ex, expression ?? "func");
        }
        catch (Exception ex)
        {
            VerificationFlow.Fail($"Verification failed: expected {expression ?? "func"} to throw {typeof(T).Name}, but it threw {ex.GetType().Name}.", expression ?? "func");
        }

        VerificationFlow.Fail($"Verification failed: expected {expression ?? "func"} to throw {typeof(T).Name}, but it did not throw.", expression ?? "func");
        throw new UnreachableException();
    }

    /// <summary>Verifies that the given action does not throw any exception.</summary>
    /// <param name="action">The action that should not throw.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    public static void NotThrow(this Action action, [CallerArgumentExpression(nameof(action))] string? expression = null)
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

    /// <summary>Verifies that the given function does not throw any exception.</summary>
    /// <param name="func">The function that should not throw (its return value is ignored).</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    public static void NotThrow(this Func<object?> func, [CallerArgumentExpression(nameof(func))] string? expression = null)
    {
        try
        {
            func();
        }
        catch (Exception ex)
        {
            VerificationFlow.Fail($"Verification failed: expected {expression ?? "func"} not to throw, but it threw {ex.GetType().Name}: {ex.Message}", expression ?? "func");
        }
    }

    /// <summary>Verifies that the given asynchronous action throws an exception of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The expected exception type.</typeparam>
    /// <param name="action">The asynchronous action that is expected to throw.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A task representing the asynchronous operation, containing <see cref="ExceptionAssertions{T}"/> to continue verifying the exception.</returns>
    public static async Task<ExceptionAssertions<T>> ThrowsAsync<T>(this Func<Task> action, [CallerArgumentExpression(nameof(action))] string? expression = null) where T : Exception
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

    /// <summary>Verifies that the given asynchronous function throws an exception of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The expected exception type.</typeparam>
    /// <param name="func">The asynchronous function that is expected to throw (its return value is ignored).</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A task representing the asynchronous operation, containing <see cref="ExceptionAssertions{T}"/> to continue verifying the exception.</returns>
    public static async Task<ExceptionAssertions<T>> ThrowsAsync<T>(this Func<Task<object?>> func, [CallerArgumentExpression(nameof(func))] string? expression = null) where T : Exception
    {
        try
        {
            await func();
        }
        catch (T ex)
        {
            return new ExceptionAssertions<T>(ex, expression ?? "func");
        }
        catch (Exception ex)
        {
            VerificationFlow.Fail($"Verification failed: expected {expression ?? "func"} to throw {typeof(T).Name}, but it threw {ex.GetType().Name}.", expression ?? "func");
        }

        VerificationFlow.Fail($"Verification failed: expected {expression ?? "func"} to throw {typeof(T).Name}, but it did not throw.", expression ?? "func");
        throw new UnreachableException();
    }

    /// <summary>Verifies that the given asynchronous action does not throw any exception.</summary>
    /// <param name="action">The asynchronous action that should not throw.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task NotThrowAsync(this Func<Task> action, [CallerArgumentExpression(nameof(action))] string? expression = null)
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

    /// <summary>Verifies that the given asynchronous function does not throw any exception.</summary>
    /// <param name="func">The asynchronous function that should not throw (its return value is ignored).</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task NotThrowAsync(this Func<Task<object?>> func, [CallerArgumentExpression(nameof(func))] string? expression = null)
    {
        try
        {
            await func();
        }
        catch (Exception ex)
        {
            VerificationFlow.Fail($"Verification failed: expected {expression ?? "func"} not to throw, but it threw {ex.GetType().Name}: {ex.Message}", expression ?? "func");
        }
    }

    /// <summary>Verifies that the given asynchronous action completes within the specified time span.</summary>
    /// <param name="timeout">The maximum allowed duration.</param>
    /// <param name="action">The asynchronous action to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task CompleteWithin(this TimeSpan timeout, Func<Task> action, [CallerArgumentExpression(nameof(action))] string? expression = null)
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
    public static DateTimeAssertions Must(this DateTime actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a DateTimeOffset subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="DateTimeOffsetAssertions"/> object to continue the verification.</returns>
    public static DateTimeOffsetAssertions Must(this DateTimeOffset actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a TimeSpan subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="TimeSpanAssertions"/> object to continue the verification.</returns>
    public static TimeSpanAssertions Must(this TimeSpan actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a DateOnly subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="DateOnlyAssertions"/> object to continue the verification.</returns>
    public static DateOnlyAssertions Must(this DateOnly actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a TimeOnly subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="TimeOnlyAssertions"/> object to continue the verification.</returns>
    public static TimeOnlyAssertions Must(this TimeOnly actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a <see cref="ReadOnlySpan{T}"/> subject without any allocation.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="actual">The span to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="SpanAssertions{T}"/> object to continue the verification.</returns>
    public static SpanAssertions<T> Must<T>(this ReadOnlySpan<T> actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a <see cref="Span{T}"/> subject without any allocation.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="actual">The span to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="SpanAssertions{T}"/> object to continue the verification.</returns>
    public static SpanAssertions<T> Must<T>(this Span<T> actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a <see cref="ReadOnlyMemory{T}"/> subject.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="actual">The memory to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="SpanAssertions{T}"/> object to continue the verification.</returns>
    public static SpanAssertions<T> Must<T>(this ReadOnlyMemory<T> actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual.Span, expression ?? "actual");

    /// <summary>Begins verifying a <see cref="Memory{T}"/> subject.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="actual">The memory to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="SpanAssertions{T}"/> object to continue the verification.</returns>
    public static SpanAssertions<T> Must<T>(this Memory<T> actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual.Span, expression ?? "actual");

    /// <summary>Begins verifying an object subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>An <see cref="ObjectAssertions"/> object to continue the verification.</returns>
    public static ObjectAssertions Must(this object? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
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


