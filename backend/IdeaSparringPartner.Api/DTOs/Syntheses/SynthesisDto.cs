namespace IdeaSparringPartner.Api.DTOs.Syntheses;

public class SynthesisDto
{
    public Guid Id { get; set; }
    public Guid IdeaId { get; set; }
    public int Version { get; set; }
    public List<string> StrongestChallenges { get; set; } = [];
    public List<string> WeakestReasoning { get; set; } = [];
    public List<string> UnresolvedTensions { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}
