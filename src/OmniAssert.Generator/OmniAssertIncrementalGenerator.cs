using Microsoft.CodeAnalysis;

namespace OmniAssert.Generator;

/// <summary>
/// Incremental source generator placeholder for future OmniAssert compile-time features.
/// Boolean <c>Assert.Verify</c> rewriting is performed by <c>OmniAssert.Build</c>.
/// </summary>
[Generator]
public sealed class OmniAssertIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Reserved for incremental helpers (e.g. interceptor metadata) without slowing the IDE.
    }
}
