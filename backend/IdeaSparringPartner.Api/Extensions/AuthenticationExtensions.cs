using System.Text;
using IdeaSparringPartner.Api.Configuration;
using IdeaSparringPartner.Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace IdeaSparringPartner.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? new JwtSettings();

        if (string.IsNullOrWhiteSpace(jwtSettings.Secret))
            return services;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var message = context.AuthenticateFailure switch
                        {
                            null => ApiErrorResponse.Messages.Unauthorized,
                            { } failure when failure.Message.Contains("expired", StringComparison.OrdinalIgnoreCase)
                                => ApiErrorResponse.Messages.SessionExpired,
                            _ => ApiErrorResponse.Messages.InvalidToken
                        };

                        await context.Response.WriteAsJsonAsync(ApiErrorResponse.Create(message));
                    },
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(
                            ApiErrorResponse.Create(ApiErrorResponse.Messages.Forbidden));
                    }
                };
            });

        services.AddAuthorization();
        return services;
    }
}
