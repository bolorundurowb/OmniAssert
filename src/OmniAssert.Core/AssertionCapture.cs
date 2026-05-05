namespace OmniAssert;

/// <summary>
/// Source expression text and optional operand snapshot for structured failures (expression + captured sub-values).
/// Produced by <see cref="Assert.VerifyExpression"/>, emitted interceptors, or
/// boolean-expression lowering in the <c>OmniAssert.Generator</c> tooling layer.
/// </summary>
/// <param name="sourceExpression">Expression text or a synthetic label shown on <see cref="OmniAssertionException"/>.</param>
/// <param name="capturedValues">Operand map for diagnostics, or <c>null</c> when only the expression label is available.</param>
public readonly struct AssertionCapture(
    string sourceExpression,
    IReadOnlyDictionary<string, object?>? capturedValues = null)
{
    /// <summary>Expression source text or a synthetic label shown in <see cref="OmniAssertionException"/> messages.</summary>
    public string SourceExpression { get; } = sourceExpression;

    /// <summary>Operand snapshots keyed by sub-expression text, or <c>null</c> when not captured.</summary>
    public IReadOnlyDictionary<string, object?>? CapturedValues { get; } = capturedValues;

    /// <summary>Builds a capture with explicit operand snapshots (for tests and tooling).</summary>
    /// <param name="sourceExpression">Label or expression text shown on failure.</param>
    /// <param name="operands">Name/value pairs merged into the operand map.</param>
    public static AssertionCapture WithOperands(string sourceExpression, params (string name, object? value)[] operands)
    {
        var dict = new Dictionary<string, object?>(operands.Length);
        foreach (var (name, value) in operands)
            dict[name] = value;
        return new AssertionCapture(sourceExpression, dict);
    }
}
