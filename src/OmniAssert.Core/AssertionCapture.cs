namespace OmniAssert;

/// <summary>
/// Source expression text and optional operand snapshot for structured failures (expression + captured sub-values).
/// Produced by <see cref="Assert.VerifyExpression"/>, emitted interceptors, or
/// boolean-expression lowering in the <c>OmniAssert.Generator</c> tooling layer.
/// </summary>
/// <param name="sourceExpression">Expression text or a synthetic label shown on <see cref="OmniAssertionException"/>.</param>
/// <param name="capturedValues">Operand map for diagnostics; <c>null</c> when only the expression string is available.</param>
public readonly struct AssertionCapture(
    string sourceExpression,
    IReadOnlyDictionary<string, object?>? capturedValues = null)
{
    public string SourceExpression { get; } = sourceExpression;

    public IReadOnlyDictionary<string, object?>? CapturedValues { get; } = capturedValues;

    /// <summary>Builds a capture with explicit operand snapshots (for tests and tooling).</summary>
    public static AssertionCapture WithOperands(string sourceExpression, params (string name, object? value)[] operands)
    {
        var dict = new Dictionary<string, object?>(operands.Length);
        foreach (var (name, value) in operands)
            dict[name] = value;
        return new AssertionCapture(sourceExpression, dict);
    }
}
