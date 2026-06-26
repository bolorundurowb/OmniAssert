using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using OmniAssert.Extensions.Internal;

namespace OmniAssert.Extensions.Web;

/// <summary>Web, network, and communications assertions for <see cref="StringAssertions"/> subjects.</summary>
public static partial class StringWebExtensions
{
    /// <summary>Verifies that the string is a valid email address (practical RFC 5322 subset).</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeEmailAddress(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (IsValidEmail(actual))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid email address", actual), expression);
    }

    /// <summary>Verifies that the string is a well-formed absolute HTTP or HTTPS URL.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeUrl(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (IsValidAbsoluteUrl(actual))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid absolute URL", actual), expression);
    }

    /// <summary>Alias for <see cref="BeUrl"/>.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeAbsoluteUrl(this StringAssertions assertions) => BeUrl(assertions);

    /// <summary>Verifies that the string is a relative URL path starting with <c>/</c>.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeRelativeUrl(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (IsValidRelativeUrl(actual))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid relative URL", actual), expression);
    }

    /// <summary>Verifies that the string is a URL-friendly slug (lowercase, hyphen-separated).</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeSlug(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (!string.IsNullOrEmpty(actual) && SlugPattern().IsMatch(actual))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid slug", actual), expression);
    }

    /// <summary>Verifies that the string is well-formed JSON.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeJson(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (IsValidJson(actual))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be valid JSON", actual), expression);
    }

    /// <summary>Verifies that the string is well-formed XML.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeXml(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (IsValidXml(actual))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be valid XML", actual), expression);
    }

    /// <summary>Verifies that the string has basic HTML document structure.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeHtml(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (IsValidHtml(actual))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be valid HTML", actual), expression);
    }

    /// <summary>Verifies that the string is a valid IPv4 or IPv6 address.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeIpAddress(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (!string.IsNullOrEmpty(actual) && IPAddress.TryParse(actual, out _))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid IP address", actual), expression);
    }

    /// <summary>Alias for <see cref="BeIpAddress"/>.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeValidIpAddress(this StringAssertions assertions) => BeIpAddress(assertions);

    /// <summary>Verifies that the string is a valid IPv4 address.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeIpv4(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (!string.IsNullOrEmpty(actual) && IPAddress.TryParse(actual, out var addr) && addr.AddressFamily == AddressFamily.InterNetwork)
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid IPv4 address", actual), expression);
    }

    /// <summary>Verifies that the string is a valid IPv6 address.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeIpv6(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (!string.IsNullOrEmpty(actual) && IPAddress.TryParse(actual, out var addr) && addr.AddressFamily == AddressFamily.InterNetworkV6)
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid IPv6 address", actual), expression);
    }

    /// <summary>Verifies that the string is a valid DNS hostname.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeValidHostname(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (IsValidHostname(actual))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid hostname", actual), expression);
    }

    /// <summary>Verifies that the string is a valid MAC address (colon, hyphen, or contiguous hex formats).</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeValidMacAddress(this StringAssertions assertions)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();

        if (!string.IsNullOrEmpty(actual) && MacAddressPattern().IsMatch(actual))
            return;

        VerificationFlow.Fail(FailureMessages.ExpectedGot(expression, "to be a valid MAC address", actual), expression);
    }

    /// <summary>Alias for <see cref="BeValidMacAddress"/>.</summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    public static void BeMacAddress(this StringAssertions assertions) => BeValidMacAddress(assertions);

    /// <summary>
    /// Verifies that the URL or hostname is reachable via HTTP (HEAD, then GET fallback).
    /// Skipped when <c>OMNIASSERT_SKIP_NETWORK=1</c> is set.
    /// </summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    /// <param name="timeout">Optional request timeout (default 5 seconds).</param>
    public static void BeReachable(this StringAssertions assertions, TimeSpan? timeout = null)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();
        var result = NetworkReachability.Check(actual, timeout);

        if (result.WasSkipped || result.Success)
            return;

        VerificationFlow.Fail(
            FailureMessages.ExpectedGot(expression, $"to be reachable ({result.FailureDetail})", actual),
            expression);
    }

    /// <summary>
    /// Asynchronously verifies that the URL or hostname is reachable via HTTP.
    /// Skipped when <c>OMNIASSERT_SKIP_NETWORK=1</c> is set.
    /// </summary>
    /// <param name="assertions">The string assertions chain from <see cref="Ensure.Must(string?, string?)"/>.</param>
    /// <param name="timeout">Optional request timeout (default 5 seconds).</param>
    public static async Task BeReachableAsync(this StringAssertions assertions, TimeSpan? timeout = null)
    {
        var (actual, expression) = ((IAssertionContext<string?>)assertions).Unwrap();
        var result = await NetworkReachability.CheckAsync(actual, timeout);

        if (result.WasSkipped || result.Success)
            return;

        VerificationFlow.Fail(
            FailureMessages.ExpectedGot(expression, $"to be reachable ({result.FailureDetail})", actual),
            expression);
    }

    private static bool IsValidEmail(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 254 || value.Contains(' '))
            return false;

        try
        {
            var addr = new MailAddress(value);
            if (string.IsNullOrEmpty(addr.Address))
                return false;

            var parts = value.Split('@');
            if (parts.Length != 2 || string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1]))
                return false;

            return parts[1].Contains('.');
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidAbsoluteUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
            return false;

        return uri.Scheme is "http" or "https" && !string.IsNullOrEmpty(uri.Host);
    }

    private static bool IsValidRelativeUrl(string? value)
    {
        if (string.IsNullOrEmpty(value) || !value.StartsWith('/'))
            return false;

        if (value.Contains("://") || RightOfColonIsScheme(value.AsSpan()))
            return false;

        return true;
    }

    private static bool RightOfColonIsScheme(ReadOnlySpan<char> value)
    {
        for (var i = 0; i < value.Length - 1; i++)
        {
            if (value[i] == ':')
            {
                var rest = value[(i + 1)..];
                return rest.StartsWith("//");
            }
        }
        return false;
    }

    private static bool IsValidJson(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        try
        {
            using var doc = JsonDocument.Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidXml(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        try
        {
            XDocument.Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidHtml(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (HtmlDocumentPattern().IsMatch(value))
            return true;

        if (HtmlTagOpenPattern().IsMatch(value) && HtmlClosingTagPattern().IsMatch(value))
            return true;

        return false;
    }

    private static bool IsValidHostname(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        if (value.EndsWith('.'))
            value = value[..^1];

        if (value.Length > 253)
            return false;

        var labels = value.Split('.');
        foreach (var label in labels)
        {
            if (label.Length < 1 || label.Length > 63)
                return false;
            if (!HostnameLabelPattern().IsMatch(label))
                return false;
        }

        return true;
    }

    [GeneratedRegex(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")]
    private static partial Regex SlugPattern();

    [GeneratedRegex(@"^([0-9A-Fa-f]{2}[:-]){5}[0-9A-Fa-f]{2}$|^[0-9A-Fa-f]{12}$")]
    private static partial Regex MacAddressPattern();

    [GeneratedRegex(@"<[a-zA-Z][\w-]*(\s|>|/>)")]
    private static partial Regex HtmlTagOpenPattern();

    [GeneratedRegex(@"</[a-zA-Z][\w-]*>")]
    private static partial Regex HtmlClosingTagPattern();

    [GeneratedRegex(@"<html[\s>].*?</html>|<body[\s>].*?</body>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex HtmlDocumentPattern();

    [GeneratedRegex(@"^[a-zA-Z0-9]([a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?$")]
    private static partial Regex HostnameLabelPattern();
}
