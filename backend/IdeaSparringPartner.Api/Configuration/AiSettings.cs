namespace IdeaSparringPartner.Api.Configuration;

public class AiSettings
{
    public const string SectionName = "Ai";

    public string Provider { get; set; } = "Gemini";
    public string GeminiApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gemini-2.5-flash";
}
