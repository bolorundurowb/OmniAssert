namespace OmniAssert.Tests;

public class DateSugarAssertionTests
{
    // DateTime BeInPast / BeInFuture
    [Fact]
    public void DateTime_BeInPast_WhenPast_ShouldSucceed() =>
        DateTime.UtcNow.AddDays(-1).Must().BeInPast();

    [Fact]
    public void DateTime_BeInPast_WhenFuture_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => DateTime.UtcNow.AddDays(1).Must().BeInPast());

    [Fact]
    public void DateTime_BeInFuture_WhenFuture_ShouldSucceed() =>
        DateTime.UtcNow.AddDays(1).Must().BeInFuture();

    [Fact]
    public void DateTime_BeInFuture_WhenPast_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => DateTime.UtcNow.AddDays(-1).Must().BeInFuture());

    // DateTime BeSameDayAs
    [Fact]
    public void DateTime_BeSameDayAs_WhenSameDay_ShouldSucceed()
    {
        var a = new DateTime(2026, 6, 26, 9, 0, 0, DateTimeKind.Utc);
        var b = new DateTime(2026, 6, 26, 23, 59, 0, DateTimeKind.Utc);
        a.Must().BeSameDayAs(b);
    }

    [Fact]
    public void DateTime_BeSameDayAs_WhenDifferentDay_ShouldThrow()
    {
        var a = new DateTime(2026, 6, 26, 9, 0, 0, DateTimeKind.Utc);
        var b = new DateTime(2026, 6, 27, 9, 0, 0, DateTimeKind.Utc);
        Xunit.Assert.Throws<OmniAssertionException>(() => a.Must().BeSameDayAs(b));
    }

    // DateTimeOffset
    [Fact]
    public void DateTimeOffset_BeInPast_WhenPast_ShouldSucceed() =>
        DateTimeOffset.UtcNow.AddHours(-1).Must().BeInPast();

    [Fact]
    public void DateTimeOffset_BeInFuture_WhenFuture_ShouldSucceed() =>
        DateTimeOffset.UtcNow.AddHours(1).Must().BeInFuture();

    [Fact]
    public void DateTimeOffset_BeSameDayAs_WhenSameUtcDay_ShouldSucceed()
    {
        var a = new DateTimeOffset(2026, 6, 26, 1, 0, 0, TimeSpan.Zero);
        var b = new DateTimeOffset(2026, 6, 26, 22, 0, 0, TimeSpan.Zero);
        a.Must().BeSameDayAs(b);
    }

    // DateOnly today / yesterday / tomorrow
    [Fact]
    public void DateOnly_BeToday_WhenToday_ShouldSucceed() =>
        DateOnly.FromDateTime(DateTime.Today).Must().BeToday();

    [Fact]
    public void DateOnly_BeToday_WhenNotToday_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            DateOnly.FromDateTime(DateTime.Today).AddDays(2).Must().BeToday());

    [Fact]
    public void DateOnly_BeYesterday_WhenYesterday_ShouldSucceed() =>
        DateOnly.FromDateTime(DateTime.Today).AddDays(-1).Must().BeYesterday();

    [Fact]
    public void DateOnly_BeTomorrow_WhenTomorrow_ShouldSucceed() =>
        DateOnly.FromDateTime(DateTime.Today).AddDays(1).Must().BeTomorrow();

    // DateOnly weekday / weekend
    [Fact]
    public void DateOnly_BeWeekday_WhenWeekday_ShouldSucceed() =>
        new DateOnly(2026, 6, 26).Must().BeWeekday(); // Friday

    [Fact]
    public void DateOnly_BeWeekday_WhenWeekend_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => new DateOnly(2026, 6, 27).Must().BeWeekday()); // Saturday

    [Fact]
    public void DateOnly_BeWeekend_WhenWeekend_ShouldSucceed() =>
        new DateOnly(2026, 6, 28).Must().BeWeekend(); // Sunday

    [Fact]
    public void DateOnly_BeWeekend_WhenWeekday_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => new DateOnly(2026, 6, 26).Must().BeWeekend());

    // DateOnly leap year
    [Fact]
    public void DateOnly_BeLeapYear_WhenLeap_ShouldSucceed() =>
        new DateOnly(2024, 2, 29).Must().BeLeapYear();

    [Fact]
    public void DateOnly_BeLeapYear_WhenNotLeap_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => new DateOnly(2026, 1, 1).Must().BeLeapYear());

    // DateOnly within days
    [Fact]
    public void DateOnly_BeWithinDays_WhenWithin_ShouldSucceed() =>
        new DateOnly(2026, 6, 26).Must().BeWithinDays(3, new DateOnly(2026, 6, 28));

    [Fact]
    public void DateOnly_BeWithinDays_WhenAtBound_ShouldSucceed() =>
        new DateOnly(2026, 6, 26).Must().BeWithinDays(2, new DateOnly(2026, 6, 28));

    [Fact]
    public void DateOnly_BeWithinDays_WhenOutside_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            new DateOnly(2026, 6, 26).Must().BeWithinDays(1, new DateOnly(2026, 6, 30)));

    [Fact]
    public void DateOnly_BeWithinDays_WhenNegativeArg_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            new DateOnly(2026, 6, 26).Must().BeWithinDays(-1, new DateOnly(2026, 6, 26)));
}
