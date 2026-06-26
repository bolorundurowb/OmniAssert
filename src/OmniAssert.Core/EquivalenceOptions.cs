namespace OmniAssert;

/// <summary>
/// Options that relax how <c>BeEquivalentTo</c> compares values. This replaces the previously
/// suggested <c>BeQuasiEquivalentTo</c> idea: rather than a separate method with a vague name,
/// equivalence is tuned through this options value.
/// </summary>
/// <remarks>
/// Migration: <c>BeQuasiEquivalentTo(expected)</c> becomes
/// <c>BeEquivalentTo(expected, new EquivalenceOptions { IgnoreCase = true, IgnoreCollectionOrder = true })</c>.
/// </remarks>
public readonly struct EquivalenceOptions
{
    /// <summary>When set, <see cref="string"/> values are compared using <see cref="StringComparison.OrdinalIgnoreCase"/>.</summary>
    public bool IgnoreCase { get; init; }

    /// <summary>
    /// When set, enumerable members of an object graph are compared as multisets (order ignored).
    /// Top-level collection equivalence is already order-independent; this primarily affects nested sequences.
    /// </summary>
    public bool IgnoreCollectionOrder { get; init; }

    /// <summary>The default options: case-sensitive, order-sensitive for nested sequences.</summary>
    public static EquivalenceOptions Default => default;
}
