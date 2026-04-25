using static OmniAssert.Assert;

namespace VerifyInterceptorsSample;

internal static class Program
{
    private static void Main()
    {
        // Requires OmniAssertEnableVerifyInterceptors + InterceptorsNamespaces (see README).
        var x = 2;
        var y = 3;
        VerifyExpression(x < y);
    }
}
