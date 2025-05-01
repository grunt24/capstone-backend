using Capstone.Core.Entities.Dto;
using Capstone.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using NuGet.Protocol.Plugins;

namespace BackendApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDto userDto)
    {
        try
        {
            var token = await _authService.RegisterAsync(userDto.Username, userDto.Password);
            return Ok(new { message = "User created successfully", token });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDto request)
    {
        var result = await _authService.LoginAsync(request.Username, request.Password); // ✅ Await it!
        return Ok(new { token = result.NewToken, username = result.Username, role = result.Role }); // ✅ Only return needed fields
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUser()
    {
        var result = await _authService.GetAllUser();
        return Ok(result);
    }

    //[Authorize]
    //[HttpGet("protected")]
    //public IActionResult Protected()
    //{
    //    return Ok(new { message = "You are authorized!" });
    //}

}