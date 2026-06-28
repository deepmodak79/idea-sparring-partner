using IdeaSparringPartner.Api.Data;
using IdeaSparringPartner.Api.DTOs.Memory;
using IdeaSparringPartner.Api.Models;
using IdeaSparringPartner.Api.Models.Enums;
using IdeaSparringPartner.Api.Services.Ai;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace IdeaSparringPartner.Api.Services.Memory;

public class MemoryExtractionService
{
    private readonly AppDbContext _dbContext;
    private readonly IAiService _aiService;

    public MemoryExtractionService(AppDbContext dbContext, IAiService aiService)
    {
        _dbContext = dbContext;
        _aiService = aiService;
    }

    public async Task<MemoryDto?> ExtractMemoryAsync(
        Models.Thread thread,
        Message userMessage,
        Message assistantMessage,
        CancellationToken cancellationToken)
    {
        var systemPrompt = """
            Extract at most one memory from this exchange. Return JSON only:
            {"shouldStore":true|false,"scope":"Idea"|"User","type":"Fact|Assumption|Constraint|Pattern|Concession|Deflection|Risk|OpenQuestion","content":"..."}
            Use scope Idea for idea-specific facts. Use scope User for cross-idea user patterns.
            If nothing worth storing, set shouldStore to false.
            """;

        var userPrompt = $"""
            Persona: {thread.Persona}
            User: {userMessage.Content}
            Assistant: {assistantMessage.Content}
            """;

        try
        {
            var raw = await _aiService.GenerateTextAsync(systemPrompt, userPrompt, cancellationToken);
            var json = ExtractJson(raw);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.GetProperty("shouldStore").GetBoolean())
                return null;

            var scope = Enum.Parse<MemoryScope>(root.GetProperty("scope").GetString()!, true);
            var type = Enum.Parse<MemoryType>(root.GetProperty("type").GetString()!, true);
            var content = root.GetProperty("content").GetString()?.Trim();
            if (string.IsNullOrWhiteSpace(content)) return null;

            var idea = await _dbContext.Ideas.AsNoTracking().FirstAsync(i => i.Id == thread.IdeaId, cancellationToken);

            var memory = new Models.Memory
            {
                Id = Guid.NewGuid(),
                UserId = idea.UserId,
                IdeaId = scope == MemoryScope.Idea ? idea.Id : null,
                Scope = scope,
                Type = type,
                Content = content,
                SourceThreadId = thread.Id,
                SourceMessageId = assistantMessage.Id,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Memories.Add(memory);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return MapMemory(memory);
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<MemoryDto>> GetMemoriesAsync(
        Guid userId,
        Guid? ideaId,
        MemoryScope? scope,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Memories
            .AsNoTracking()
            .Where(m => m.UserId == userId && !m.IsDeleted);

        if (ideaId.HasValue)
            query = query.Where(m => m.IdeaId == ideaId || m.Scope == MemoryScope.User);

        if (scope.HasValue)
            query = query.Where(m => m.Scope == scope);

        return await query
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => MapMemory(m))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> DeleteMemoryAsync(Guid userId, Guid memoryId, CancellationToken cancellationToken)
    {
        var memory = await _dbContext.Memories
            .FirstOrDefaultAsync(m => m.Id == memoryId && m.UserId == userId && !m.IsDeleted, cancellationToken);

        if (memory is null) return false;

        memory.IsDeleted = true;
        memory.DeletedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static MemoryDto MapMemory(Models.Memory memory) => new()
    {
        Id = memory.Id,
        Scope = memory.Scope.ToString(),
        Type = memory.Type.ToString(),
        Content = memory.Content,
        IdeaId = memory.IdeaId,
        SourceThreadId = memory.SourceThreadId,
        SourceMessageId = memory.SourceMessageId,
        CreatedAt = memory.CreatedAt
    };

    private static string ExtractJson(string raw)
    {
        var start = raw.IndexOf('{');
        var end = raw.LastIndexOf('}');
        if (start >= 0 && end > start)
            return raw[start..(end + 1)];
        return raw;
    }
}
