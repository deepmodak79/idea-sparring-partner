using IdeaSparringPartner.Api.Models.Enums;

namespace IdeaSparringPartner.Api.Models;

public class Message
{
    public Guid Id { get; set; }
    public Guid ThreadId { get; set; }
    public MessageRole Role { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Thread Thread { get; set; } = null!;
}
