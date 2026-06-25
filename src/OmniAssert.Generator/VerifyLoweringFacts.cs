using Microsoft.CodeAnalysis;

namespace OmniAssert.Generator;

/// <summary>
/// Identifies interceptable <c>Ensure.Expression(bool, string?)</c>, legacy extension
/// <c>VerifyExpression(bool, string?)</c>, and obsolete <c>Assert.VerifyExpression</c> for the incremental generator.
/// </summary>
internal static class VerifyLoweringFacts
{
    /// <summary>Returns whether <paramref name="sym"/> is the public boolean <c>VerifyExpression</c> overload on <c>Ensure</c> or obsolete <c>Assert</c>.</summary>
    public static bool IsAssertVerifyExpression(IMethodSymbol sym) =>
        IsInterceptableBooleanExpression(sym) && sym.Name == "VerifyExpression";

    /// <summary>Returns whether <paramref name="sym"/> is the static boolean <c>Expression</c> overload on <c>Ensure</c>.</summary>
    public static bool IsEnsureExpression(IMethodSymbol sym) =>
        IsInterceptableBooleanExpression(sym) && sym.Name == "Expression";

    /// <summary>Returns whether <paramref name="sym"/> is an interceptable boolean expression assertion entry point.</summary>
    public static bool IsInterceptableBooleanExpression(IMethodSymbol sym)
    {
        if (sym.Name is not ("Expression" or "VerifyExpression"))
            return false;

        if (sym.ContainingType?.Name is not ("Assert" or "Ensure") || sym.ContainingNamespace?.Name != "OmniAssert")
            return false;

        if (sym.Name == "Expression" && sym.ContainingType.Name != "Ensure")
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
