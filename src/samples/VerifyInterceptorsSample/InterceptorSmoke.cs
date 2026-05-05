using static OmniAssert.Assert;

namespace VerifyInterceptorsSample;

/// <summary>Compiled entry point so the generator can emit interceptors for a real syntax tree (Program.cs is only an AdditionalFile for rewrite).</summary>
internal static class InterceptorSmoke
{
    internal static void Run()
    {
        var flag = true;
        VerifyExpression(flag);
    }
}
