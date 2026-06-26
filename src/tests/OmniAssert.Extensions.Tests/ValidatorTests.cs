using OmniAssert.Extensions.Financials.Validators;
using OmniAssert.Extensions.Regional.Validators;

namespace OmniAssert.Extensions.Tests;

public class ValidatorTests
{
    [Theory]
    [InlineData("79927398713")]
    [InlineData("4111111111111111")]
    public void LuhnValidator_ValidNumbers_ShouldPass(string digits)
    {
        Xunit.Assert.True(LuhnValidator.IsValid(digits));
    }

    [Theory]
    [InlineData("79927398710")]
    [InlineData("4111111111111112")]
    public void LuhnValidator_InvalidNumbers_ShouldFail(string digits)
    {
        Xunit.Assert.False(LuhnValidator.IsValid(digits));
    }

    [Theory]
    [InlineData("GB29NWBK60161331926819")]
    [InlineData("DE89370400440532013000")]
    public void IbanValidator_ValidIbans_ShouldPass(string iban)
    {
        Xunit.Assert.True(IbanValidator.IsValid(iban));
    }

    [Theory]
    [InlineData("GB29NWBK60161331926810")]
    [InlineData("GB29")]
    public void IbanValidator_InvalidIbans_ShouldFail(string iban)
    {
        Xunit.Assert.False(IbanValidator.IsValid(iban));
    }

    [Theory]
    [InlineData("0-306-40615-2")]
    [InlineData("978-0-306-40615-7")]
    public void IsbnValidator_ValidIsbns_ShouldPass(string isbn)
    {
        Xunit.Assert.True(IsbnValidator.IsValid(isbn));
    }

    [Theory]
    [InlineData("978-0-306-40615-0")]
    [InlineData("")]
    public void IsbnValidator_InvalidIsbns_ShouldFail(string isbn)
    {
        Xunit.Assert.False(IsbnValidator.IsValid(isbn));
    }

    [Fact]
    public void NubanValidator_ValidAccountAndBankCode_ShouldPass()
    {
        Xunit.Assert.True(NubanValidator.IsValid("0022728153", "058"));
    }

    [Fact]
    public void NubanValidator_InvalidCheckDigit_ShouldFail()
    {
        Xunit.Assert.False(NubanValidator.IsValid("0022728151", "058"));
    }

    [Fact]
    public void NubanValidator_InvalidBankCodeLength_ShouldFail()
    {
        Xunit.Assert.False(NubanValidator.IsValid("0022728153", "05"));
    }
}
