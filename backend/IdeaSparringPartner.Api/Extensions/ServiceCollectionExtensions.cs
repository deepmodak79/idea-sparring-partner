using IdeaSparringPartner.Api.Configuration;
using IdeaSparringPartner.Api.Data;
using IdeaSparringPartner.Api.Services.Auth;
using Microsoft.EntityFrameworkCore;

namespace IdeaSparringPartner.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));
        }

        return services;
    }

    public static IServiceCollection AddApiConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<AiSettings>(configuration.GetSection(AiSettings.SectionName));

        var frontendUrl = configuration["FrontendUrl"] ?? "http://localhost:4300";

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(frontendUrl)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}
