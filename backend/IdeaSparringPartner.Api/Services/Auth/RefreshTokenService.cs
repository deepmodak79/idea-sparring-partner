using System.Security.Cryptography;
using System.Text;
using IdeaSparringPartner.Api.Configuration;
using IdeaSparringPartner.Api.Data;
using IdeaSparringPartner.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IdeaSparringPartner.Api.Services.Auth;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AppDbContext _dbContext;
    private readonly JwtSettings _settings;

    public RefreshTokenService(AppDbContext dbContext, IOptions<JwtSettings> settings)
    {
        _dbContext = dbContext;
        _settings = settings.Value;
    }

    public async Task<(string RawToken, RefreshToken Entity)> CreateRefreshTokenAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        var rawToken = GenerateSecureToken();
        var entity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = HashToken(rawToken),
            ExpiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenDays),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.RefreshTokens.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (rawToken, entity);
    }

    public async Task<RefreshToken?> ValidateRefreshTokenAsync(
        string rawToken,
        CancellationToken cancellationToken = default)
    {
        var hash = HashToken(rawToken);

        return await _dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt =>
                rt.TokenHash == hash &&
                rt.RevokedAt == null &&
                rt.ExpiresAt > DateTime.UtcNow,
                cancellationToken);
    }

    public async Task RevokeRefreshTokenAsync(string rawToken, CancellationToken cancellationToken = default)
    {
        var hash = HashToken(rawToken);
        var token = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == hash, cancellationToken);

        if (token is null || token.RevokedAt is not null)
            return;

        token.RevokedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public string HashToken(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes);
    }

    private static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}
