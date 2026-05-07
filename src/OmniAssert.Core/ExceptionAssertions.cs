using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>Fluent follow-ups after <see cref="Assert"/> exception helpers (for example <c>Throws&lt;T&gt;</c> / <c>ThrowsAsync&lt;T&gt;</c>).</summary>
/// <typeparam name="T">The caught exception type.</typeparam>
public readonly struct ExceptionAssertions<T> where T : Exception
{
    /// <summary>The exception instance under test.</summary>
    public T Exception { get; }
    private readonly string _expression;

    internal ExceptionAssertions(T exception, string expression)
    {
        Exception = exception;
        _expression = expression;
    }

    /// <summary>Verifies that <see cref="Exception.Message"/> is exactly <paramref name="expectedMessage"/>.</summary>
    /// <param name="expectedMessage">The expected exception message.</param>
    /// <param name="expectedMessageExpression">The expression for the expected message (automatically captured).</param>
    /// <returns><c>this</c> when the message matches.</returns>
    public ExceptionAssertions<T> WithMessage(string expectedMessage, [CallerArgumentExpression(nameof(expectedMessage))] string? expectedMessageExpression = null)
    {
        if (Exception.Message == expectedMessage)
            return this;

        VerificationFlow.Fail($"Verification failed: expected exception message to be {expectedMessageExpression ?? "message"} (\"{expectedMessage}\"), but was \"{Exception.Message}\".", _expression);
        return this;
    }

    /// <summary>Verifies that <see cref="Exception.Message"/> contains <paramref name="expectedSubstring"/>.</summary>
    /// <param name="expectedSubstring">The substring that must appear in the exception message.</param>
    /// <param name="expectedSubstringExpression">The expression for the expected substring (automatically captured).</param>
    /// <returns><c>this</c> when the message contains the substring.</returns>
    public ExceptionAssertions<T> WithMessageContaining(string expectedSubstring, [CallerArgumentExpression(nameof(expectedSubstring))] string? expectedSubstringExpression = null)
    {
        if (Exception.Message.Contains(expectedSubstring, StringComparison.Ordinal))
            return this;

        VerificationFlow.Fail(
            $"Verification failed: expected exception message to contain {expectedSubstringExpression ?? "substring"} (\"{expectedSubstring}\"), but was \"{Exception.Message}\".",
            _expression);
        return this;
    }

    /// <summary>Verifies that <see cref="Exception.InnerException"/> is an instance of <typeparamref name="TInner"/>.</summary>
    /// <returns><c>this</c> when the inner exception type matches.</returns>
    public ExceptionAssertions<T> WithInnerException<TInner>() where TInner : Exception
    {
        if (Exception.InnerException is TInner)
            return this;

        var actualType = Exception.InnerException?.GetType().Name ?? "null";
        VerificationFlow.Fail($"Verification failed: expected inner exception of type {typeof(TInner).Name}, but was {actualType}.", _expression);
        return this;
    }
}
