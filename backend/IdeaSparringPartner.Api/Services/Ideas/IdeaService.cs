using IdeaSparringPartner.Api.Data;
using IdeaSparringPartner.Api.DTOs.Ideas;
using IdeaSparringPartner.Api.Models;
using IdeaSparringPartner.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace IdeaSparringPartner.Api.Services.Ideas;

public class IdeaService
{
    private readonly AppDbContext _dbContext;
    private readonly OpeningChallengeService _openingChallengeService;

    public IdeaService(AppDbContext dbContext, OpeningChallengeService openingChallengeService)
    {
        _dbContext = dbContext;
        _openingChallengeService = openingChallengeService;
    }

    public async Task<List<IdeaDto>> GetIdeasAsync(Guid userId, CancellationToken cancellationToken)
    {
        var ideas = await _dbContext.Ideas
            .AsNoTracking()
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.CreatedAt)
            .Include(i => i.Threads)
            .ThenInclude(t => t.Messages)
            .ToListAsync(cancellationToken);

        return ideas.Select(MapIdea).ToList();
    }

    public async Task<IdeaDto?> GetIdeaAsync(Guid userId, Guid ideaId, CancellationToken cancellationToken)
    {
        var idea = await _dbContext.Ideas
            .AsNoTracking()
            .Include(i => i.Threads)
            .ThenInclude(t => t.Messages)
            .FirstOrDefaultAsync(i => i.Id == ideaId && i.UserId == userId, cancellationToken);

        return idea is null ? null : MapIdea(idea);
    }

    public async Task<IdeaDto> CreateIdeaAsync(Guid userId, CreateIdeaRequest request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var idea = new Idea
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        foreach (PersonaType persona in Enum.GetValues<PersonaType>())
        {
            idea.Threads.Add(new Models.Thread
            {
                Id = Guid.NewGuid(),
                IdeaId = idea.Id,
                Persona = persona,
                Status = ThreadStatus.Active,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        _dbContext.Ideas.Add(idea);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _openingChallengeService.GenerateOpeningChallengesAsync(idea, cancellationToken);

        idea = await _dbContext.Ideas
            .AsNoTracking()
            .Include(i => i.Threads)
            .ThenInclude(t => t.Messages)
            .FirstAsync(i => i.Id == idea.Id, cancellationToken);

        return MapIdea(idea);
    }

    private static IdeaDto MapIdea(Idea idea) => new()
    {
        Id = idea.Id,
        Title = idea.Title,
        Description = idea.Description,
        CreatedAt = idea.CreatedAt,
        UpdatedAt = idea.UpdatedAt,
        Threads = idea.Threads
            .OrderBy(t => t.Persona)
            .Select(t => new ThreadSummaryDto
            {
                Id = t.Id,
                Persona = t.Persona.ToString(),
                Status = t.Status.ToString(),
                MessageCount = t.Messages.Count
            }).ToList()
    };
}
