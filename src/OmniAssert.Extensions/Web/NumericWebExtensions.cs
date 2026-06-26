using OmniAssert.Extensions.Internal;

namespace OmniAssert.Extensions.Web;

/// <summary>Web and HTTP-related assertions for integer numeric subjects from <see cref="Ensure.Must(int, string?)"/>.</summary>
public static class NumericWebExtensions
{
    /// <summary>Verifies that the port number is within the valid range <c>0</c>–<c>65535</c>.</summary>
    /// <param name="assertions">The numeric assertions chain from <see cref="Ensure.Must(int, string?)"/>.</param>
    public static void BeValidPort(this NumericAssertions<int> assertions)
    {
        var (actual, expression) = ((IAssertionContext<int>)assertions).Unwrap();

        if (actual >= 0 && actual <= 65535)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {expression} to be a valid port (0-65535), but was {actual}.",
            expression);
    }

    /// <summary>Verifies that the value equals the expected HTTP status code.</summary>
    /// <param name="assertions">The numeric assertions chain from <see cref="Ensure.Must(int, string?)"/>.</param>
    /// <param name="expected">The expected status code (for example <c>200</c> or <c>404</c>).</param>
    public static void BeHttpStatusCode(this NumericAssertions<int> assertions, int expected)
    {
        var (actual, expression) = ((IAssertionContext<int>)assertions).Unwrap();

        if (actual == expected)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {expression} to be HTTP status code {expected}, but was {actual}.",
            expression);
    }

    /// <summary>Verifies that the value is a success HTTP status code (<c>2xx</c>).</summary>
    /// <param name="assertions">The numeric assertions chain from <see cref="Ensure.Must(int, string?)"/>.</param>
    public static void BeSuccessStatusCode(this NumericAssertions<int> assertions)
    {
        var (actual, expression) = ((IAssertionContext<int>)assertions).Unwrap();

        if (actual >= 200 && actual <= 299)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {expression} to be a success status code (200-299), but was {actual}.",
            expression);
    }
}
