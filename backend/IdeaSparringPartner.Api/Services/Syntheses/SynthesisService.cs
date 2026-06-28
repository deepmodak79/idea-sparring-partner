using System.Text.Json;
using IdeaSparringPartner.Api.Data;
using IdeaSparringPartner.Api.DTOs.Syntheses;
using IdeaSparringPartner.Api.Models;
using IdeaSparringPartner.Api.Services.Ai;
using Microsoft.EntityFrameworkCore;

namespace IdeaSparringPartner.Api.Services.Syntheses;

public class SynthesisService
{
    private readonly AppDbContext _dbContext;
    private readonly IAiService _aiService;

    public SynthesisService(AppDbContext dbContext, IAiService aiService)
    {
        _dbContext = dbContext;
        _aiService = aiService;
    }

    public async Task<List<SynthesisDto>> GetSynthesesAsync(Guid userId, Guid ideaId, CancellationToken cancellationToken)
    {
        var syntheses = await _dbContext.Syntheses
            .AsNoTracking()
            .Where(s => s.IdeaId == ideaId)
            .OrderBy(s => s.Version)
            .ToListAsync(cancellationToken);

        return syntheses.Select(Map).ToList();
    }

    public Task<bool> IdeaExistsForUserAsync(Guid userId, Guid ideaId, CancellationToken cancellationToken) =>
        _dbContext.Ideas.AnyAsync(i => i.Id == ideaId && i.UserId == userId, cancellationToken);

    public async Task<SynthesisDto?> CreateSynthesisAsync(Guid userId, Guid ideaId, CancellationToken cancellationToken)
    {
        var idea = await _dbContext.Ideas
            .Include(i => i.Threads)
            .ThenInclude(t => t.Messages)
            .FirstOrDefaultAsync(i => i.Id == ideaId && i.UserId == userId, cancellationToken);

        if (idea is null) return null;

        if (!idea.Threads.SelectMany(t => t.Messages).Any())
            throw new InvalidOperationException("Add at least one message in a thread before generating synthesis.");

        var conversations = idea.Threads
            .OrderBy(t => t.Persona)
            .Select(t => $"## {t.Persona}\n" + string.Join("\n", t.Messages.OrderBy(m => m.CreatedAt).Select(m => $"{m.Role}: {m.Content}")))
            .ToList();

        var systemPrompt = """
            You synthesize four isolated sparring threads for one idea.
            Return JSON only:
            {
              "strongestChallenges": ["..."],
              "weakestReasoning": ["..."],
              "unresolvedTensions": ["..."]
            }
            Each array should contain 2-5 concise bullet strings.
            """;

        var userPrompt = $"""
            Idea: {idea.Title}
            Description: {idea.Description}

            Threads:
            {string.Join("\n\n", conversations)}
            """;

        var raw = await _aiService.GenerateTextAsync(systemPrompt, userPrompt, cancellationToken);
        var json = ExtractJson(raw);
        using var doc = JsonDocument.Parse(json);

        var nextVersion = await _dbContext.Syntheses
            .Where(s => s.IdeaId == ideaId)
            .Select(s => (int?)s.Version)
            .MaxAsync(cancellationToken) ?? 0;

        var synthesis = new Synthesis
        {
            Id = Guid.NewGuid(),
            IdeaId = ideaId,
            Version = nextVersion + 1,
            StrongestChallengesJson = doc.RootElement.GetProperty("strongestChallenges").GetRawText(),
            WeakestReasoningJson = doc.RootElement.GetProperty("weakestReasoning").GetRawText(),
            UnresolvedTensionsJson = doc.RootElement.GetProperty("unresolvedTensions").GetRawText(),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Syntheses.Add(synthesis);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(synthesis);
    }

    private static SynthesisDto Map(Synthesis synthesis) => new()
    {
        Id = synthesis.Id,
        IdeaId = synthesis.IdeaId,
        Version = synthesis.Version,
        StrongestChallenges = JsonSerializer.Deserialize<List<string>>(synthesis.StrongestChallengesJson) ?? [],
        WeakestReasoning = JsonSerializer.Deserialize<List<string>>(synthesis.WeakestReasoningJson) ?? [],
        UnresolvedTensions = JsonSerializer.Deserialize<List<string>>(synthesis.UnresolvedTensionsJson) ?? [],
        CreatedAt = synthesis.CreatedAt
    };

    private static string ExtractJson(string raw)
    {
        var start = raw.IndexOf('{');
        var end = raw.LastIndexOf('}');
        if (start >= 0 && end > start) return raw[start..(end + 1)];
        return raw;
    }
}
