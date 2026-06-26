using OmniAssert.Extensions.Security;

namespace OmniAssert.Extensions.Tests;

public class SecurityAssertionTests
{
    [Fact]
    public void BeStrongPassword_StrongPassword_ShouldSucceed()
    {
        "MyP@ssw0rd!".Must().BeStrongPassword();
    }

    [Fact]
    public void BeStrongPassword_NoUppercase_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "myp@ssw0rd!".Must().BeStrongPassword());
    }

    [Fact]
    public void BeStrongPassword_NoDigit_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "MyP@ssword!".Must().BeStrongPassword());
    }

    [Fact]
    public void BeStrongPassword_NoSymbol_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "MyPassw0rd".Must().BeStrongPassword());
    }

    [Fact]
    public void BeStrongPassword_TooShort_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "Aa1!".Must().BeStrongPassword());
    }

    [Fact]
    public void BeStrongPassword_CustomMinLength_ShouldSucceed()
    {
        "Aa1!".Must().BeStrongPassword(minLength: 4);
    }

    [Fact]
    public void BeSha256Hash_ValidHash_ShouldSucceed()
    {
        "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855".Must().BeSha256Hash();
    }

    [Fact]
    public void BeSha256Hash_WithPrefix_ShouldSucceed()
    {
        "0xe3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855".Must().BeSha256Hash();
    }

    [Fact]
    public void BeSha256Hash_WrongLength_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "e3b0c44298fc1c14".Must().BeSha256Hash());
    }

    [Fact]
    public void BeMd5Hash_ValidHash_ShouldSucceed()
    {
        "d41d8cd98f00b204e9800998ecf8427e".Must().BeMd5Hash();
    }

    [Fact]
    public void BeMd5Hash_WrongLength_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "d41d8cd98f00b204".Must().BeMd5Hash());
    }

    [Fact]
    public void BeHashedWith_Sha256_ShouldSucceed()
    {
        "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855".Must().BeHashedWith(HashType.Sha256);
    }

    [Fact]
    public void BeHashedWith_Md5_ShouldSucceed()
    {
        "d41d8cd98f00b204e9800998ecf8427e".Must().BeHashedWith(HashType.Md5);
    }

    [Fact]
    public void BeHashedWith_Sha512_ShouldSucceed()
    {
        "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e".Must().BeHashedWith(HashType.Sha512);
    }

    [Fact]
    public void BeHashedWith_WrongType_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "d41d8cd98f00b204e9800998ecf8427e".Must().BeHashedWith(HashType.Sha256));
    }

    [Fact]
    public void BeJwtToken_ValidJwt_ShouldSucceed()
    {
        var header = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("""{"alg":"HS256","typ":"JWT"}""")).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        var payload = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("""{"sub":"1234567890","name":"John"}""")).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        var signature = "signature";
        var jwt = $"{header}.{payload}.{signature}";

        jwt.Must().BeJwtToken();
    }

    [Fact]
    public void BeJwtToken_InvalidJwt_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "not.a.jwt".Must().BeJwtToken());
    }

    [Fact]
    public void BeJwtToken_TwoParts_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "part1.part2".Must().BeJwtToken());
    }

    [Fact]
    public void BeJwt_Alias_ShouldSucceed()
    {
        var header = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("""{"alg":"HS256","typ":"JWT"}""")).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        var payload = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("""{"sub":"1234567890"}""")).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        var jwt = $"{header}.{payload}.sig";

        jwt.Must().BeJwt();
    }

    [Fact]
    public void BeApiKey_ValidKey_ShouldSucceed()
    {
        "abc123def456ghij789".Must().BeApiKey();
    }

    [Fact]
    public void BeApiKey_TooShort_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "abc123".Must().BeApiKey());
    }

    [Fact]
    public void BeApiKey_NoDigits_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "abcdefghijklmnopqrs".Must().BeApiKey());
    }

    [Fact]
    public void BeOAuthToken_ValidToken_ShouldSucceed()
    {
        "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.test".Must().BeOAuthToken();
    }

    [Fact]
    public void BeOAuthToken_TokenWithoutPrefix_ShouldSucceed()
    {
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.test".Must().BeOAuthToken();
    }

    [Fact]
    public void BeOAuthToken_TooShort_ShouldThrow()
    {
        Xunit.Assert.Throws<OmniAssertionException>(() => "short".Must().BeOAuthToken());
    }
}
