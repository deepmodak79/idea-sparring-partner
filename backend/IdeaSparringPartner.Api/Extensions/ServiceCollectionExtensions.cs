using IdeaSparringPartner.Api.Configuration;
using IdeaSparringPartner.Api.Data;
using IdeaSparringPartner.Api.Services.Ai;
using IdeaSparringPartner.Api.Services.Auth;
using IdeaSparringPartner.Api.Services.Ideas;
using IdeaSparringPartner.Api.Services.Memory;
using IdeaSparringPartner.Api.Services.Syntheses;
using IdeaSparringPartner.Api.Services.Threads;
using Microsoft.EntityFrameworkCore;

namespace IdeaSparringPartner.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IdeaService, IdeaService>();
        services.AddScoped<OpeningChallengeService, OpeningChallengeService>();
        services.AddScoped<ContextBuilder, ContextBuilder>();
        services.AddScoped<ThreadMessageService, ThreadMessageService>();
        services.AddScoped<MemoryExtractionService, MemoryExtractionService>();
        services.AddScoped<SynthesisService, SynthesisService>();
        services.AddHttpClient<IAiService, GeminiAiService>();
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
        var extraOrigins = configuration["FrontendUrls"]?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            ?? [];

        var allowedOrigins = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            frontendUrl.TrimEnd('/')
        };
        foreach (var origin in extraOrigins)
            allowedOrigins.Add(origin.TrimEnd('/'));

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.SetIsOriginAllowed(origin =>
                {
                    if (string.IsNullOrWhiteSpace(origin))
                        return false;

                    var normalized = origin.TrimEnd('/');
                    if (allowedOrigins.Contains(normalized))
                        return true;

                    if (!Uri.TryCreate(normalized, UriKind.Absolute, out var uri))
                        return false;

                    if (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
                        return true;

                    // Netlify production + deploy preview URLs (*.netlify.app)
                    return uri.Host.EndsWith(".netlify.app", StringComparison.OrdinalIgnoreCase);
                })
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });

        return services;
    }

    public static IServiceCollection AddJwtAuthenticationIfConfigured(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services.AddJwtAuthentication(configuration);
}
