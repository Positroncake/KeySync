using Backend;
using KeySync.Shared;
using Microsoft.AspNetCore.Mvc;

namespace KeySync.Server.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private const string ConnStrFilePath = "/home/positron/connStr";
    
    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Register([FromBody] AuthRequest request)
    {
        #region Check input

        string username = request.Username;
        string password = request.Password;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return BadRequest("Username and password cannot be blank.");

        #endregion

        #region Create account

        var auth = new Authentication(ConnStrFilePath);
        string token = await auth.NewAccount(username, password);
        return Ok(token);

        #endregion
    }

    public async Task<ActionResult> Login([FromBody] AuthRequest request)
    {
        #region Check input

        string username = request.Username;
        string password = request.Password;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return BadRequest("Username and password cannot be blank.");

        #endregion

        #region Check credentials

        var auth = new Authentication(ConnStrFilePath);
        (bool valid, string token) = await auth.Login(username, password);
        if (valid == false) return Unauthorized("Invalid username or password.");
        return Ok(token);

        #endregion
    }
}