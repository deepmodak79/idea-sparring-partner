using IdeaSparringPartner.Api.Data;
using IdeaSparringPartner.Api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace IdeaSparringPartner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public HealthController(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            service = "Idea Sparring Partner API",
            timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("database")]
    public async Task<IActionResult> GetDatabase(CancellationToken cancellationToken)
    {
        var timestamp = DateTime.UtcNow;
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ApiErrorResponse.Create(
                "Database connection is not configured on the server."));
        }

        var dbContext = _serviceProvider.GetService<AppDbContext>();
        if (dbContext is null)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ApiErrorResponse.Create(
                "Database is not configured on the server."));
        }

        try
        {
            var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

            if (canConnect)
            {
                return Ok(new
                {
                    status = "ok",
                    database = "reachable",
                    timestamp
                });
            }

            return StatusCode(StatusCodes.Status503ServiceUnavailable, ApiErrorResponse.Create(
                "Database is unreachable. Check the connection string and Supabase status."));
        }
        catch
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ApiErrorResponse.Create(
                "Database connection failed. Please try again shortly."));
        }
    }
}
