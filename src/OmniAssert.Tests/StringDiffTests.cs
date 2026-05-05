using System.Text.RegularExpressions;
using static OmniAssert.Assert;

namespace OmniAssert.Tests;

public class StringDiffTests
{
    private string StripAnsi(string text) => Regex.Replace(text, @"\u001b\[[0-9;]*m", "");

    [Fact]
    public void Verify_StringToBe_WhenDiffer_ShouldShowPrettyDiff()
    {
        var expected = "The quick brown fox jumps";
        var actual = "The quick brown fax jumps";

        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => Verify(actual).ToBe(expected));
        var message = StripAnsi(ex.Message);

        Xunit.Assert.Contains("Verification failed.", message);
        Xunit.Assert.Contains("Expected: \"The quick brown fox jumps\"", message);
        Xunit.Assert.Contains("Got:      \"The quick brown fax jumps\"", message);
        Xunit.Assert.Contains("^ expected 'o', got 'a' at position 17", message);
        Xunit.Assert.Contains("Lengths: 25 vs 25 chars", message);
    }

    [Fact]
    public void Verify_StringToBe_WithZeroWidthSpace_ShouldBeVisible()
    {
        var expected = "AB";
        var actual = "A\u200BB";

        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => Verify(actual).ToBe(expected));
        var message = StripAnsi(ex.Message);

        Xunit.Assert.Contains("Got:      \"A\\u200BB\"", message);
        Xunit.Assert.Contains("^ expected 'B', got '\\u200B' at position 1", message);
    }

    [Fact]
    public void Verify_StringToBe_WhenOneIsPrefix_ShouldShowEndOfPrefix()
    {
        var expected = "ABC";
        var actual = "AB";

        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => Verify(actual).ToBe(expected));
        var message = StripAnsi(ex.Message);

        Xunit.Assert.Contains("expected 'C', got end of string at position 2", message);
    }

    [Fact]
    public void Verify_StringToBe_WithNewlines_ShouldEscape()
    {
        var expected = "A\nB";
        var actual = "A\rB";

        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => Verify(actual).ToBe(expected));
        var message = StripAnsi(ex.Message);

        Xunit.Assert.Contains("Expected: \"A\\nB\"", message);
        Xunit.Assert.Contains("Got:      \"A\\rB\"", message);
        Xunit.Assert.Contains("expected '\\n', got '\\r' at position 1", message);
    }

    [Fact]
    public void Verify_StringNotToBe_WhenSame_ShouldShowSimpleMessage()
    {
        var expected = "hello";
        var actual = "hello";

        var ex = Xunit.Assert.Throws<OmniAssertionException>(() => Verify(actual).NotToBe(expected));
        var message = StripAnsi(ex.Message);

        Xunit.Assert.Contains("Verification failed.", message);
        Xunit.Assert.Contains("Expected actual not to be \"hello\", but it was.", message);
    }
}
