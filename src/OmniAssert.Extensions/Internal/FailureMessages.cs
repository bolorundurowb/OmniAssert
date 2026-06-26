namespace OmniAssert.Extensions.Internal;

internal static class FailureMessages
{
    internal static string ExpectedGot(string expression, string expectation, string? actual)
    {
        var actualDisplay = actual is null ? "null" : $"""{actual}""";
        return $"Verification failed: expected {expression} {expectation}, but was {actualDisplay}.";
    }

    internal static string ExpectedOnly(string expression, string expectation)
        => $"Verification failed: expected {expression} {expectation}.";
}
