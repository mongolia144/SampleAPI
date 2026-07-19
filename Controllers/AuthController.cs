namespace SampleApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using SampleApi.DTOs.Auth;
using SampleApi.Interfaces.AuthInterfaces;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        var user = _authService.ValidateUser(dto.Email, dto.Password);

        if (user is null)
            return Unauthorized("Invalid email or password");

        // ⭐ Next step: generate JWT token here
        var token = _authService.GenerateJwtToken(user);
        return Ok(new
        {
            Token = token
        });
    }
}
