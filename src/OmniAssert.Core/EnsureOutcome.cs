namespace OmniAssert;

public static partial class Ensure
{
    /// <summary>
    /// Explicitly marks a verification as successful without performing any checks.
    /// Useful for placeholder tests, documented no-ops, or branches that must not fail.
    /// </summary>
    public static void Succeed()
    {
    }

    /// <summary>
    /// Forces an immediate verification failure with an optional message.
    /// </summary>
    /// <param name="message">Failure detail. When omitted, a default message is used.</param>
    /// <exception cref="OmniAssertionException">Thrown when no enclosing <see cref="AssertionScope"/> is collecting failures.</exception>
    public static void Fail(string? message = null)
    {
        var text = string.IsNullOrWhiteSpace(message)
            ? "Verification failed: explicit failure."
            : message.StartsWith("Verification failed", StringComparison.Ordinal)
                ? message
                : $"Verification failed: {message}";

        VerificationFlow.Fail(text, "Ensure.Fail");
    }
}
