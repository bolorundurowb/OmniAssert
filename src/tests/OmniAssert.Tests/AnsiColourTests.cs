namespace OmniAssert.Tests;

public class AnsiColourTests
{
    [Fact]
    public void Wrap_WhenTextEmpty_ReturnsEmptyWithoutCodes()
    {
        Xunit.Assert.Equal(string.Empty, AnsiColour.Wrap(AnsiColour.Green, string.Empty));
    }

    [Fact]
    public void Expected_WrapsTextAccordingToUseColourSetting()
    {
        var result = AnsiColour.Expected("expected");
        if (AnsiColour.UseColour)
        {
            Xunit.Assert.StartsWith(AnsiColour.Green, result);
            Xunit.Assert.EndsWith(AnsiColour.Reset, result);
            Xunit.Assert.Contains("expected", result);
        }
        else
        {
            Xunit.Assert.Equal("expected", result);
        }
    }

    [Fact]
    public void Actual_WrapsTextAccordingToUseColourSetting()
    {
        var result = AnsiColour.Actual("actual");
        if (AnsiColour.UseColour)
        {
            Xunit.Assert.StartsWith(AnsiColour.Red, result);
            Xunit.Assert.EndsWith(AnsiColour.Reset, result);
            Xunit.Assert.Contains("actual", result);
        }
        else
        {
            Xunit.Assert.Equal("actual", result);
        }
    }

    [Fact]
    public void UseColour_OnWindowsTestHost_IsTrueUnlessNoColorSet()
    {
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NO_COLOR")))
        {
            Xunit.Assert.False(AnsiColour.UseColour);
            return;
        }

        if (Environment.GetEnvironmentVariable("TERM") == "dumb")
        {
            Xunit.Assert.False(AnsiColour.UseColour);
            return;
        }

        if (OperatingSystem.IsWindows())
            Xunit.Assert.True(AnsiColour.UseColour);
    }
}
