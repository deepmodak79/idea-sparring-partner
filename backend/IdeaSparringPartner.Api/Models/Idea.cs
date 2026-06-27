namespace IdeaSparringPartner.Api.Models;

public class Idea
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User User { get; set; } = null!;
    public ICollection<Thread> Threads { get; set; } = [];
    public ICollection<Memory> Memories { get; set; } = [];
    public ICollection<Synthesis> Syntheses { get; set; } = [];
}
