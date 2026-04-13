namespace OmniAssert;

/// <summary>Structured capture for Power-Assert style diagnostics (populated by the build-time rewriter).</summary>
public readonly struct AssertionCapture(
    string sourceExpression,
    IReadOnlyDictionary<string, object?>? capturedValues = null)
{
    public string SourceExpression { get; } = sourceExpression;

    public IReadOnlyDictionary<string, object?>? CapturedValues { get; } = capturedValues;
}
