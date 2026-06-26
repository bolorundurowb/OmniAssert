using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>Assertions for <see cref="Guid"/> subjects from <see cref="Ensure.Must(Guid, string?)"/>.</summary>
public readonly struct GuidAssertions : IAssertionContext<Guid>
{
    private readonly Guid _actual;
    private readonly string _expression;

    internal GuidAssertions(Guid actual, string expression)
    {
        _actual = actual;
        _expression = expression;
    }

    Guid IAssertionContext<Guid>.Subject => _actual;
    string IAssertionContext<Guid>.Expression => _expression;

    /// <summary>Verifies that the GUID is equal to <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected GUID.</param>
    /// <param name="expectedExpression">The expression for the expected GUID (automatically captured).</param>
    public void Be(Guid expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null)
    {
        if (_actual == expected)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be {expectedExpression ?? "expected"} ({FormatGuid(expected)}), but was {FormatGuid(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the GUID is <see cref="Guid.Empty"/>.</summary>
    public void BeEmpty()
    {
        if (_actual == Guid.Empty)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} to be Guid.Empty, but was {FormatGuid(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the GUID is not <see cref="Guid.Empty"/>.</summary>
    public void NotBeEmpty()
    {
        if (_actual != Guid.Empty)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected {_expression} not to be Guid.Empty, but it was {FormatGuid(_actual)}.",
            _expression);
    }

    /// <summary>Verifies that the GUID matches one of the provided expected values.</summary>
    /// <param name="expected">Allowed GUID values.</param>
    public void BeOneOf(params Guid[] expected)
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
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBe(Guid expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpression = null) => Be(expected, expectedExpression);
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeEmpty() => BeEmpty();
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void NotToBeEmpty() => NotBeEmpty();
    [Obsolete("Use the Ensure, Must(), and Be* fluent syntax instead.", false)]
    public void ToBeOneOf(params Guid[] expected) => BeOneOf(expected);
}
