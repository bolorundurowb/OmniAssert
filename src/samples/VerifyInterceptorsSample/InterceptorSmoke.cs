using static OmniAssert.Assert;

namespace VerifyInterceptorsSample;

/// <summary>Holds a compiled <see cref="Assert.VerifyExpression(bool)"/> call site so interceptors are generated; <c>Program.cs</c> is supplied only as an AdditionalFile for rewrite.</summary>
internal static class InterceptorSmoke
{
    internal static void Run()
    {
        var flag = true;
        VerifyExpression(flag);
    }
}
