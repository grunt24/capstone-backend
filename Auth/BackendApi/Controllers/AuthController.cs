using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using BackendApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Plugins;

namespace BackendApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authService;

    public AuthController(IAuthRepository authService)
    {
        _authService = authService;
    }

    [HttpPost("create-student")]
    public async Task<IActionResult> Register([FromBody] UserCredentialsDto userDto)
    {
        try
        {
            var result = await _authService.RegisterAsync(userDto);
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var result = await _authService.LoginAsync(request); 
        return Ok(new { token = result.NewToken, username = result.Username, fullname = result.Fullname, role = result.Role.ToString(), id = result.Id}); 
    }
    [HttpPut("update-role/{id}")]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UserRoleUpdateDto dto)
    {
        var result = await _authService.UpdateUserRoleAsync(id, dto);
        return Ok(result);
    }
    [HttpPut("update-user/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto dto)
    {
        var result = await _authService.UpdateUserDetailsAsync(id, dto);
        return Ok(result);
    }
    [HttpGet("all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _authService.GetAllUsersAsync();
        return Ok(users);
    }
    [HttpGet("all-students")]
    public async Task<IActionResult> GetAllStudents()
    {
        var users = await _authService.GetAllStudents();
        return Ok(users);
    }

    [HttpDelete("delete-user/{id}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var result = await _authService.DeleteStudentAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}