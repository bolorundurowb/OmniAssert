namespace OmniAssert.Tests;

#pragma warning disable OA001 // Legacy Assert entry point under test
#pragma warning disable OA002 // Legacy Verify() under test
#pragma warning disable OA003 // Legacy To*/NotTo* under test

/// <summary>
/// Ensures obsolete v1 entry points and grammar still behave like their v2 replacements until removal in v3.
/// </summary>
public class LegacyApiCompatibilityTests
{
    [Fact]
    public void Legacy_Verify_ToBeTrue_matches_Must_BeTrue_success_path()
    {
#pragma warning disable CS0618
        (true).Verify().ToBeTrue();
#pragma warning restore CS0618
        (true).Must().BeTrue();
    }

    [Fact]
    public void Legacy_Verify_ToBeTrue_matches_Must_BeTrue_failure_path()
    {
#pragma warning disable CS0618
        var legacy = Xunit.Assert.Throws<OmniAssertionException>(() => (false).Verify().ToBeTrue());
#pragma warning restore CS0618
        var modern = Xunit.Assert.Throws<OmniAssertionException>(() => (false).Must().BeTrue());
        Xunit.Assert.Equal(legacy.Message, modern.Message);
    }

    [Fact]
    public void Legacy_Verify_ToBe_matches_Must_Be()
    {
#pragma warning disable CS0618
        (42).Verify().ToBe(42);
#pragma warning restore CS0618
        (42).Must().Be(42);
    }

    [Fact]
    public void Legacy_Verify_NotToBeNull_matches_Must_NotBeNull()
    {
        string? value = "present";
#pragma warning disable CS0618
        value.Verify().NotToBeNull();
#pragma warning restore CS0618
        value.Must().NotBeNull();
    }

    [Fact]
    public void Legacy_static_VerifyExpression_delegates_like_Ensure()
    {
#pragma warning disable CS0618
        OmniAssert.Assert.VerifyExpression(true);
#pragma warning restore CS0618
        Ensure.VerifyExpression(true);
    }

    [Fact]
    public void Expect_throws_delegates_to_Ensure_throws()
    {
        var action = (Action)(() => throw new InvalidOperationException("boom"));
        var fromExpect = Expect.Throws<InvalidOperationException>(action);
        var fromEnsure = Ensure.Throws<InvalidOperationException>(action);
        Xunit.Assert.Equal(typeof(InvalidOperationException), fromExpect.Exception.GetType());
        Xunit.Assert.Equal(typeof(InvalidOperationException), fromEnsure.Exception.GetType());
    }
}
#pragma warning restore OA003
#pragma warning restore OA002
#pragma warning restore OA001
