using IdeaSparringPartner.Api.Models.Enums;

namespace IdeaSparringPartner.Api.Models;

public class Memory
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? IdeaId { get; set; }
    public MemoryScope Scope { get; set; }
    public MemoryType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? SourceThreadId { get; set; }
    public Guid? SourceMessageId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public User User { get; set; } = null!;
    public Idea? Idea { get; set; }
    public Thread? SourceThread { get; set; }
    public Message? SourceMessage { get; set; }
}
