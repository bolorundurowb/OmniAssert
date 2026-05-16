using OmniAssert;

namespace VerifyInterceptorsSample;

public static class Program
{
    public static void Main()
    {
        // See README: OmniAssertEnableVerifyInterceptors and InterceptorsNamespaces.
        var x = 2;
        var y = 3;
        var z = 10;
        InterceptorSmoke.Run();
        (z > 10 || x > y).VerifyExpression();
    }
}
