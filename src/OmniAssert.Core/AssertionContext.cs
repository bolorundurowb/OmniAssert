namespace OmniAssert;

internal sealed class AssertionContext
{
    public List<OmniAssertionException> Failures { get; } = new();
}
