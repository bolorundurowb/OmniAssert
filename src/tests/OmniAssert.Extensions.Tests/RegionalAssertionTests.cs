using OmniAssert.Extensions.Regional;

namespace OmniAssert.Extensions.Tests;

public class RegionalAssertionTests
{
    [Fact]
    public void BeNigerianPhoneNumber_ValidWithPlus234_ShouldSucceed()
    {
        "+2348012345678".Must().BeNigerianPhoneNumber();
    }

    [Fact]
    public void BeNigerianPhoneNumber_ValidWithLeadingZero_ShouldSucceed()
    {
        "08012345678".Must().BeNigerianPhoneNumber();
    }

    [Fact]
    public void BeNigerianPhoneNumber_InvalidPrefix_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "01012345678".Must().BeNigerianPhoneNumber());
    }

    [Fact]
    public void BeNigerianPhoneNumber_TooShort_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "0801234567".Must().BeNigerianPhoneNumber());
    }

    [Fact]
    public void BeNigerianBankAccountNumber_ValidFormat_ShouldSucceed()
    {
        "0123456789".Must().BeNigerianBankAccountNumber();
    }

    [Fact]
    public void BeNigerianBankAccountNumber_TooShort_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "012345678".Must().BeNigerianBankAccountNumber());
    }

    [Fact]
    public void BeNigerianBankAccountNumber_WithLetters_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "012345678a".Must().BeNigerianBankAccountNumber());
    }

    [Fact]
    public void BeNigerianBankAccountNumber_WithValidNuban_ShouldSucceed()
    {
        "0022728153".Must().BeNigerianBankAccountNumber("058");
    }

    [Fact]
    public void BeNigerianBankAccountNumber_WithInvalidNubanCheck_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            "0022728151".Must().BeNigerianBankAccountNumber("058"));
    }

    [Fact]
    public void BeNigerianBankAccountNumber_WithInvalidBankCode_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            "0022728153".Must().BeNigerianBankAccountNumber("05"));
    }

    [Fact]
    public void BeNigerianBvn_ValidBvn_ShouldSucceed()
    {
        "12345678901".Must().BeNigerianBvn();
    }

    [Fact]
    public void BeNigerianBvn_AllSameDigits_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "11111111111".Must().BeNigerianBvn());
    }

    [Fact]
    public void BeNigerianBvn_WrongLength_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "1234567890".Must().BeNigerianBvn());
    }

    [Fact]
    public void BeNigerianNin_ValidNin_ShouldSucceed()
    {
        "12345678901".Must().BeNigerianNin();
    }

    [Fact]
    public void BeNigerianNin_AllSameDigits_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "22222222222".Must().BeNigerianNin());
    }

    [Fact]
    public void BeNigerianPostalCode_Valid_ShouldSucceed()
    {
        "100001".Must().BeNigerianPostalCode();
    }

    [Fact]
    public void BeNigerianPostalCode_TooShort_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "10001".Must().BeNigerianPostalCode());
    }

    [Fact]
    public void BePhoneNumber_ValidE164_ShouldSucceed()
    {
        "+14155552671".Must().BePhoneNumber();
    }

    [Fact]
    public void BePhoneNumber_WithoutPlus_ShouldSucceed()
    {
        "14155552671".Must().BePhoneNumber();
    }

    [Fact]
    public void BePhoneNumber_TooShort_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "+123".Must().BePhoneNumber());
    }

    [Fact]
    public void BePostalCode_US_Valid_ShouldSucceed()
    {
        "90210".Must().BePostalCode(CountryCode.US);
    }

    [Fact]
    public void BePostalCode_US_WithExtension_ShouldSucceed()
    {
        "90210-1234".Must().BePostalCode(CountryCode.US);
    }

    [Fact]
    public void BePostalCode_US_Invalid_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "9021".Must().BePostalCode(CountryCode.US));
    }

    [Fact]
    public void BePostalCode_Generic_Valid_ShouldSucceed()
    {
        "AB12CD".Must().BePostalCode(CountryCode.Generic);
    }

    [Fact]
    public void BeSocialSecurityNumber_ValidSSN_ShouldSucceed()
    {
        "123-45-6789".Must().BeSocialSecurityNumber();
    }

    [Fact]
    public void BeSocialSecurityNumber_WithoutDashes_ShouldSucceed()
    {
        "123456789".Must().BeSocialSecurityNumber();
    }

    [Fact]
    public void BeSocialSecurityNumber_InvalidArea000_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "000-45-6789".Must().BeSocialSecurityNumber());
    }

    [Fact]
    public void BeSocialSecurityNumber_InvalidArea666_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "666-45-6789".Must().BeSocialSecurityNumber());
    }

    [Fact]
    public void BeSocialSecurityNumber_InvalidArea9xx_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "900-45-6789".Must().BeSocialSecurityNumber());
    }

    [Fact]
    public void BeAdult_AdultAge_ShouldSucceed()
    {
        var dob = DateTime.UtcNow.AddYears(-25);
        dob.Must().BeAdult();
    }

    [Fact]
    public void BeAdult_Underage_ShouldThrow()
    {
        var dob = DateTime.UtcNow.AddYears(-10);
        Xunit.Assert.Throws<OmniAssertionException>(() => dob.Must().BeAdult());
    }

    [Fact]
    public void BeAdult_DateOnly_AdultAge_ShouldSucceed()
    {
        var dob = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20));
        dob.Must().BeAdult();
    }

    [Fact]
    public void BeAdult_DateOnly_Underage_ShouldThrow()
    {
        var dob = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-10));
        Xunit.Assert.Throws<OmniAssertionException>(() => dob.Must().BeAdult());
    }

    [Fact]
    public void BeBusinessHours_WeekdayDuringHours_ShouldSucceed()
    {
        var weekday = new DateTime(2026, 6, 23, 10, 0, 0);
        weekday.Must().BeBusinessHours();
    }

    [Fact]
    public void BeBusinessHours_Weekend_ShouldThrow()
    {
        var weekend = new DateTime(2026, 6, 27, 10, 0, 0);
        Xunit.Assert.Throws<OmniAssertionException>(() => weekend.Must().BeBusinessHours());
    }

    [Fact]
    public void BeBusinessHours_AfterHours_ShouldThrow()
    {
        var afterHours = new DateTime(2026, 6, 23, 18, 0, 0);
        Xunit.Assert.Throws<OmniAssertionException>(() => afterHours.Must().BeBusinessHours());
    }

    [Fact]
    public void BeBusinessHours_TimeOnly_DuringHours_ShouldSucceed()
    {
        var time = new TimeOnly(10, 30);
        time.Must().BeBusinessHours();
    }

    [Fact]
    public void BeBirthday_DateTime_Today_ShouldSucceed()
    {
        DateTime.Today.Must().BeBirthday();
    }

    [Fact]
    public void BeBirthday_DateTime_NotToday_ShouldThrow()
    {
        var notToday = DateTime.Today.AddDays(-1);
        Xunit.Assert.Throws<OmniAssertionException>(() => notToday.Must().BeBirthday());
    }

    [Fact]
    public void BeBirthday_DateOnly_Today_ShouldSucceed()
    {
        DateOnly.FromDateTime(DateTime.Today).Must().BeBirthday();
    }

    [Fact]
    public void BeBirthday_DateOnly_NotToday_ShouldThrow()
    {
        var notToday = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        Xunit.Assert.Throws<OmniAssertionException>(() => notToday.Must().BeBirthday());
    }
}
