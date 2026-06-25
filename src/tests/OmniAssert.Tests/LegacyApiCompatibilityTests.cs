namespace OmniAssert.Tests;

#pragma warning disable OA001 // Legacy Assert entry point under test
#pragma warning disable OA002 // Legacy Verify() under test
#pragma warning disable OA003 // Legacy To*/NotTo* under test
#pragma warning disable OA004 // Legacy VerifyExpression() under test

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

    [Fact]
    public void Legacy_Verify_float_returns_NumericAssertions()
    {
#pragma warning disable CS0618
        (1.5f).Verify().ToBe(1.5f);
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_long_returns_NumericAssertions()
    {
#pragma warning disable CS0618
        (100L).Verify().ToBe(100L);
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_decimal_returns_NumericAssertions()
    {
#pragma warning disable CS0618
        (9.99m).Verify().ToBe(9.99m);
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_short_returns_NumericAssertions()
    {
#pragma warning disable CS0618
        ((short)7).Verify().ToBe((short)7);
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_uint_returns_NumericAssertions()
    {
#pragma warning disable CS0618
        (42u).Verify().ToBe(42u);
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_ulong_returns_NumericAssertions()
    {
#pragma warning disable CS0618
        (42ul).Verify().ToBe(42ul);
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_ushort_returns_NumericAssertions()
    {
#pragma warning disable CS0618
        ((ushort)5).Verify().ToBe((ushort)5);
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_sbyte_returns_NumericAssertions()
    {
#pragma warning disable CS0618
        ((sbyte)-3).Verify().ToBe((sbyte)-3);
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_BigInteger_returns_NumericAssertions()
    {
#pragma warning disable CS0618
        (new System.Numerics.BigInteger(999)).Verify().ToBe(new System.Numerics.BigInteger(999));
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_VerifyExpression_with_condition_false_throws()
    {
#pragma warning disable CS0618
        Xunit.Assert.Throws<OmniAssertionException>(() => OmniAssert.Assert.VerifyExpression(false));
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_float_failure_throws()
    {
#pragma warning disable CS0618
        Xunit.Assert.Throws<OmniAssertionException>(() => (1.5f).Verify().ToBe(2.5f));
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_long_failure_throws()
    {
#pragma warning disable CS0618
        Xunit.Assert.Throws<OmniAssertionException>(() => (1L).Verify().ToBe(2L));
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_decimal_failure_throws()
    {
#pragma warning disable CS0618
        Xunit.Assert.Throws<OmniAssertionException>(() => (1.0m).Verify().ToBe(2.0m));
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_short_failure_throws()
    {
#pragma warning disable CS0618
        Xunit.Assert.Throws<OmniAssertionException>(() => ((short)1).Verify().ToBe((short)2));
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_uint_failure_throws()
    {
#pragma warning disable CS0618
        Xunit.Assert.Throws<OmniAssertionException>(() => (1u).Verify().ToBe(2u));
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_ulong_failure_throws()
    {
#pragma warning disable CS0618
        Xunit.Assert.Throws<OmniAssertionException>(() => (1ul).Verify().ToBe(2ul));
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_ushort_failure_throws()
    {
#pragma warning disable CS0618
        Xunit.Assert.Throws<OmniAssertionException>(() => ((ushort)1).Verify().ToBe((ushort)2));
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_sbyte_failure_throws()
    {
#pragma warning disable CS0618
        Xunit.Assert.Throws<OmniAssertionException>(() => ((sbyte)1).Verify().ToBe((sbyte)2));
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_BigInteger_failure_throws()
    {
#pragma warning disable CS0618
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            (new System.Numerics.BigInteger(1)).Verify().ToBe(new System.Numerics.BigInteger(2)));
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_DateTime_returns_DateTimeAssertions()
    {
#pragma warning disable CS0618
        var dt = new System.DateTime(2024, 1, 1);
        (dt).Verify().ToBeAfter(new System.DateTime(2023, 1, 1));
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_DateTimeOffset_returns_DateTimeOffsetAssertions()
    {
#pragma warning disable CS0618
        var dto = new System.DateTimeOffset(2024, 1, 1, 0, 0, 0, System.TimeSpan.Zero);
        (dto).Verify().ToBeAfter(new System.DateTimeOffset(2023, 1, 1, 0, 0, 0, System.TimeSpan.Zero));
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_TimeSpan_returns_TimeSpanAssertions()
    {
#pragma warning disable CS0618
        System.TimeSpan.FromSeconds(5).Verify().ToBeGreaterThan(System.TimeSpan.FromSeconds(1));
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_Guid_returns_GuidAssertions()
    {
#pragma warning disable CS0618
        var g = System.Guid.NewGuid();
        (g).Verify().ToBe(g);
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Verify_Uri_returns_UriAssertions()
    {
#pragma warning disable CS0618
        (new System.Uri("https://example.com")).Verify().Be(new System.Uri("https://example.com"));
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Succeed_delegates_to_Ensure()
    {
#pragma warning disable CS0618
        OmniAssert.Assert.Succeed();
#pragma warning restore CS0618
    }

    [Fact]
    public void Legacy_Fail_delegates_to_Ensure()
    {
#pragma warning disable CS0618
        Xunit.Assert.Throws<OmniAssertionException>(() => OmniAssert.Assert.Fail("boom"));
#pragma warning restore CS0618
    }
}
#pragma warning restore OA004
#pragma warning restore OA003
#pragma warning restore OA002
#pragma warning restore OA001
