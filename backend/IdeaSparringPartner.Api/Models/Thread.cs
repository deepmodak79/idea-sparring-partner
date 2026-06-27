using IdeaSparringPartner.Api.Models.Enums;

namespace IdeaSparringPartner.Api.Models;

public class Thread
{
    public Guid Id { get; set; }
    public Guid IdeaId { get; set; }
    public PersonaType Persona { get; set; }
    public ThreadStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Idea Idea { get; set; } = null!;
    public ICollection<Message> Messages { get; set; } = [];
}
