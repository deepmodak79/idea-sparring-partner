namespace IdeaSparringPartner.Api.DTOs.Messages;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ThreadId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
