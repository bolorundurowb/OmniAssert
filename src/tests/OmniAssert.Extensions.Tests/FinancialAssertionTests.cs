using OmniAssert.Extensions.Financials;
using OmniAssert.Extensions.Financials.Validators;

namespace OmniAssert.Extensions.Tests;

public class FinancialAssertionTests
{
    [Fact]
    public void BeGuid_ValidGuidString_ShouldSucceed()
    {
        "d50c66b7-206e-4e3f-b3a1-5f8e3a2b7c8d".Must().BeGuid();
    }

    [Fact]
    public void BeGuid_InvalidGuid_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "not-a-guid".Must().BeGuid());
    }

    [Fact]
    public void BeGuid_NFormat_ShouldSucceed()
    {
        "d50c66b7206e4e3fb3a15f8e3a2b7c8d".Must().BeGuid(GuidFormat.N);
    }

    [Fact]
    public void BeGuid_NFormat_WhenBraced_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            "d50c66b7-206e-4e3f-b3a1-5f8e3a2b7c8d".Must().BeGuid(GuidFormat.N));
    }

    [Fact]
    public void BeGuid_Null_ShouldThrow()
    {
        string? s = null;
        Xunit.Assert.Throws<OmniAssertionException>(() => s.Must().BeGuid());
    }

    [Fact]
    public void BeUlid_ValidUlid_ShouldSucceed()
    {
        "01ARZ3NDEKTSV4RRFFQ69G5FAV".Must().BeUlid();
    }

    [Fact]
    public void BeUlid_InvalidChars_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "01ARZ3NDEKTSV4RRFFQ69G5FAI".Must().BeUlid());
    }

    [Fact]
    public void BeUlid_WrongLength_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "01ARZ3NDEK".Must().BeUlid());
    }

    [Fact]
    public void BeCuid_ValidCuid_ShouldSucceed()
    {
        "cjld2cjxh0000qzrmn831i7rn".Must().BeCuid();
    }

    [Fact]
    public void BeCuid_NotStartingWithC_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "xjld2cjxh0000qzrmn831i7rn".Must().BeCuid());
    }

    [Fact]
    public void BeCuid_TooShort_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "cjld2cjxh".Must().BeCuid());
    }

    [Fact]
    public void BeIsbn_ValidIsbn10_ShouldSucceed()
    {
        "0-306-40615-2".Must().BeIsbn();
    }

    [Fact]
    public void BeIsbn_ValidIsbn13_ShouldSucceed()
    {
        "978-0-306-40615-7".Must().BeIsbn();
    }

    [Fact]
    public void BeIsbn_InvalidIsbn_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "978-0-306-40615-0".Must().BeIsbn());
    }

    [Fact]
    public void BeIsbn_Empty_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "".Must().BeIsbn());
    }

    [Fact]
    public void BeImei_ValidImei_ShouldSucceed()
    {
        "490154203237518".Must().BeImei();
    }

    [Fact]
    public void BeImei_InvalidImei_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "490154203237519".Must().BeImei());
    }

    [Fact]
    public void BeImei_TooShort_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "49015420323751".Must().BeImei());
    }

    [Fact]
    public void BeCreditCard_ValidCardNumber_ShouldSucceed()
    {
        "4532015112830366".Must().BeCreditCard();
    }

    [Fact]
    public void BeCreditCard_InvalidLuhn_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "4532015112830367".Must().BeCreditCard());
    }

    [Fact]
    public void BeCreditCard_WithSeparators_ShouldSucceed()
    {
        "4532-0151-1283-0366".Must().BeCreditCard();
    }

    [Fact]
    public void BeCreditCardNumber_Alias_ShouldSucceed()
    {
        "4532015112830366".Must().BeCreditCardNumber();
    }

    [Fact]
    public void BeIban_ValidIban_ShouldSucceed()
    {
        "GB29NWBK60161331926819".Must().BeIban();
    }

    [Fact]
    public void BeIban_InvalidIban_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "GB29NWBK60161331926810".Must().BeIban());
    }

    [Fact]
    public void BeIban_TooShort_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "GB29".Must().BeIban());
    }

    [Fact]
    public void BeIban_AnotherValidIban_ShouldSucceed()
    {
        "DE89370400440532013000".Must().BeIban();
    }

    [Theory]
    [InlineData("01ARZ3NDEKTSV4RRFFQ69G5FAV")]
    public void UlidValidator_ValidUlid_ShouldPass(string ulid)
    {
        Xunit.Assert.True(UlidValidator.IsValid(ulid));
    }

    [Theory]
    [InlineData("")]
    [InlineData("01ARZ3NDEKTSV4RRFFQ69G5FAI")]
    public void UlidValidator_InvalidUlid_ShouldFail(string ulid)
    {
        Xunit.Assert.False(UlidValidator.IsValid(ulid));
    }

    [Theory]
    [InlineData("cjld2cjxh0000qzrmn831i7rn")]
    public void CuidValidator_ValidCuid_ShouldPass(string cuid)
    {
        Xunit.Assert.True(CuidValidator.IsValid(cuid));
    }

    [Theory]
    [InlineData("xjld2cjxh0000qzrmn831i7rn")]
    public void CuidValidator_InvalidCuid_ShouldFail(string cuid)
    {
        Xunit.Assert.False(CuidValidator.IsValid(cuid));
    }
}
