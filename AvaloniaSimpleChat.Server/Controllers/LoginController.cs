using Microsoft.AspNetCore.Mvc;
using AvaloniaSimpleChat.Server.Services;

namespace AvaloniaSimpleChat.Server.Controllers;

[ApiController]
[Route("api/login")]
public class LoginController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ISecurityService _securityService;

    public LoginController(IUserService userService, ISecurityService securityService)
    {
        _userService = userService;
        _securityService = securityService;
    }

    [HttpPost]
    public ActionResult<string> Login(LoginRequest loginRequest)
    {
        if (!_userService.VerifyOrCreateUser(loginRequest.Username, loginRequest.Password))
            return Unauthorized();
        
        return _securityService.GenerateToken(loginRequest.Username);
    }
}

public sealed class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}