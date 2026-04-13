namespace OmniAssert;

/// <summary>Structured capture for Power-Assert style diagnostics (populated by the build-time rewriter).</summary>
public readonly struct AssertionCapture
{
    public AssertionCapture(string sourceExpression, IReadOnlyDictionary<string, object?>? capturedValues = null)
    {
        SourceExpression = sourceExpression;
        CapturedValues = capturedValues;
    }

    public string SourceExpression { get; }

    public IReadOnlyDictionary<string, object?>? CapturedValues { get; }
}
