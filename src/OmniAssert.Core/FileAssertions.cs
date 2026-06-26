using System.Runtime.CompilerServices;

namespace OmniAssert;

/// <summary>Assertions for an existing file subject.</summary>
public readonly struct FileAssertions
{
    private readonly string _path;
    private readonly string _expression;

    internal FileAssertions(string path, string expression)
    {
        _path = path;
        _expression = expression;
    }

    /// <summary>Verifies that the file content matches <paramref name="text"/> exactly.</summary>
    /// <param name="text">The expected file content.</param>
    /// <param name="textExpression">The expression for the expected content (automatically captured).</param>
    public void HaveContent(string text, [CallerArgumentExpression(nameof(text))] string? textExpression = null)
    {
        if (!File.Exists(_path))
        {
            VerificationFlow.Fail(
                $"Verification failed: expected file {_expression} ({StringFormatter.Quote(_path)}) to exist, but it does not.",
                _expression);
            return;
        }

        var actual = File.ReadAllText(_path);
        if (string.Equals(actual, text, StringComparison.Ordinal))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected file {_expression} ({StringFormatter.Quote(_path)}) to have content {textExpression ?? "text"} ({StringFormatter.Quote(text)}), but was {StringFormatter.Quote(actual)}.",
            _expression);
    }

    /// <summary>Verifies that the file has no content.</summary>
    public void BeEmpty()
    {
        if (!File.Exists(_path))
        {
            VerificationFlow.Fail(
                $"Verification failed: expected file {_expression} ({StringFormatter.Quote(_path)}) to exist, but it does not.",
                _expression);
            return;
        }

        var info = new FileInfo(_path);
        if (info.Length == 0)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected file {_expression} ({StringFormatter.Quote(_path)}) to be empty, but had {info.Length} bytes.",
            _expression);
    }

    /// <summary>Verifies that the file has content (non-zero byte length).</summary>
    public void NotBeEmpty()
    {
        if (!File.Exists(_path))
        {
            VerificationFlow.Fail(
                $"Verification failed: expected file {_expression} ({StringFormatter.Quote(_path)}) to exist, but it does not.",
                _expression);
            return;
        }

        var info = new FileInfo(_path);
        if (info.Length > 0)
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected file {_expression} ({StringFormatter.Quote(_path)}) not to be empty, but had 0 bytes.",
            _expression);
    }

    /// <summary>Verifies that the file path ends with <paramref name="extension"/> (case-insensitive; a leading dot is optional).</summary>
    /// <param name="extension">The expected extension, with or without a leading dot (for example <c>".json"</c> or <c>"json"</c>).</param>
    /// <param name="extensionExpression">The expression for the extension (automatically captured).</param>
    public void HaveExtension(string extension, [CallerArgumentExpression(nameof(extension))] string? extensionExpression = null)
    {
        var normalized = extension.StartsWith('.') ? extension : "." + extension;
        var actualExtension = Path.GetExtension(_path);
        if (string.Equals(actualExtension, normalized, StringComparison.OrdinalIgnoreCase))
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected file {_expression} ({StringFormatter.Quote(_path)}) to have extension {extensionExpression ?? "extension"} ({StringFormatter.Quote(normalized)}), but had {StringFormatter.Quote(actualExtension)}.",
            _expression);
    }
}
