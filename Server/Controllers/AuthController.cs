using Microsoft.AspNetCore.Mvc;

namespace KeySync.Server.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Register()
    {
        return Ok();
    }
}