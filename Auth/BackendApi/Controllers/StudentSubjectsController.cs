using BackendApi.Core.General;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using BackendApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentSubjectsController : ControllerBase
    {
        private readonly IStudentSubjectService _studentSubjectService;

        public StudentSubjectsController(IStudentSubjectService studentSubjectService)
        {
            _studentSubjectService = studentSubjectService;
        }

        // GET: api/StudentSubjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentSubject>>> GetAllStudentSubjects()
        {
            var studentSubjects = await _studentSubjectService.GetAllStudentSubjects();
            return Ok(studentSubjects);
        }

        // GET: api/StudentSubjects/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentSubject>> GetStudentSubjectById(int id)
        {
            var studentSubject = await _studentSubjectService.GetStudentSubjectById(id);
            if (studentSubject == null)
            {
                return NotFound(new GeneralServiceResponse { Success = false, Message = "StudentSubject not found" });
            }

            return Ok(studentSubject);
        }

        // ✅ POST: api/StudentSubjects (Bulk add multiple subjects for 1 student)
        [HttpPost]
        public async Task<ActionResult<GeneralServiceResponse>> AddMultipleStudentSubjects([FromBody] StudentSubjectsDto dto)
        {
            var result = await _studentSubjectService.AddStudentSubject(dto);
            if (result.Success == true)
                return Ok(result);
            else
                return BadRequest(result);
        }

        // DELETE: api/StudentSubjects/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralServiceResponse>> DeleteStudentSubject(int id)
        {
            var result = await _studentSubjectService.DeleteStudentSubject(id);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GeneralServiceResponse>> UpdateStudentSubject([FromBody] StudentSubjectsDto dto)
        {
            var result = await _studentSubjectService.UpdateStudentSubjects(dto);
            if (result.Success == true)
                return Ok(result);
            return NotFound(result);
        }


    }
}
