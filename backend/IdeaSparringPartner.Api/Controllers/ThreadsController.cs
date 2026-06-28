using IdeaSparringPartner.Api.DTOs.Messages;
using IdeaSparringPartner.Api.Services.Threads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdeaSparringPartner.Api.Controllers;

[Authorize]
[ApiController]
[Route("api")]
public class ThreadsController : ControllerBase
{
    private readonly ThreadMessageService _threadMessageService;

    public ThreadsController(ThreadMessageService threadMessageService)
    {
        _threadMessageService = threadMessageService;
    }

    [HttpGet("ideas/{ideaId:guid}/threads")]
    public async Task<ActionResult<object>> GetThreadsForIdea(Guid ideaId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var items = await _threadMessageService.GetThreadsForIdeaAsync(userId.Value, ideaId, cancellationToken);
        return Ok(new { items });
    }

    [HttpGet("threads/{threadId:guid}/messages")]
    public async Task<ActionResult<object>> GetMessages(Guid threadId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var items = await _threadMessageService.GetMessagesAsync(userId.Value, threadId, cancellationToken);
        if (items is null) return NotFound();

        return Ok(new { items });
    }

    [HttpPost("threads/{threadId:guid}/messages")]
    public async Task<ActionResult<PostMessageResponse>> PostMessage(
        Guid threadId,
        [FromBody] PostMessageRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest(new { error = "Content is required." });

        try
        {
            var result = await _threadMessageService.PostMessageAsync(
                userId.Value, threadId, request.Content, cancellationToken);

            if (result is null) return NotFound();
            return Created($"/api/threads/{threadId}/messages", result);
        }
        catch (InvalidOperationException)
        {
            return StatusCode(502, new { error = "AI provider request failed." });
        }
    }

    private Guid? GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
