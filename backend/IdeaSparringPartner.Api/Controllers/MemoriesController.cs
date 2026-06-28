using IdeaSparringPartner.Api.Extensions;
using IdeaSparringPartner.Api.Models.Enums;
using IdeaSparringPartner.Api.Services.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdeaSparringPartner.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MemoriesController : ControllerBase
{
    private readonly MemoryExtractionService _memoryService;

    public MemoriesController(MemoryExtractionService memoryService)
    {
        _memoryService = memoryService;
    }

    [HttpGet]
    public async Task<ActionResult<object>> GetMemories(
        [FromQuery] Guid? ideaId,
        [FromQuery] MemoryScope? scope,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized(ApiErrorResponse.Create(ApiErrorResponse.Messages.Unauthorized));

        var items = await _memoryService.GetMemoriesAsync(userId.Value, ideaId, scope, cancellationToken);
        return Ok(new { items });
    }

    [HttpDelete("{memoryId:guid}")]
    public async Task<IActionResult> DeleteMemory(Guid memoryId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized(ApiErrorResponse.Create(ApiErrorResponse.Messages.Unauthorized));

        var deleted = await _memoryService.DeleteMemoryAsync(userId.Value, memoryId, cancellationToken);
        return deleted
            ? NoContent()
            : NotFound(ApiErrorResponse.Create(ApiErrorResponse.Messages.MemoryNotFound));
    }

    private Guid? GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
