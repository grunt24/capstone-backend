// BackendApi.Controllers/GradesController.cs
using BackendApi.Core.General;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Teacher,Superadmin,Admin")]
    public class GradesController : ControllerBase
    {
        private readonly IGradeRepository _gradeService;

        public GradesController(IGradeRepository gradeService)
        {
            _gradeService = gradeService;
        }

        [HttpPost("save")]
        public async Task<ActionResult<GeneralServiceResponse>> SaveGrades([FromBody] List<SaveGradesDto> saveGradesDto)
        {
            var result = await _gradeService.SaveGradesAsync(saveGradesDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("subject/{subjectId}")]
        public async Task<ActionResult<IEnumerable<GradeDto>>> GetGradesBySubject(int subjectId)
        {
            var grades = await _gradeService.GetGradesBySubjectIdAsync(subjectId);
            if (grades == null)
            {
                return NotFound(new GeneralServiceResponse { Success = false, Message = "Grades not found for this subject." });
            }
            return Ok(grades);
        }
    }
}
