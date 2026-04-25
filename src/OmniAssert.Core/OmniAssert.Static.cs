using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>
/// Static entry points for verifications: fluent <c>Verify(...)</c> overloads, boolean <see cref="VerifyExpression"/>,
/// exception helpers, and <see cref="VerifyBoolean"/> (used by interceptors and Roslyn lowering, not typical test code).
/// </summary>
public static partial class Assert
{
    /// <summary>Begins verifying a boolean subject; chain <see cref="BoolAssertions.ToBeTrue"/> or <see cref="BoolAssertions.ToBeFalse"/>.</summary>
    /// <param name="actual">The boolean value under test.</param>
    /// <param name="expression">Caller expression text for failures (compiler-supplied via <c>[CallerArgumentExpression]</c>).</param>
    /// <returns>Fluent assertions for the boolean.</returns>
    public static BoolAssertions Verify(bool actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>
    /// Asserts that <paramref name="condition"/> is <c>true</c> immediately, with structured failure output (expression line and optional captured operands).
    /// </summary>
    /// <remarks>
    /// With C# interceptors enabled (<c>OmniAssertEnableVerifyInterceptors</c>, analyzer <c>OmniAssert.Generator</c>,
    /// <c>InterceptorsNamespaces</c> including <c>OmniAssert.Generated</c>), call sites are rewritten: bare identifiers
    /// become <c>Verify(condition, expression).ToBeTrue()</c>; other expressions become
    /// <see cref="VerifyBoolean"/> with an expression-only <see cref="AssertionCapture"/>. See the repository README.
    /// </remarks>
    /// <param name="condition">The boolean result; must be <c>true</c>.</param>
    /// <param name="expression">Caller expression text stored on the capture when the assertion fails.</param>
    public static void VerifyExpression(bool condition, [CallerArgumentExpression(nameof(condition))] string? expression = null)
    {
        var capture = new AssertionCapture(expression ?? "condition", null);
        VerifyBoolean(condition, in capture);
    }

    /// <summary>
    /// Low-level boolean check with a pre-built <see cref="AssertionCapture"/>. Used by <see cref="VerifyExpression"/>,
    /// generated interceptors, and <c>VerifyExpansionEngine</c> output—not a normal application-level API.
    /// </summary>
    /// <param name="result"><c>true</c> passes; <c>false</c> records or throws using <paramref name="capture"/>.</param>
    /// <param name="capture">Source expression and optional operand dictionary for failure output.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void VerifyBoolean(bool result, in AssertionCapture capture)
    {
        if (result)
            return;

        var ex = OmniAssertionException.ForBooleanFailure(in capture);
        var ctx = AssertionScope.Current;
        if (ctx is not null)
        {
            ctx.Failures.Add(ex);
            return;
        }

        throw ex;
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

    /// <summary>Begins verifying a string subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="StringAssertions"/> object to continue the verification.</returns>
    public static StringAssertions Verify(string? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Begins verifying a collection subject.</summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="actual">The collection to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="CollectionAssertions{T}"/> object to continue the verification.</returns>
    public static CollectionAssertions<T> Verify<T>(IEnumerable<T> actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

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

    /// <summary>Begins verifying an object subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>An <see cref="ObjectAssertions"/> object to continue the verification.</returns>
    public static ObjectAssertions Verify(object? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");
}

