using System.Net.Http.Json;
using System.Text.Json;
using IdeaSparringPartner.Api.Configuration;
using Microsoft.Extensions.Options;

namespace IdeaSparringPartner.Api.Services.Ai;

public class GeminiAiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly AiSettings _settings;
    private readonly ILogger<GeminiAiService> _logger;

    public GeminiAiService(HttpClient httpClient, IOptions<AiSettings> settings, ILogger<GeminiAiService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string> GenerateTextAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.GeminiApiKey))
            throw new InvalidOperationException("Gemini API key is not configured on the server.");

        var model = string.IsNullOrWhiteSpace(_settings.Model) ? "gemini-2.5-flash" : _settings.Model;
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={_settings.GeminiApiKey}";

        var payload = new
        {
            systemInstruction = new { parts = new[] { new { text = systemPrompt } } },
            contents = new[]
            {
                new { role = "user", parts = new[] { new { text = userPrompt } } }
            }
        };

        using var response = await _httpClient.PostAsJsonAsync(url, payload, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Gemini API error: {Status} {Body}", response.StatusCode, body);
            throw new InvalidOperationException(ParseGeminiError(body, response.StatusCode));
        }

        using var doc = JsonDocument.Parse(body);
        if (!doc.RootElement.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
            throw new InvalidOperationException("Gemini returned an empty response. Please try again.");

        var text = candidates[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return text?.Trim() ?? string.Empty;
    }

    private static string ParseGeminiError(string body, System.Net.HttpStatusCode statusCode)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("error", out var error) &&
                error.TryGetProperty("message", out var message))
            {
                var detail = message.GetString();
                if (!string.IsNullOrWhiteSpace(detail))
                {
                    if (detail.Contains("quota", StringComparison.OrdinalIgnoreCase) ||
                        detail.Contains("rate", StringComparison.OrdinalIgnoreCase))
                        return "AI rate limit reached. Please wait a moment and try again.";

                    if (detail.Contains("API key", StringComparison.OrdinalIgnoreCase))
                        return "Gemini API key is invalid or missing on the server.";

                    if (detail.Contains("not found", StringComparison.OrdinalIgnoreCase))
                        return "Configured Gemini model is unavailable. Check Ai__Model on the server.";

                    return $"AI request failed: {detail}";
                }
            }
        }
        catch
        {
            // Fall through to generic message.
        }

        return statusCode switch
        {
            System.Net.HttpStatusCode.TooManyRequests =>
                "AI rate limit reached. Please wait a moment and try again.",
            System.Net.HttpStatusCode.Unauthorized or System.Net.HttpStatusCode.Forbidden =>
                "Gemini API key is invalid or missing on the server.",
            _ => "AI provider request failed. Please try again shortly."
        };
    }
}