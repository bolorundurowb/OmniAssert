using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>Assertions for <see cref="Guid"/> subjects from <see cref="Assert.Verify(Guid, string?)"/>.</summary>
public readonly struct GuidAssertions
{
    private readonly Guid _actual;
    private readonly string _expression;

    internal GuidAssertions(Guid actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    /// <summary>Verifies that the GUID is <see cref="Guid.Empty"/>.</summary>
    public void ToBeEmpty()
    {
        if (_actual == Guid.Empty)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be Guid.Empty, but was {FormatGuid(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the GUID is not <see cref="Guid.Empty"/>.</summary>
    public void NotToBeEmpty()
    {
        if (_actual != Guid.Empty)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} not to be Guid.Empty, but it was {FormatGuid(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the GUID matches one of the provided expected values.</summary>
    /// <param name="expected">Allowed GUID values.</param>
    public void ToBeOneOf(params Guid[] expected)
    {
        if (expected is not null)
        {
            foreach (var candidate in expected)
            {
                if (_actual == candidate)
                    return;
            }
        }

        var expectedList = expected is null || expected.Length == 0
            ? "[]"
            : $"[{string.Join(", ", expected.Select(FormatGuid))}]";
        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be one of {expectedList}, but was {FormatGuid(_actual)}.",
            _expression);
    }

    private static string FormatGuid(Guid value) => OmniAssertionException.FormatValueForMessage(value);
}
