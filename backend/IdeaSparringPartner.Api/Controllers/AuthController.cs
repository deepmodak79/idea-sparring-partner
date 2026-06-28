using IdeaSparringPartner.Api.Data;
using IdeaSparringPartner.Api.DTOs.Auth;
using IdeaSparringPartner.Api.Extensions;
using IdeaSparringPartner.Api.Models;
using IdeaSparringPartner.Api.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdeaSparringPartner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthController(
        AppDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
    }

    [HttpPost("signup")]
    public async Task<ActionResult<AuthResponse>> Signup(
        [FromBody] SignupRequest request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        if (await _dbContext.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken))
            return Conflict(ApiErrorResponse.Create("Email already registered."));

        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            DisplayName = request.DisplayName.Trim(),
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            CreatedAt = now,
            UpdatedAt = now
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return StatusCode(StatusCodes.Status201Created, await BuildAuthResponseAsync(user, cancellationToken));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return Unauthorized(ApiErrorResponse.Create("Invalid email or password."));

        return Ok(await BuildAuthResponseAsync(user, cancellationToken));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<TokenRefreshResponse>> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var storedToken = await _refreshTokenService.ValidateRefreshTokenAsync(
            request.RefreshToken, cancellationToken);

        if (storedToken?.User is null)
            return Unauthorized(ApiErrorResponse.Create("Invalid or expired refresh token."));

        await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);

        var accessToken = _jwtTokenService.GenerateAccessToken(storedToken.User, out var expiresIn);
        var (newRefreshToken, _) = await _refreshTokenService.CreateRefreshTokenAsync(
            storedToken.User, cancellationToken);

        return Ok(new TokenRefreshResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = expiresIn
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized(ApiErrorResponse.Create(ApiErrorResponse.Messages.Unauthorized));

        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            return Unauthorized(ApiErrorResponse.Create("User account no longer exists. Please log in again."));

        return Ok(MapUser(user));
    }

    private Guid? GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
        return Guid.TryParse(value, out var id) ? id : null;
    }

    private async Task<AuthResponse> BuildAuthResponseAsync(User user, CancellationToken cancellationToken)
    {
        var accessToken = _jwtTokenService.GenerateAccessToken(user, out var expiresIn);
        var (refreshToken, _) = await _refreshTokenService.CreateRefreshTokenAsync(user, cancellationToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = expiresIn,
            User = MapUser(user)
        };
    }

    private static UserDto MapUser(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        DisplayName = user.DisplayName,
        CreatedAt = user.CreatedAt
    };
}
