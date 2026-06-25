using Microsoft.CodeAnalysis.CSharp;

namespace OmniAssert.Generator;

/// <summary>One generated stub: compiler location plus routing metadata for the emitted interceptor body.</summary>
internal readonly struct InterceptorCandidate
{
    public InterceptorCandidate(InterceptableLocation location, bool simpleIdentifierPath, bool extensionCallSite)
    {
        Location = location;
        SimpleIdentifierPath = simpleIdentifierPath;
        ExtensionCallSite = extensionCallSite;
    }

    /// <summary>Interceptable location metadata for <see cref="InterceptsLocationAttribute"/>.</summary>
    public InterceptableLocation Location { get; }

    /// <summary>
    /// <c>true</c> when the first argument is a bare identifier (or parenthesized identifier), matching
    /// <see cref="Rewrite.VerifyExpansionEngine.IsSimpleBooleanIdentifier"/>—emit <c>Must(...).BeTrue()</c>.
    /// </summary>
    public bool SimpleIdentifierPath { get; }

    /// <summary>
    /// <c>true</c> for extension call sites such as <c>expr.VerifyExpression()</c>; <c>false</c> for static
    /// <c>Ensure.Expression(expr)</c> or <c>Assert.VerifyExpression(expr)</c>.
    /// </summary>
    public bool ExtensionCallSite { get; }
}
