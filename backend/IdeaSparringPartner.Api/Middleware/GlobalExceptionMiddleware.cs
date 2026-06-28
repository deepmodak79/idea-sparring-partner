using System.Diagnostics;
using IdeaSparringPartner.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace IdeaSparringPartner.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, message) = MapException(ex);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        return context.Response.WriteAsJsonAsync(new
        {
            error = message,
            message,
            traceId
        });
    }

    private static (int StatusCode, string Message) MapException(Exception ex) => ex switch
    {
        InvalidOperationException operation => (StatusCodes.Status502BadGateway, operation.Message),
        UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, ApiErrorResponse.Messages.Unauthorized),
        DbUpdateException => (StatusCodes.Status503ServiceUnavailable, "Database operation failed. Please try again shortly."),
        ArgumentException argument => (StatusCodes.Status400BadRequest, argument.Message),
        _ => (StatusCodes.Status500InternalServerError, ApiErrorResponse.Messages.Unexpected)
    };
}
