namespace OmniAssert;

internal static class VerificationFlow
{
    public static void Fail(string message, string subjectExpression)
    {
        var capture = new AssertionCapture(subjectExpression, null);
        var ex = new OmniAssertionException(message, capture);
        var ctx = AssertionScope.Current;
        if (ctx is not null)
        {
            ctx.Failures.Add(ex);
            return;
        }

        throw ex;
    }
}
