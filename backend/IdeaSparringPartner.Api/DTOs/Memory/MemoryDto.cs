namespace IdeaSparringPartner.Api.DTOs.Memory;

public class MemoryDto
{
    public Guid Id { get; set; }
    public string Scope { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid? IdeaId { get; set; }
    public Guid? SourceThreadId { get; set; }
    public Guid? SourceMessageId { get; set; }
    public DateTime CreatedAt { get; set; }
}
