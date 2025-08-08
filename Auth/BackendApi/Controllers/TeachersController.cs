using BackendApi.Core.General;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using BackendApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherRepository _teacherService;

        public TeachersController(ITeacherRepository teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Superadmin,Teacher")]
        public async Task<ActionResult<IEnumerable<TeacherWithSubjectsDto>>> GetAllTeachers()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new GeneralServiceResponse { Success = false, Message = "Unauthorized user" });

            int userId = int.Parse(userIdClaim);

            IEnumerable<TeacherWithSubjectsDto> teachers;

            if (userRole == "Admin" || userRole == "Superadmin")
            {
                teachers = await _teacherService.GetAllTeachers();
            }
            else if (userRole == "Teacher")
            {
                var teacher = await _teacherService.GetTeacherByUserId(userId);
                if (teacher == null)
                    return NotFound(new GeneralServiceResponse { Success = false, Message = "Teacher not found for this user" });

                teachers = new List<TeacherWithSubjectsDto> { teacher };
            }
            else
            {
                return Forbid(); // or Unauthorized()
            }

            return Ok(teachers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherWithSubjectsDto>> GetTeacherById(int id)
        {
            var teacher = await _teacherService.GetTeacherById(id);
            if (teacher == null)
                return NotFound(new GeneralServiceResponse { Success = false, Message = "Teacher not found" });

            return Ok(teacher);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GeneralServiceResponse>> UpdateTeacher(int id, [FromBody] TeacherDto teacherDto)
        {
            var result = await _teacherService.UpdateTeacher(id, teacherDto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralServiceResponse>> DeleteTeacher(int id)
        {
            var result = await _teacherService.DeleteTeacher(id);
            return Ok(result);
        }
        [HttpPost("create-teacher-with-subjects")]
        public async Task<IActionResult> CreateTeacherWithSubjects([FromBody] CreateTeacherWithAccountDto dto)
        {
            var result = await _teacherService.CreateTeacherWithAccountAsync(dto);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

    }
}
