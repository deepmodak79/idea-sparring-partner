using Microsoft.AspNetCore.Mvc;

namespace IdeaSparringPartner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            service = "Idea Sparring Partner API",
            timestamp = DateTime.UtcNow
        });
    }
}
