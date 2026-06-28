namespace IdeaSparringPartner.Api.Services.Ai;

public interface IAiService
{
    Task<string> GenerateTextAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default);
}
