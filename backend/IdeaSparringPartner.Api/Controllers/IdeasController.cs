using IdeaSparringPartner.Api.DTOs.Ideas;
using IdeaSparringPartner.Api.Services.Ideas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdeaSparringPartner.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class IdeasController : ControllerBase
{
    private readonly IdeaService _ideaService;

    public IdeasController(IdeaService ideaService)
    {
        _ideaService = ideaService;
    }

    [HttpGet]
    public async Task<ActionResult<object>> GetIdeas(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var items = await _ideaService.GetIdeasAsync(userId.Value, cancellationToken);
        return Ok(new { items });
    }

    [HttpPost]
    public async Task<ActionResult<IdeaDto>> CreateIdea(
        [FromBody] CreateIdeaRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
            return BadRequest(new { error = "Title and description are required." });

        var idea = await _ideaService.CreateIdeaAsync(userId.Value, request, cancellationToken);
        return Created($"/api/ideas/{idea.Id}", idea);
    }

    [HttpGet("{ideaId:guid}")]
    public async Task<ActionResult<IdeaDto>> GetIdea(Guid ideaId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var idea = await _ideaService.GetIdeaAsync(userId.Value, ideaId, cancellationToken);
        return idea is null ? NotFound() : Ok(idea);
    }

    private Guid? GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
