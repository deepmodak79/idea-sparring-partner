using IdeaSparringPartner.Api.Data;
using IdeaSparringPartner.Api.DTOs.Messages;
using IdeaSparringPartner.Api.Models;
using IdeaSparringPartner.Api.Models.Enums;
using IdeaSparringPartner.Api.Services.Ai;
using Microsoft.EntityFrameworkCore;

namespace IdeaSparringPartner.Api.Services.Ideas;

public class OpeningChallengeService
{
    private readonly AppDbContext _dbContext;
    private readonly IAiService _aiService;
    private readonly ILogger<OpeningChallengeService> _logger;

    public OpeningChallengeService(
        AppDbContext dbContext,
        IAiService aiService,
        ILogger<OpeningChallengeService> logger)
    {
        _dbContext = dbContext;
        _aiService = aiService;
        _logger = logger;
    }

    public async Task GenerateOpeningChallengesAsync(Idea idea, CancellationToken cancellationToken)
    {
        var threads = await _dbContext.Threads
            .Where(t => t.IdeaId == idea.Id)
            .ToListAsync(cancellationToken);

        foreach (var thread in threads)
        {
            try
            {
                await EnsureOpeningForThreadAsync(thread, idea, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to generate opening challenge for persona {Persona}", thread.Persona);
            }
        }
    }

    public async Task<MessageDto?> EnsureOpeningChallengeForUserAsync(
        Guid userId,
        Guid threadId,
        CancellationToken cancellationToken)
    {
        var thread = await _dbContext.Threads
            .Include(t => t.Idea)
            .FirstOrDefaultAsync(t => t.Id == threadId && t.Idea.UserId == userId, cancellationToken);

        if (thread is null)
            return null;

        var existing = await GetOpeningMessageAsync(thread.Id, cancellationToken);
        if (existing is not null)
            return existing;

        await EnsureOpeningForThreadAsync(thread, thread.Idea, cancellationToken);
        return await GetOpeningMessageAsync(thread.Id, cancellationToken);
    }

    private async Task EnsureOpeningForThreadAsync(
        Models.Thread thread,
        Idea idea,
        CancellationToken cancellationToken)
    {
        if (await _dbContext.Messages.AnyAsync(m => m.ThreadId == thread.Id, cancellationToken))
            return;

        var systemPrompt = PersonaPrompts.GetSystemPrompt(thread.Persona);
        var userPrompt = PersonaPrompts.BuildOpeningUserPrompt(idea.Title, idea.Description);
        var content = await _aiService.GenerateTextAsync(systemPrompt, userPrompt, cancellationToken);

        _dbContext.Messages.Add(new Message
        {
            Id = Guid.NewGuid(),
            ThreadId = thread.Id,
            Role = MessageRole.Assistant,
            Content = content,
            CreatedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<MessageDto?> GetOpeningMessageAsync(Guid threadId, CancellationToken cancellationToken)
    {
        var message = await _dbContext.Messages
            .AsNoTracking()
            .Where(m => m.ThreadId == threadId)
            .OrderBy(m => m.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        return message is null ? null : MapMessage(message);
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
