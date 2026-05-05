using static OmniAssert.Assert;

namespace VerifyInterceptorsSample;

public static class Program
{
    public static void Main()
    {
        // Requires OmniAssertEnableVerifyInterceptors + InterceptorsNamespaces (see README).
        var x = 2;
        var y = 3;
        var z = 10;
        InterceptorSmoke.Run();
        VerifyExpression(z > 10 || x > y);
    }
}
