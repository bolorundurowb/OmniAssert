namespace OmniAssert;

/// <summary>Central path for hard failures: enqueue on <see cref="AssertionScope"/> or throw <see cref="OmniAssertionException"/>.</summary>
internal static class VerificationFlow
{
    /// <param name="message">Full failure text.</param>
    /// <param name="subjectExpression">Stored on <see cref="OmniAssertionException.Capture"/> as <see cref="AssertionCapture.SourceExpression"/>.</param>
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
