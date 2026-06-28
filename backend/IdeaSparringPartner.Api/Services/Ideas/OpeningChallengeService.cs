using IdeaSparringPartner.Api.Data;
using IdeaSparringPartner.Api.Models;
using IdeaSparringPartner.Api.Models.Enums;
using IdeaSparringPartner.Api.Services.Ai;
using Microsoft.EntityFrameworkCore;

namespace IdeaSparringPartner.Api.Services.Ideas;

public class OpeningChallengeService
{
    private readonly AppDbContext _dbContext;
    private readonly IAiService _aiService;

    public OpeningChallengeService(AppDbContext dbContext, IAiService aiService)
    {
        _dbContext = dbContext;
        _aiService = aiService;
    }

    public async Task GenerateOpeningChallengesAsync(Idea idea, CancellationToken cancellationToken)
    {
        var threads = await _dbContext.Threads
            .Where(t => t.IdeaId == idea.Id)
            .ToListAsync(cancellationToken);

        foreach (var thread in threads)
        {
            if (await _dbContext.Messages.AnyAsync(m => m.ThreadId == thread.Id, cancellationToken))
                continue;

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
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
