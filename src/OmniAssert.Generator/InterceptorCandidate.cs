using Microsoft.CodeAnalysis.CSharp;

namespace OmniAssert.Generator;

/// <summary>One generated stub: compiler location plus whether the condition is a simple identifier.</summary>
internal readonly struct InterceptorCandidate
{
    public InterceptorCandidate(InterceptableLocation location, bool simpleIdentifierPath)
    {
        Location = location;
        SimpleIdentifierPath = simpleIdentifierPath;
    }

    /// <summary>Interceptable location metadata for <see cref="InterceptsLocationAttribute"/>.</summary>
    public InterceptableLocation Location { get; }

    /// <summary>
    /// <c>true</c> when the first argument is a bare identifier (or parenthesized identifier), matching
    /// <see cref="Rewrite.VerifyExpansionEngine.IsSimpleBooleanIdentifier"/>—emit <c>Verify(...).ToBeTrue()</c>.
    /// </summary>
    public bool SimpleIdentifierPath { get; }
}
