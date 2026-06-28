using IdeaSparringPartner.Api.Data;
using IdeaSparringPartner.Api.DTOs.Messages;
using IdeaSparringPartner.Api.DTOs.Threads;
using IdeaSparringPartner.Api.Models;
using IdeaSparringPartner.Api.Models.Enums;
using IdeaSparringPartner.Api.Services.Ai;
using IdeaSparringPartner.Api.Services.Memory;
using Microsoft.EntityFrameworkCore;

namespace IdeaSparringPartner.Api.Services.Threads;

public class ThreadMessageService
{
    private readonly AppDbContext _dbContext;
    private readonly ContextBuilder _contextBuilder;
    private readonly IAiService _aiService;
    private readonly MemoryExtractionService _memoryExtractionService;

    public ThreadMessageService(
        AppDbContext dbContext,
        ContextBuilder contextBuilder,
        IAiService aiService,
        MemoryExtractionService memoryExtractionService)
    {
        _dbContext = dbContext;
        _contextBuilder = contextBuilder;
        _aiService = aiService;
        _memoryExtractionService = memoryExtractionService;
    }

    public async Task<List<MessageDto>?> GetMessagesAsync(Guid userId, Guid threadId, CancellationToken cancellationToken)
    {
        var thread = await GetAuthorizedThreadAsync(userId, threadId, cancellationToken);
        if (thread is null) return null;

        return await _dbContext.Messages
            .AsNoTracking()
            .Where(m => m.ThreadId == thread.Id)
            .OrderBy(m => m.CreatedAt)
            .Select(m => MapMessage(m))
            .ToListAsync(cancellationToken);
    }

    public async Task<PostMessageResponse?> PostMessageAsync(
        Guid userId,
        Guid threadId,
        string content,
        CancellationToken cancellationToken)
    {
        var thread = await GetAuthorizedThreadAsync(userId, threadId, cancellationToken);
        if (thread is null) return null;

        var trimmed = content.Trim();
        if (string.IsNullOrWhiteSpace(trimmed)) return null;

        var now = DateTime.UtcNow;
        var userMessage = new Message
        {
            Id = Guid.NewGuid(),
            ThreadId = thread.Id,
            Role = MessageRole.User,
            Content = trimmed,
            CreatedAt = now
        };
        _dbContext.Messages.Add(userMessage);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var (systemPrompt, userPrompt) = await _contextBuilder.BuildThreadReplyContextAsync(thread, cancellationToken);
        var assistantText = await _aiService.GenerateTextAsync(systemPrompt, userPrompt, cancellationToken);

        var assistantMessage = new Message
        {
            Id = Guid.NewGuid(),
            ThreadId = thread.Id,
            Role = MessageRole.Assistant,
            Content = assistantText,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Messages.Add(assistantMessage);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var extractedMemory = await _memoryExtractionService.ExtractMemoryAsync(
            thread, userMessage, assistantMessage, cancellationToken);

        return new PostMessageResponse
        {
            UserMessage = MapMessage(userMessage),
            AssistantMessage = MapMessage(assistantMessage),
            ExtractedMemory = extractedMemory
        };
    }

    public async Task<List<ThreadListItemDto>> GetThreadsForIdeaAsync(
        Guid userId,
        Guid ideaId,
        CancellationToken cancellationToken)
    {
        var ideaExists = await IdeaExistsForUserAsync(userId, ideaId, cancellationToken);
        if (!ideaExists) return [];

        return await _dbContext.Threads
            .AsNoTracking()
            .Where(t => t.IdeaId == ideaId)
            .OrderBy(t => t.Persona)
            .Select(t => new ThreadListItemDto
            {
                Id = t.Id,
                IdeaId = t.IdeaId,
                Persona = t.Persona.ToString(),
                Status = t.Status.ToString(),
                CreatedAt = t.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IdeaExistsForUserAsync(Guid userId, Guid ideaId, CancellationToken cancellationToken) =>
        _dbContext.Ideas.AnyAsync(i => i.Id == ideaId && i.UserId == userId, cancellationToken);

    private async Task<Models.Thread?> GetAuthorizedThreadAsync(
        Guid userId,
        Guid threadId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Threads
            .Include(t => t.Idea)
            .FirstOrDefaultAsync(t => t.Id == threadId && t.Idea.UserId == userId, cancellationToken);
    }

    private static MessageDto MapMessage(Message message) => new()
    {
        Id = message.Id,
        ThreadId = message.ThreadId,
        Role = message.Role.ToString(),
        Content = message.Content,
        CreatedAt = message.CreatedAt
    };
}
