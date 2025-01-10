using Microsoft.AspNetCore.Mvc;
using TicketingSystem.DTOs.User;
using TicketingSystem.Interfaces;

namespace TicketingSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
        
    }

    [HttpPost("register")]
    public async Task<ActionResult<Response<string>>> Register([FromBody] UserRegisterDTO userDto)
    {
        var result = await _authService.RegisterUser(userDto);
        Console.WriteLine($"Resultado: {result}");
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<Response<LoginResponseDTO>>> Login([FromBody] UserLoginDTO loginDto)
    {
        var result = await _authService.LoginUser(loginDto);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }
}