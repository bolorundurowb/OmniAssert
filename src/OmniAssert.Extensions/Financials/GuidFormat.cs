namespace OmniAssert.Extensions.Financials;

/// <summary>GUID string layout formats matching <see cref="Guid.ToString(string?)"/> specifiers.</summary>
public enum GuidFormat
{
    /// <summary>32 hexadecimal digits with no grouping (for example <c>d50c66b7206e4e3fb3a15f8e3a2b7c8d</c>).</summary>
    N,

    /// <summary>32 hexadecimal digits in groups separated by hyphens (for example <c>d50c66b7-206e-4e3f-b3a1-5f8e3a2b7c8d</c>).</summary>
    D,

    /// <summary>Braced format with hyphens (for example <c>{d50c66b7-206e-4e3f-b3a1-5f8e3a2b7c8d}</c>).</summary>
    B,

    /// <summary>Parentheses format with hyphens (for example <c>(d50c66b7-206e-4e3f-b3a1-5f8e3a2b7c8d)</c>).</summary>
    P,

    /// <summary>Four hexadecimal values enclosed in braces (for example <c>{0xd50c66b7,0x206e,0x4e3f,{0xb3,0xa1,0x5f,0x8e,0x3a,0x2b,0x7c,0x8d}}</c>).</summary>
    X
}
