using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>Entry points for OmniAssert verifications.</summary>
public static partial class Assert
{
    /// <summary>
    /// Verifies that a boolean condition holds. 
    /// </summary>
    /// <remarks>
    /// Prefer this form in user code; the OmniAssert build task rewrites call sites to capture operand values 
    /// for more descriptive failure messages.
    /// </remarks>
    /// <param name="condition">The condition to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    public static void Verify(bool condition, [CallerArgumentExpression(nameof(condition))] string? expression = null)
    {
        var capture = new AssertionCapture(expression ?? "condition", null);
        VerifyBoolean(condition, in capture);
    }

    /// <summary>
    /// Invoked from rewritten code with structured captures. Not intended for direct use.
    /// </summary>
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

    /// <summary>Begins verifying a boolean subject.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="BoolAssertions"/> object to continue the verification.</returns>
    public static BoolAssertions VerifyBool(bool actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

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

    /// <summary>Begins verifying a subject's type.</summary>
    /// <param name="actual">The value to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A <see cref="TypeAssertions"/> object to continue the verification.</returns>
    public static TypeAssertions VerifyType(object? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null) =>
        new(actual, expression ?? "actual");

    /// <summary>Compares two objects by walking public properties and reports a structured diff on mismatch.</summary>
    /// <param name="expected">The expected object state.</param>
    /// <param name="actual">The actual object to compare.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    public static void VerifyEquivalent(object? expected, object? actual, [CallerArgumentExpression(nameof(actual))] string? expression = null)
    {
        var diff = ObjectDiffWalker.Diff(expected, actual, expression ?? "actual");
        if (diff is null)
            return;

        var message = diff.FormatMessage();
        var capture = new AssertionCapture(expression ?? "actual", null);
        var ex = new OmniAssertionException(message, capture);
        var ctx = AssertionScope.Current;
        if (ctx is not null)
        {
            ctx.Failures.Add(ex);
            return;
        }

        throw ex;
    }
}

