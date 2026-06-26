namespace OmniAssert.Extensions.Internal;

internal static class AssertionContextExtensions
{
    internal static (T Subject, string Expression) Unwrap<T>(this IAssertionContext<T> context)
        => (context.Subject, context.Expression);
}
