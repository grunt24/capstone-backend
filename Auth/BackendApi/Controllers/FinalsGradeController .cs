using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.GradeCalculation
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinalsGradeController : ControllerBase
    {
        private readonly IGradeCalculationService _gradeCalculationService;

        public FinalsGradeController(IGradeCalculationService gradeCalculationService)
        {
            _gradeCalculationService = gradeCalculationService;
        }

        // ✅ Get all finals grades by subject + period
        [HttpGet("{subjectId:int}/{academicPeriodId:int}")]
        public async Task<IActionResult> GetGradesBySubjectAndPeriod(int subjectId, int academicPeriodId)
        {
            var grades = await _gradeCalculationService.GetFinalsGradesBySubjectAndPeriodAsync(subjectId, academicPeriodId);
            return Ok(grades);
        }

        // ✅ Update finals grade (PUT)
        [HttpPut("{studentId:int}")]
        public async Task<IActionResult> UpdateFinalsGrade(int studentId, [FromBody] FinalsGradeDto updatedGrade)
        {
            if (updatedGrade == null)
                return BadRequest("Invalid grade data.");

            var result = await _gradeCalculationService.UpdateFinalsGradeAsync(studentId, updatedGrade);

            if (!result)
                return NotFound($"Student ID {studentId} not found or update failed.");

            return Ok(new { message = "Finals grade updated successfully." });
        }
    }
}
