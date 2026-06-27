using IdeaSparringPartner.Api.Models;

namespace IdeaSparringPartner.Api.Services.Auth;

public interface IRefreshTokenService
{
    Task<(string RawToken, RefreshToken Entity)> CreateRefreshTokenAsync(User user, CancellationToken cancellationToken = default);
    Task<RefreshToken?> ValidateRefreshTokenAsync(string rawToken, CancellationToken cancellationToken = default);
    Task RevokeRefreshTokenAsync(string rawToken, CancellationToken cancellationToken = default);
    string HashToken(string rawToken);
}
