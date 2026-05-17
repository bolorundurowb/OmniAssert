using Microsoft.CodeAnalysis;

namespace OmniAssert.Generator;

/// <summary>Identifies <c>Assert.VerifyExpression(bool, string?)</c> for the incremental generator.</summary>
internal static class VerifyLoweringFacts
{
    /// <summary>Returns whether <paramref name="sym"/> is the public boolean <c>VerifyExpression</c> overload on <c>Assert</c>.</summary>
    public static bool IsAssertVerifyExpression(IMethodSymbol sym)
    {
        if (sym.Name != "VerifyExpression")
            return false;

        if (sym.ContainingType?.Name != "Assert" || sym.ContainingNamespace?.Name != "OmniAssert")
            return false;

        // When called in extension-method form (receiver.VerifyExpression()), Roslyn provides the
        // reduced symbol whose Parameters collection omits the 'this' receiver parameter.
        // Use ReducedFrom (the original non-reduced definition) so Parameters[0] is the bool.
        var original = sym.ReducedFrom ?? sym;

        if (original.Parameters.Length < 1 || original.Parameters.Length > 2)
            return false;

        if (original.Parameters[0].Type.SpecialType != SpecialType.System_Boolean)
            return false;

        if (original.Parameters.Length == 2)
        {
            var t = original.Parameters[1].Type;
            if (t.SpecialType != SpecialType.System_String && t.Name != "String" && t.ToDisplayString() != "string?")
                return false;
        }

        return true;
    }
}
