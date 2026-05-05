using System.Text;

namespace OmniAssert.Tests;

public class StringFormatterTests
{
    // ── Quote ────────────────────────────────────────────────────────────────

    [Fact]
    public void Quote_WhenNull_ReturnsNullLiteral()
    {
        Xunit.Assert.Equal("null", StringFormatter.Quote(null));
    }

    [Fact]
    public void Quote_WhenEmpty_ReturnsEmptyQuotedString()
    {
        Xunit.Assert.Equal("\"\"", StringFormatter.Quote(""));
    }

    [Fact]
    public void Quote_WhenSimpleString_ReturnsQuoted()
    {
        Xunit.Assert.Equal("\"hello\"", StringFormatter.Quote("hello"));
    }

    [Fact]
    public void Quote_EscapesDoubleQuote()
    {
        Xunit.Assert.Equal("\"\\\"\"", StringFormatter.Quote("\""));
    }

    [Fact]
    public void Quote_EscapesBackslash()
    {
        Xunit.Assert.Equal("\"\\\\\"", StringFormatter.Quote("\\"));
    }

    [Fact]
    public void Quote_EscapesNewline()
    {
        Xunit.Assert.Equal("\"\\n\"", StringFormatter.Quote("\n"));
    }

    [Fact]
    public void Quote_EscapesCarriageReturn()
    {
        Xunit.Assert.Equal("\"\\r\"", StringFormatter.Quote("\r"));
    }

    [Fact]
    public void Quote_EscapesTab()
    {
        Xunit.Assert.Equal("\"\\t\"", StringFormatter.Quote("\t"));
    }

    [Fact]
    public void Quote_EscapesNullChar()
    {
        Xunit.Assert.Equal("\"\\0\"", StringFormatter.Quote("\0"));
    }

    [Fact]
    public void Quote_EscapesAlert()
    {
        Xunit.Assert.Equal("\"\\a\"", StringFormatter.Quote("\a"));
    }

    [Fact]
    public void Quote_EscapesBackspace()
    {
        Xunit.Assert.Equal("\"\\b\"", StringFormatter.Quote("\b"));
    }

    [Fact]
    public void Quote_EscapesFormFeed()
    {
        Xunit.Assert.Equal("\"\\f\"", StringFormatter.Quote("\f"));
    }

    [Fact]
    public void Quote_EscapesVerticalTab()
    {
        Xunit.Assert.Equal("\"\\v\"", StringFormatter.Quote("\v"));
    }

    [Fact]
    public void Quote_EscapesControlCharacter_AsUnicodeEscape()
    {
        // \u0001 is a control char not handled by a named escape
        Xunit.Assert.Equal("\"\\u0001\"", StringFormatter.Quote("\u0001"));
    }

    [Fact]
    public void Quote_EscapesZeroWidthSpace_AsUnicodeEscape()
    {
        Xunit.Assert.Equal("\"\\u200B\"", StringFormatter.Quote("\u200B"));
    }

    [Fact]
    public void Quote_EscapesZeroWidthNonJoiner_AsUnicodeEscape()
    {
        Xunit.Assert.Equal("\"\\u200C\"", StringFormatter.Quote("\u200C"));
    }

    [Fact]
    public void Quote_EscapesZeroWidthJoiner_AsUnicodeEscape()
    {
        Xunit.Assert.Equal("\"\\u200D\"", StringFormatter.Quote("\u200D"));
    }

    [Fact]
    public void Quote_EscapesByteOrderMark_AsUnicodeEscape()
    {
        Xunit.Assert.Equal("\"\\uFEFF\"", StringFormatter.Quote("\uFEFF"));
    }

    [Fact]
    public void Quote_MixedContent_EscapesOnlySpecialChars()
    {
        Xunit.Assert.Equal("\"ab\\ncd\"", StringFormatter.Quote("ab\ncd"));
    }

    // ── AppendVisible ────────────────────────────────────────────────────────

    [Theory]
    [InlineData('"', "\\\"")]
    [InlineData('\\', "\\\\")]
    [InlineData('\0', "\\0")]
    [InlineData('\a', "\\a")]
    [InlineData('\b', "\\b")]
    [InlineData('\f', "\\f")]
    [InlineData('\n', "\\n")]
    [InlineData('\r', "\\r")]
    [InlineData('\t', "\\t")]
    [InlineData('\v', "\\v")]
    [InlineData('A', "A")]
    [InlineData('z', "z")]
    [InlineData('5', "5")]
    public void AppendVisible_EscapesCharacterCorrectly(char input, string expected)
    {
        var sb = new StringBuilder();
        StringFormatter.AppendVisible(sb, input);
        Xunit.Assert.Equal(expected, sb.ToString());
    }

    [Fact]
    public void AppendVisible_ControlCharacter_AppendsUnicodeEscape()
    {
        var sb = new StringBuilder();
        StringFormatter.AppendVisible(sb, '\u0002');
        Xunit.Assert.Equal("\\u0002", sb.ToString());
    }

    [Fact]
    public void AppendVisible_ZeroWidthChar_AppendsUnicodeEscape()
    {
        var sb = new StringBuilder();
        StringFormatter.AppendVisible(sb, '\u200B');
        Xunit.Assert.Equal("\\u200B", sb.ToString());
    }

    // ── EscapeChar ───────────────────────────────────────────────────────────

    [Theory]
    [InlineData('\'', "\\'")]
    [InlineData('\\', "\\\\")]
    [InlineData('\0', "\\0")]
    [InlineData('\a', "\\a")]
    [InlineData('\b', "\\b")]
    [InlineData('\f', "\\f")]
    [InlineData('\n', "\\n")]
    [InlineData('\r', "\\r")]
    [InlineData('\t', "\\t")]
    [InlineData('\v', "\\v")]
    [InlineData('A', "A")]
    [InlineData('1', "1")]
    public void EscapeChar_ReturnsCorrectEscapeSequence(char input, string expected)
    {
        Xunit.Assert.Equal(expected, StringFormatter.EscapeChar(input));
    }

    [Fact]
    public void EscapeChar_ControlCharacter_ReturnsUnicodeEscape()
    {
        Xunit.Assert.Equal("\\u001F", StringFormatter.EscapeChar('\u001F'));
    }

    [Fact]
    public void EscapeChar_ZeroWidthChar_ReturnsUnicodeEscape()
    {
        Xunit.Assert.Equal("\\uFEFF", StringFormatter.EscapeChar('\uFEFF'));
    }

    // ── IsZeroWidth ──────────────────────────────────────────────────────────

    [Theory]
    [InlineData('\u200B', true)]
    [InlineData('\u200C', true)]
    [InlineData('\u200D', true)]
    [InlineData('\uFEFF', true)]
    [InlineData('A', false)]
    [InlineData(' ', false)]
    [InlineData('\t', false)]
    [InlineData('\n', false)]
    public void IsZeroWidth_ReturnsExpectedResult(char input, bool expected)
    {
        Xunit.Assert.Equal(expected, StringFormatter.IsZeroWidth(input));
    }
}
