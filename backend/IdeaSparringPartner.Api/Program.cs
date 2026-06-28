using IdeaSparringPartner.Api.Configuration;
using IdeaSparringPartner.Api.Extensions;
using IdeaSparringPartner.Api.Middleware;

// Containers (Render) must not watch appsettings.json — exhausts inotify limits.
Environment.SetEnvironmentVariable("DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE", "false");

EnvFileLoader.Load(
    Path.Combine(Directory.GetCurrentDirectory(), ".env"),
    Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env"));

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiConfiguration(builder.Configuration)
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddJwtAuthenticationIfConfigured(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Json(new
{
    service = "Idea Sparring Partner API",
    status = "ok",
    health = "/api/health",
    database = "/api/health/database"
}));

app.Run();
