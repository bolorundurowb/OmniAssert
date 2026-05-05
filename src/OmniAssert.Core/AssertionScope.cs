namespace OmniAssert;

/// <summary>
/// Collects verification failures until disposal, then throws one exception that lists every failure.
/// Nested scopes use a stack: disposing an inner scope only aggregates failures recorded whilst that scope was innermost.
/// </summary>
/// <remarks>Nested scopes follow async call chains via async-local storage.</remarks>
public sealed class AssertionScope : IDisposable
{
    private static readonly AsyncLocal<Stack<AssertionContext>?> ScopeStack = new();

    private readonly AssertionContext _context;
    private readonly Stack<AssertionContext> _stack;
    private bool _disposed;

    /// <summary>Opens a new scope and pushes it onto the ambient stack.</summary>
    public AssertionScope()
    {
        _stack = ScopeStack.Value ??= new Stack<AssertionContext>();
        _context = new AssertionContext();
        _stack.Push(_context);
    }

    internal static AssertionContext? Current => ScopeStack.Value is { Count: > 0 } s ? s.Peek() : null;

    /// <summary>Pops this scope, then throws if any failures were recorded whilst it was innermost.</summary>
    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        if (_stack.Count == 0 || !ReferenceEquals(_stack.Peek(), _context))
            return;

        _stack.Pop();
        if (_stack.Count == 0)
            ScopeStack.Value = null;

        if (_context.Failures.Count == 0)
            return;

        if (_context.Failures.Count == 1)
            throw _context.Failures[0];

        var message = FormatAggregate(_context.Failures);
        throw new AggregateException(message, _context.Failures);
    }

    private static string FormatAggregate(IReadOnlyList<OmniAssertionException> failures)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Multiple verifications failed:");
        for (var i = 0; i < failures.Count; i++)
        {
            sb.Append(i + 1);
            sb.Append(". ");
            sb.AppendLine(failures[i].Message);
        }

        return sb.ToString().TrimEnd();
    }
}
