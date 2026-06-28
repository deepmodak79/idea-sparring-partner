using IdeaSparringPartner.Api.Data;
using IdeaSparringPartner.Api.Models;
using IdeaSparringPartner.Api.Models.Enums;
using IdeaSparringPartner.Api.Services.Threads;
using Microsoft.EntityFrameworkCore;

namespace IdeaSparringPartner.Api.Tests;

public class ContextBuilderTests
{
    [Fact]
    public async Task BuildThreadReplyContext_IncludesOnlyCurrentThreadMessages()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new AppDbContext(options);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = "hash",
            DisplayName = "Test",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var idea = new Idea
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Title = "Test idea",
            Description = "Desc",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var pragmatist = new Models.Thread
        {
            Id = Guid.NewGuid(),
            IdeaId = idea.Id,
            Persona = PersonaType.Pragmatist,
            Status = ThreadStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var skeptic = new Models.Thread
        {
            Id = Guid.NewGuid(),
            IdeaId = idea.Id,
            Persona = PersonaType.Skeptic,
            Status = ThreadStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        db.Ideas.Add(idea);
        db.Threads.AddRange(pragmatist, skeptic);
        db.Messages.AddRange(
            new Message { Id = Guid.NewGuid(), ThreadId = pragmatist.Id, Role = MessageRole.User, Content = "Pragmatist-only", CreatedAt = DateTime.UtcNow },
            new Message { Id = Guid.NewGuid(), ThreadId = skeptic.Id, Role = MessageRole.User, Content = "Skeptic-only", CreatedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var builder = new ContextBuilder(db);
        var (_, userPrompt) = await builder.BuildThreadReplyContextAsync(pragmatist, CancellationToken.None);

        Assert.Contains("Pragmatist-only", userPrompt);
        Assert.DoesNotContain("Skeptic-only", userPrompt);
    }
}
