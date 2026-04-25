namespace OmniAssert;

/// <summary>Structured capture for Power-Assert style diagnostics (populated by callers of <see cref="Assert.VerifyBoolean"/> or tooling that embeds captures).</summary>
public readonly struct AssertionCapture(
    string sourceExpression,
    IReadOnlyDictionary<string, object?>? capturedValues = null)
{
    public string SourceExpression { get; } = sourceExpression;

    public IReadOnlyDictionary<string, object?>? CapturedValues { get; } = capturedValues;
}
