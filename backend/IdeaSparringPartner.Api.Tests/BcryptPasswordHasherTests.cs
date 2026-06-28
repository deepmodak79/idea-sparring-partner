using IdeaSparringPartner.Api.Services.Auth;

namespace IdeaSparringPartner.Api.Tests;

public class BcryptPasswordHasherTests
{
    [Fact]
    public void HashAndVerify_RoundTrip_Succeeds()
    {
        var hasher = new BcryptPasswordHasher();
        var hash = hasher.HashPassword("password123");
        Assert.True(hasher.VerifyPassword("password123", hash));
        Assert.False(hasher.VerifyPassword("wrong", hash));
    }
}
