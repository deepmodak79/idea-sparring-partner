namespace IdeaSparringPartner.Api.Models;

public class Synthesis
{
    public Guid Id { get; set; }
    public Guid IdeaId { get; set; }
    public int Version { get; set; }
    public string StrongestChallengesJson { get; set; } = "[]";
    public string WeakestReasoningJson { get; set; } = "[]";
    public string UnresolvedTensionsJson { get; set; } = "[]";
    public DateTime CreatedAt { get; set; }

    public Idea Idea { get; set; } = null!;
}
