using Microsoft.AspNetCore.Mvc;

namespace LMS.WebApi.Controllers;

/// <summary>Simple liveness endpoint, equivalent to the Node <c>GET /test</c>.</summary>
[ApiController]
public class TestController : ControllerBase
{
    [HttpGet("test")]
    public IActionResult Test() => Ok(new { success = true, message = "Api is working" });
}
