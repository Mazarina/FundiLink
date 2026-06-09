using Microsoft.AspNetCore.Mvc;

namespace FundiLink.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet("/health")]
    public IActionResult Get() => Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
}
