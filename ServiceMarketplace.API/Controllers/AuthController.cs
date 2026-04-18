using Microsoft.AspNetCore.Mvc;
using ServiceMarketplace.Application.Interfaces;
using ServiceMarketplace.API.Models.Auth;

namespace ServiceMarketplace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var token = await _authService.RegisterAsync(
            request.Email,
            request.Password,
            request.FullName
        );

        return Ok(token);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _authService.LoginAsync(
            request.Email,
            request.Password
        );

        return Ok(token);
    }
}