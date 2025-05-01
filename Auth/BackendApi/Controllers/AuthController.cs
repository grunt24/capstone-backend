using BCASGradePortal.Core.Authentication;
using BCASGradePortal.Core.Models;
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
            var result = await _authService.RegisterAsync(userDto.Username, userDto.Password);
            return Ok(result);
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
        return Ok(new { token = result.NewToken, username = result.Username}); // ✅ Only return needed fields
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUser()
    {
        var result = await _authService.UsersList();
        return Ok(result);
    }

    //[Authorize]
    //[HttpGet("protected")]
    //public IActionResult Protected()
    //{
    //    return Ok(new { message = "You are authorized!" });
    //}

}