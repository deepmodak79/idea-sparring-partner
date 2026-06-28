using IdeaSparringPartner.Api.DTOs.Syntheses;
using IdeaSparringPartner.Api.Services.Syntheses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdeaSparringPartner.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/ideas/{ideaId:guid}/[controller]")]
public class SynthesesController : ControllerBase
{
    private readonly SynthesisService _synthesisService;

    public SynthesesController(SynthesisService synthesisService)
    {
        _synthesisService = synthesisService;
    }

    [HttpGet]
    public async Task<ActionResult<object>> GetSyntheses(Guid ideaId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var items = await _synthesisService.GetSynthesesAsync(userId.Value, ideaId, cancellationToken);
        return Ok(new { items });
    }

    [HttpPost]
    public async Task<ActionResult<SynthesisDto>> CreateSynthesis(Guid ideaId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        try
        {
            var synthesis = await _synthesisService.CreateSynthesisAsync(userId.Value, ideaId, cancellationToken);
            if (synthesis is null) return NotFound();
            return Created($"/api/ideas/{ideaId}/syntheses/{synthesis.Id}", synthesis);
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
