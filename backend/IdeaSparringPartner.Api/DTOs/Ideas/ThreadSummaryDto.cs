namespace IdeaSparringPartner.Api.DTOs.Ideas;

public class ThreadSummaryDto
{
    public Guid Id { get; set; }
    public string Persona { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int MessageCount { get; set; }
}
