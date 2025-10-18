using BackendApi.Context;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using BackendApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Plugins;

namespace BackendApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authService;
    private readonly AppDbContext _context;

    public AuthController(IAuthRepository authService, AppDbContext context)
    {
        _authService = authService;
        _context = context;
    }

    [HttpGet("roles")]
    [Authorize]
    public IActionResult GetAllRoles()
    {
        var currentUser = _authService.GetCurrentUserAsync();
        var roles = Enum.GetValues(typeof(UserRole))
            .Cast<UserRole>()
            .Select(r => new
            {
                Value = (int)r,
                RoleName = r.ToString()
            })
            .ToList();

        return Ok(roles);
    }


    [HttpGet("user-events")]
    public async Task<IActionResult> GetUsersLatestEvent()
    {
        try
        {
            var response = await _authService.GetUserLatestAction();
            return Ok(response.Data);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }


    [HttpGet("currentuser")]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var user = await _authService.GetCurrentUserAsync();
            return Ok(user);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
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
        var academicPeriod = await _authService.GetCurrentAcademicPeriodAsync();
        return Ok(new
        {
            token = result.NewToken,
            username = result.Username,
            fullname = result.Fullname,
            role = result.Role.ToString(),
            id = result.Id,
            academicYear = academicPeriod != null ? $"{academicPeriod.StartYear}-{academicPeriod.EndYear}" : null,
            semester = academicPeriod?.Semester
        });
    }
    [HttpPut("update-userRole/{id}")]
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

    [HttpGet("students-by-year-department")]
    public async Task<IActionResult> GetStudentsGroupedByYearAndDepartment()
    {
        var students = await _authService.GetAllStudents(); // Already returns StudentDto list

        var grouped = students
            .GroupBy(s => s.YearLevel)
            .Select(g => new
            {
                YearLevel = g.Key,
                Departments = g
                    .GroupBy(s => s.Department)
                    .Select(dg => new
                    {
                        Department = dg.Key,
                        Count = dg.Count()
                    })
                    .ToList()
            })
            .OrderBy(g => g.YearLevel)
            .ToList();

        return Ok(grouped);
    }


    [HttpDelete("delete-user/{id}")]
    public async Task<bool> DeleteStudentAsync(int id)
    {
        var student = await _context.Users
            .Include(u => u.MidtermGrades)
                .ThenInclude(m => m.Quizzes)
            .Include(u => u.MidtermGrades)
                .ThenInclude(m => m.ClassStandingItems)
            .Include(u => u.FinalsGrades)
                .ThenInclude(f => f.Quizzes)
            .Include(u => u.FinalsGrades)
                .ThenInclude(f => f.ClassStandingItems)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (student == null) return false;

        // Delete Midterm quizzes and class standing items (cascade is optional)
        _context.QuizLists.RemoveRange(student.MidtermGrades.SelectMany(m => m.Quizzes));
        _context.ClassStanding.RemoveRange(student.MidtermGrades.SelectMany(m => m.ClassStandingItems));

        // Delete Finals quizzes and class standing items manually (NoAction)
        _context.QuizLists.RemoveRange(student.FinalsGrades.SelectMany(f => f.Quizzes));
        _context.ClassStanding.RemoveRange(student.FinalsGrades.SelectMany(f => f.ClassStandingItems));

        // Delete the grades themselves
        _context.MidtermGrades.RemoveRange(student.MidtermGrades);
        _context.FinalsGrades.RemoveRange(student.FinalsGrades);

        // Finally delete the student
        _context.Users.Remove(student);

        await _context.SaveChangesAsync();
        return true;
    }


}