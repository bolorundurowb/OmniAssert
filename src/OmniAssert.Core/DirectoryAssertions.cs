namespace OmniAssert;

/// <summary>Assertions for an existing directory subject.</summary>
public readonly struct DirectoryAssertions
{
    private readonly string _path;
    private readonly string _expression;

    internal DirectoryAssertions(string path, string expression)
    {
        _path = path;
        _expression = expression;
    }

    /// <summary>Verifies that the directory has no files or subdirectories.</summary>
    public void BeEmpty()
    {
        using var entries = Directory.EnumerateFileSystemEntries(_path).GetEnumerator();
        if (!entries.MoveNext())
            return;

        VerificationFlow.Fail(
            $"Verification failed: expected directory {_expression} ({StringFormatter.Quote(_path)}) to be empty, but found at least one entry.",
            _expression);
    }
}
