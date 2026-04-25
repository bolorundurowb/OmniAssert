using static OmniAssert.Assert;

namespace VerifyInterceptorsSample;

internal static class Program
{
    private static void Main()
    {
        var x = 2;
        var y = 3;
        Verify(x < y);
    }
}
