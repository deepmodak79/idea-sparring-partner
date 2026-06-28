namespace IdeaSparringPartner.Api.DTOs.Threads;

public class ThreadListItemDto
{
    public Guid Id { get; set; }
    public Guid IdeaId { get; set; }
    public string Persona { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
