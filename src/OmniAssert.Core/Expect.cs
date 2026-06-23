using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>
/// Alternative entry point to <see cref="Assert"/> that avoids naming collisions with other test frameworks
/// (for example NUnit's <c>Assert</c>). Use <see cref="Expect.Throws{T}(Action, string?)"/> and friends
/// when <c>Assert</c> is ambiguous or shadowed.
/// </summary>
/// <remarks>
/// <para>This class is intentionally a thin wrapper around the equivalent <see cref="Assert"/> helpers;
/// it exists only to provide a non-conflicting name.</para>
/// </remarks>
public static class Expect
{
    /// <summary>Verifies that the given action throws an exception of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The expected exception type.</typeparam>
    /// <param name="action">The action that is expected to throw.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>An <see cref="ExceptionAssertions{T}"/> object to continue verifying the exception.</returns>
    public static ExceptionAssertions<T> Throws<T>(Action action, [CallerArgumentExpression(nameof(action))] string? expression = null) where T : Exception
        => Assert.Throws<T>(action, expression);

    /// <summary>Verifies that the given function throws an exception of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The expected exception type.</typeparam>
    /// <param name="func">The function that is expected to throw (its return value is ignored).</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>An <see cref="ExceptionAssertions{T}"/> object to continue verifying the exception.</returns>
    public static ExceptionAssertions<T> Throws<T>(Func<object?> func, [CallerArgumentExpression(nameof(func))] string? expression = null) where T : Exception
        => Assert.Throws<T>(func, expression);

    /// <summary>Verifies that the given action does not throw any exception.</summary>
    /// <param name="action">The action that should not throw.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    public static void NotThrow(Action action, [CallerArgumentExpression(nameof(action))] string? expression = null)
        => Assert.NotThrow(action, expression);

    /// <summary>Verifies that the given function does not throw any exception.</summary>
    /// <param name="func">The function that should not throw (its return value is ignored).</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    public static void NotThrow(Func<object?> func, [CallerArgumentExpression(nameof(func))] string? expression = null)
        => Assert.NotThrow(func, expression);

    /// <summary>Verifies that the given asynchronous action throws an exception of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The expected exception type.</typeparam>
    /// <param name="action">The asynchronous action that is expected to throw.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A task representing the asynchronous operation, containing <see cref="ExceptionAssertions{T}"/> to continue verifying the exception.</returns>
    public static Task<ExceptionAssertions<T>> ThrowsAsync<T>(Func<Task> action, [CallerArgumentExpression(nameof(action))] string? expression = null) where T : Exception
        => Assert.ThrowsAsync<T>(action, expression);

    /// <summary>Verifies that the given asynchronous function throws an exception of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The expected exception type.</typeparam>
    /// <param name="func">The asynchronous function that is expected to throw (its return value is ignored).</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A task representing the asynchronous operation, containing <see cref="ExceptionAssertions{T}"/> to continue verifying the exception.</returns>
    public static Task<ExceptionAssertions<T>> ThrowsAsync<T>(Func<Task<object?>> func, [CallerArgumentExpression(nameof(func))] string? expression = null) where T : Exception
        => Assert.ThrowsAsync<T>(func, expression);

    /// <summary>Verifies that the given asynchronous action does not throw any exception.</summary>
    /// <param name="action">The asynchronous action that should not throw.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task NotThrowAsync(Func<Task> action, [CallerArgumentExpression(nameof(action))] string? expression = null)
        => Assert.NotThrowAsync(action, expression);

    /// <summary>Verifies that the given asynchronous function does not throw any exception.</summary>
    /// <param name="func">The asynchronous function that should not throw (its return value is ignored).</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task NotThrowAsync(Func<Task<object?>> func, [CallerArgumentExpression(nameof(func))] string? expression = null)
        => Assert.NotThrowAsync(func, expression);

    /// <summary>Verifies that the given asynchronous action completes within the specified time span.</summary>
    /// <param name="timeout">The maximum allowed duration.</param>
    /// <param name="action">The asynchronous action to verify.</param>
    /// <param name="expression">The expression being verified (automatically captured).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task CompleteWithin(TimeSpan timeout, Func<Task> action, [CallerArgumentExpression(nameof(action))] string? expression = null)
        => timeout.CompleteWithin(action, expression);
}
