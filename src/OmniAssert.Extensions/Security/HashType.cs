namespace OmniAssert.Extensions.Security;

/// <summary>Cryptographic hash formats accepted by <see cref="StringSecurityExtensions.BeHashedWith"/>.</summary>
public enum HashType
{
    /// <summary>MD5 digest (32 hexadecimal characters).</summary>
    Md5,

    /// <summary>SHA-256 digest (64 hexadecimal characters).</summary>
    Sha256,

    /// <summary>SHA-512 digest (128 hexadecimal characters).</summary>
    Sha512
}
