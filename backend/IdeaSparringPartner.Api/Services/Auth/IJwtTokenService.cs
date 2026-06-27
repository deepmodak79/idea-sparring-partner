using IdeaSparringPartner.Api.Models;

namespace IdeaSparringPartner.Api.Services.Auth;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user, out int expiresInSeconds);
}
