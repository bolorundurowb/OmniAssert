using System.Collections.Generic;

namespace OmniAssert;

/// <summary>
/// Collects verification failures until disposal, then throws a single exception listing every failure.
/// Nested scopes use a stack: each dispose only aggregates failures recorded while that scope was innermost.
/// </summary>
public sealed class AssertionScope : IDisposable
{
    private static readonly AsyncLocal<Stack<AssertionContext>?> ScopeStack = new();

    private readonly AssertionContext _context;
    private readonly Stack<AssertionContext> _stack;
    private bool _disposed;

    public AssertionScope()
    {
        _stack = ScopeStack.Value ??= new Stack<AssertionContext>();
        _context = new AssertionContext();
        _stack.Push(_context);
    }

    internal static AssertionContext? Current => ScopeStack.Value is { Count: > 0 } s ? s.Peek() : null;

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
