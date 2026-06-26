namespace OmniAssert.Tests;

public class StringFormatAssertionTests
{
    // BeTrimmed
    [Fact]
    public void BeTrimmed_WhenNoSurroundingWhitespace_ShouldSucceed() => "a b c".Must().BeTrimmed();

    [Fact]
    public void BeTrimmed_WhenEmpty_ShouldSucceed() => string.Empty.Must().BeTrimmed();

    [Fact]
    public void BeTrimmed_WhenLeadingWhitespace_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => " abc".Must().BeTrimmed());

    [Fact]
    public void BeTrimmed_WhenTrailingWhitespace_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => "abc ".Must().BeTrimmed());

    [Fact]
    public void BeTrimmed_WhenNull_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => ((string?)null).Must().BeTrimmed());

    // HaveLengthBetween
    [Fact]
    public void HaveLengthBetween_WhenWithinRange_ShouldSucceed() => "abc".Must().HaveLengthBetween(1, 5);

    [Fact]
    public void HaveLengthBetween_WhenAtBounds_ShouldSucceed()
    {
        "abc".Must().HaveLengthBetween(3, 3);
        "ab".Must().HaveLengthBetween(2, 4);
    }

    [Fact]
    public void HaveLengthBetween_WhenTooShort_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => "a".Must().HaveLengthBetween(2, 5));

    [Fact]
    public void HaveLengthBetween_WhenTooLong_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => "abcdef".Must().HaveLengthBetween(1, 5));

    // BeAlphanumeric
    [Fact]
    public void BeAlphanumeric_WhenLettersAndDigits_ShouldSucceed() => "abc123".Must().BeAlphanumeric();

    [Fact]
    public void BeAlphanumeric_WhenContainsSymbol_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => "abc-123".Must().BeAlphanumeric());

    [Fact]
    public void BeAlphanumeric_WhenEmpty_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => string.Empty.Must().BeAlphanumeric());

    // BeLowerCase / BeUpperCase
    [Fact]
    public void BeLowerCase_WhenAllLower_ShouldSucceed() => "abc123".Must().BeLowerCase();

    [Fact]
    public void BeLowerCase_WhenContainsUpper_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => "abC".Must().BeLowerCase());

    [Fact]
    public void BeUpperCase_WhenAllUpper_ShouldSucceed() => "ABC123".Must().BeUpperCase();

    [Fact]
    public void BeUpperCase_WhenContainsLower_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => "ABc".Must().BeUpperCase());

    // BeNumeric
    [Fact]
    public void BeNumeric_WhenAllDigits_ShouldSucceed() => "00123".Must().BeNumeric();

    [Fact]
    public void BeNumeric_WhenContainsLetter_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => "12a".Must().BeNumeric());

    [Fact]
    public void BeNumeric_WhenNegativeSign_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => "-12".Must().BeNumeric());

    // BeBase64
    [Fact]
    public void BeBase64_WhenValid_ShouldSucceed() => "aGVsbG8=".Must().BeBase64();

    [Fact]
    public void BeBase64_WhenInvalidChars_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => "not base64!".Must().BeBase64());

    [Fact]
    public void BeBase64_WhenBadLength_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => "abcde".Must().BeBase64());

    // BeHexString
    [Fact]
    public void BeHexString_WhenValid_ShouldSucceed() => "DEADbeef01".Must().BeHexString();

    [Fact]
    public void BeHexString_WhenInvalid_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => "xyz".Must().BeHexString());

    [Fact]
    public void BeHexString_WhenEmpty_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => string.Empty.Must().BeHexString());

    // BeHexColor
    [Theory]
    [InlineData("#fff")]
    [InlineData("#FFFF")]
    [InlineData("#ff8800")]
    [InlineData("#FF8800AA")]
    public void BeHexColor_WhenValid_ShouldSucceed(string value) => value.Must().BeHexColor();

    [Theory]
    [InlineData("fff")]
    [InlineData("#ff")]
    [InlineData("#gggggg")]
    [InlineData("#fffff")]
    public void BeHexColor_WhenInvalid_ShouldThrow(string value) =>
        Xunit.Assert.Throws<OmniAssertionException>(() => value.Must().BeHexColor());

    // BeIso8601
    [Theory]
    [InlineData("2026-06-26")]
    [InlineData("2026-06-26T12:34:56Z")]
    [InlineData("2026-06-26T12:34:56.789+02:00")]
    public void BeIso8601_WhenValid_ShouldSucceed(string value) => value.Must().BeIso8601();

    [Fact]
    public void BeIso8601_WhenInvalid_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => "not-a-date".Must().BeIso8601());

    // BeDateString
    [Fact]
    public void BeDateString_WhenMatchesFormat_ShouldSucceed() => "2026-06-26".Must().BeDateString("yyyy-MM-dd");

    [Fact]
    public void BeDateString_WhenWrongFormat_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => "26/06/2026".Must().BeDateString("yyyy-MM-dd"));

    // BeAbsolutePath / BeRelativePath
    [Fact]
    public void BeAbsolutePath_WhenRooted_ShouldSucceed() =>
        Path.Combine(Path.GetTempPath(), "x.txt").Must().BeAbsolutePath();

    [Fact]
    public void BeAbsolutePath_WhenRelative_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() => "sub/dir/file.txt".Must().BeAbsolutePath());

    [Fact]
    public void BeRelativePath_WhenRelative_ShouldSucceed() => "sub/dir/file.txt".Must().BeRelativePath();

    [Fact]
    public void BeRelativePath_WhenRooted_ShouldThrow() =>
        Xunit.Assert.Throws<OmniAssertionException>(() =>
            Path.Combine(Path.GetTempPath(), "x.txt").Must().BeRelativePath());
}
