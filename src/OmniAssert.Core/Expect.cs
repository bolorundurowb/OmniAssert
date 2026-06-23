using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>
/// Alternative entry point to <see cref="Ensure"/> that avoids naming collisions with other test frameworks
/// (for example NUnit's <c>Assert</c>). Use <see cref="Throws{T}(Action, string?)"/> and friends
/// when <c>Assert</c> is ambiguous or shadowed.
/// </summary>
/// <remarks>
/// <para>This class is intentionally a thin wrapper around the equivalent <see cref="Ensure"/> helpers;
/// it exists only to provide a non-conflicting name.</para>
/// </remarks>
public static class Expect
{
    /// <summary>Verifies that the given action throws an exception of type <typeparamref name="T"/>.</summary>
    public static ExceptionAssertions<T> Throws<T>(Action action, [CallerArgumentExpression(nameof(action))] string? expression = null) where T : Exception
        => Ensure.Throws<T>(action, expression);

    /// <summary>Verifies that the given function throws an exception of type <typeparamref name="T"/>.</summary>
    public static ExceptionAssertions<T> Throws<T>(Func<object?> func, [CallerArgumentExpression(nameof(func))] string? expression = null) where T : Exception
        => Ensure.Throws<T>(func, expression);

    /// <summary>Verifies that the given action does not throw any exception.</summary>
    public static void NotThrow(Action action, [CallerArgumentExpression(nameof(action))] string? expression = null)
        => Ensure.NotThrow(action, expression);

    /// <summary>Verifies that the given function does not throw any exception.</summary>
    public static void NotThrow(Func<object?> func, [CallerArgumentExpression(nameof(func))] string? expression = null)
        => Ensure.NotThrow(func, expression);

    /// <summary>Verifies that the given asynchronous action throws an exception of type <typeparamref name="T"/>.</summary>
    public static Task<ExceptionAssertions<T>> ThrowsAsync<T>(Func<Task> action, [CallerArgumentExpression(nameof(action))] string? expression = null) where T : Exception
        => Ensure.ThrowsAsync<T>(action, expression);

    /// <summary>Verifies that the given asynchronous function throws an exception of type <typeparamref name="T"/>.</summary>
    public static Task<ExceptionAssertions<T>> ThrowsAsync<T>(Func<Task<object?>> func, [CallerArgumentExpression(nameof(func))] string? expression = null) where T : Exception
        => Ensure.ThrowsAsync<T>(func, expression);

    /// <summary>Verifies that the given asynchronous action does not throw any exception.</summary>
    public static Task NotThrowAsync(Func<Task> action, [CallerArgumentExpression(nameof(action))] string? expression = null)
        => Ensure.NotThrowAsync(action, expression);

    /// <summary>Verifies that the given asynchronous function does not throw any exception.</summary>
    public static Task NotThrowAsync(Func<Task<object?>> func, [CallerArgumentExpression(nameof(func))] string? expression = null)
        => Ensure.NotThrowAsync(func, expression);

    /// <summary>Verifies that the given asynchronous action completes within the specified time span.</summary>
    public static Task CompleteWithin(TimeSpan timeout, Func<Task> action, [CallerArgumentExpression(nameof(action))] string? expression = null)
        => Ensure.CompleteWithin(timeout, action, expression);
}
