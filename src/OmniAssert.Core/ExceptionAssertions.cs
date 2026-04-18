using System.Runtime.CompilerServices;

namespace OmniAssert;

public readonly struct ExceptionAssertions<T> where T : Exception
{
    public T Exception { get; }
    private readonly string _expression;

    internal ExceptionAssertions(T exception, string expression)
    {
        Exception = exception;
        _expression = expression;
    }

    public ExceptionAssertions<T> WithMessage(string expectedMessage, [CallerArgumentExpression(nameof(expectedMessage))] string? expectedMessageExpression = null)
    {
        if (Exception.Message == expectedMessage)
            return this;

        VerificationFlow.Fail($"Verification failed: expected exception message to be {expectedMessageExpression ?? "message"} (\"{expectedMessage}\"), but was \"{Exception.Message}\".", _expression);
        return this;
    }

    public ExceptionAssertions<T> WithInnerException<TInner>() where TInner : Exception
    {
        if (Exception.InnerException is TInner)
            return this;

        var actualType = Exception.InnerException?.GetType().Name ?? "null";
        VerificationFlow.Fail($"Verification failed: expected inner exception of type {typeof(TInner).Name}, but was {actualType}.", _expression);
        return this;
    }
}
