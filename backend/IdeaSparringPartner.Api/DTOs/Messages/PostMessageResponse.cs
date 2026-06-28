using IdeaSparringPartner.Api.DTOs.Memory;

namespace IdeaSparringPartner.Api.DTOs.Messages;

public class PostMessageResponse
{
    public MessageDto UserMessage { get; set; } = null!;
    public MessageDto AssistantMessage { get; set; } = null!;
    public MemoryDto? ExtractedMemory { get; set; }
}
