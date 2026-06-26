namespace OmniAssert;

/// <summary>
/// Internal contract that exposes the assertion subject and captured expression to companion packages such as
/// <c>OmniAssert.Extensions</c>.
/// </summary>
/// <typeparam name="T">The type of the value under verification.</typeparam>
internal interface IAssertionContext<out T>
{
    /// <summary>The value under verification.</summary>
    T Subject { get; }

    /// <summary>The source expression captured from the <c>Must()</c> call site.</summary>
    string Expression { get; }
}
