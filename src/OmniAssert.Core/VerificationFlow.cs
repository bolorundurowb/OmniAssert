namespace OmniAssert;

/// <summary>Central path for hard failures: enqueue on <see cref="AssertionScope"/> or throw <see cref="OmniAssertionException"/>.</summary>
internal static class VerificationFlow
{
    /// <param name="message">Full failure text.</param>
    /// <param name="subjectExpression">Stored on <see cref="OmniAssertionException.Capture"/> as <see cref="AssertionCapture.SourceExpression"/>.</param>
    public static void Fail(string message, string subjectExpression) =>
        Fail(new OmniAssertionException(message, new AssertionCapture(subjectExpression, null)));

    public static void Fail(OmniAssertionException ex)
    {
        var ctx = AssertionScope.Current;
        if (ctx is not null)
        {
            ctx.Failures.Add(ex);
            return;
        }

        throw ex;
    }
}
