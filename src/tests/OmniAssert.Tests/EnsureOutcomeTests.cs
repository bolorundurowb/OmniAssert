namespace OmniAssert.Tests;

public class EnsureOutcomeTests
{
    [Fact]
    public void Succeed_does_not_throw()
    {
        Ensure.Succeed();
    }

    [Fact]
    public void Fail_without_message_throws_with_default_text()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => Ensure.Fail());
        Xunit.Assert.Equal("Verification failed: explicit failure.", ex.Message);
    }

    [Fact]
    public void Fail_with_message_prefixes_when_needed()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => Ensure.Fail("not implemented yet"));
        Xunit.Assert.Equal("Verification failed: not implemented yet", ex.Message);
    }

    [Fact]
    public void Fail_with_prefixed_message_is_used_as_is()
    {
        const string message = "Verification failed: custom wording";
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => Ensure.Fail(message));
        Xunit.Assert.Equal(message, ex.Message);
    }

    [Fact]
    public void Fail_within_AssertionScope_collects_instead_of_throwing_immediately()
    {
        var ex = Xunit.Assert.Throws<OmniAssertionException>(() =>
        {
            using var scope = new AssertionScope();
            Ensure.Fail("deferred");
        });

        Xunit.Assert.Equal("Verification failed: deferred", ex.Message);
    }

    [Fact]
    public void Legacy_Assert_Succeed_and_Fail_delegate_to_Ensure()
    {
#pragma warning disable CS0618
#pragma warning disable OA001
        Assert.Succeed();

        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => Assert.Fail("legacy path"));
#pragma warning restore OA001
#pragma warning restore CS0618

        Xunit.Assert.Equal("Verification failed: legacy path", ex.Message);
    }
}
