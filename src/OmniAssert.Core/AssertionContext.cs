namespace OmniAssert;

/// <summary>Failure list for one <see cref="AssertionScope"/> frame on the async-local stack.</summary>
internal sealed class AssertionContext
{
    public List<OmniAssertionException> Failures { get; } = [];
}
