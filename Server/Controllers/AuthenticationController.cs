using Microsoft.AspNetCore.Mvc;
using Server.Services.Interfaces;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }
    
    [HttpPost("register")]
    public IActionResult Register(AuthenticationRequest request)
    {
        var (success, content) = _authenticationService.Register(request.UserName, request.Password);
        if (success == false)
        {
            return BadRequest(content);
        }
        
        return Login(request);
    }
    
    [HttpPost("login")]
    public IActionResult Login(AuthenticationRequest request)
    {
        
        var (success, content) = _authenticationService.Login(request.UserName, request.Password);
        if (success == false)
        {
            return BadRequest(content);
        }

        return Ok(new AuthenticationResponse(){Token = content});
    }
}