using IdeaSparringPartner.Api.Data;
using IdeaSparringPartner.Api.Models;
using IdeaSparringPartner.Api.Models.Enums;
using IdeaSparringPartner.Api.Services.Ai;
using Microsoft.EntityFrameworkCore;

namespace IdeaSparringPartner.Api.Services.Threads;

public class ContextBuilder
{
    private readonly AppDbContext _dbContext;

    public ContextBuilder(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(string SystemPrompt, string UserPrompt)> BuildThreadReplyContextAsync(
        Models.Thread thread,
        CancellationToken cancellationToken)
    {
        var idea = await _dbContext.Ideas
            .AsNoTracking()
            .FirstAsync(i => i.Id == thread.IdeaId, cancellationToken);

        var threadMessages = await _dbContext.Messages
            .AsNoTracking()
            .Where(m => m.ThreadId == thread.Id)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

        var ideaMemories = await _dbContext.Memories
            .AsNoTracking()
            .Where(m => m.IdeaId == idea.Id && m.Scope == MemoryScope.Idea && !m.IsDeleted)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

        var userMemories = await _dbContext.Memories
            .AsNoTracking()
            .Where(m => m.UserId == idea.UserId && m.Scope == MemoryScope.User && !m.IsDeleted)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

        var systemPrompt = PersonaPrompts.GetSystemPrompt(thread.Persona) + """

            You are in an isolated thread. You must NOT reference messages from other persona threads.
            Shared knowledge may appear only in the memory sections below.
            Reply in 2-5 sentences. Challenge the user's reasoning directly.
            """;

        var conversation = string.Join("\n", threadMessages.Select(m =>
            $"{m.Role}: {m.Content}"));

        var memoryBlock = BuildMemoryBlock(ideaMemories, userMemories);

        var userPrompt = $"""
            Idea title: {idea.Title}
            Idea description: {idea.Description}

            {memoryBlock}

            Current thread conversation:
            {conversation}
            """;

        return (systemPrompt, userPrompt);
    }

    private static string BuildMemoryBlock(IReadOnlyList<Models.Memory> ideaMemories, IReadOnlyList<Models.Memory> userMemories)
    {
        if (ideaMemories.Count == 0 && userMemories.Count == 0)
            return "Active memories: none";

        var lines = new List<string> { "Active memories:" };
        foreach (var memory in ideaMemories)
            lines.Add($"- [Idea/{memory.Type}] {memory.Content}");
        foreach (var memory in userMemories)
            lines.Add($"- [User/{memory.Type}] {memory.Content}");

        return string.Join("\n", lines);
    }
}
