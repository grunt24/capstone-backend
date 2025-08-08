using BackendApi.Core.General;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using BackendApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectRepository _subjectService;

        public SubjectsController(ISubjectRepository subjectService)
        {
            _subjectService = subjectService;
        }

        // GET: api/Subjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectWithTeacherDto>>> GetAllSubjects()
        {
            var subjects = await _subjectService.GetAllSubjects();
            return Ok(subjects);
        }

        // GET: api/Subjects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Subject>> GetSubjectById(int id)
        {
            var subject = await _subjectService.GetSubjectById(id);
            if (subject == null)
            {
                return NotFound(new GeneralServiceResponse
                {
                    Success = false,
                    Message = "Subject not found"
                });
            }

            return Ok(subject);
        }

        // POST: api/Subjects
        [HttpPost]
        public async Task<ActionResult<GeneralServiceResponse>> CreateSubject([FromBody] SubjectDto subjectDto)
        {
            var result = await _subjectService.CreateSubject(subjectDto);
            return Ok(result);
        }

        // PUT: api/Subjects/5
        [HttpPut("{id}")]
        public async Task<ActionResult<GeneralServiceResponse>> UpdateSubject(int id, [FromBody] SubjectDto subjectDto)
        {
            var result = await _subjectService.UpdateSubject(id, subjectDto);
            return Ok(result);
        }

        // DELETE: api/Subjects/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralServiceResponse>> DeleteSubject(int id)
        {
            var result = await _subjectService.DeleteSubject(id);
            return Ok(result);
        }
    }
}
