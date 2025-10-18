using BackendApi.Context;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizzesController : ControllerBase
    {
        private readonly IGradeCalculationService _gradeCalculationService;
        private readonly AppDbContext _dbContext;
        private readonly IAuthRepository _authRepo;

        public QuizzesController(IGradeCalculationService gradeCalculationService, AppDbContext dbContext, IAuthRepository authRepo)
        {
            _gradeCalculationService = gradeCalculationService;
            _dbContext = dbContext;
            _authRepo = authRepo;
        }
        [HttpPost("add-quiz")]
        public async Task<IActionResult> AddQuizAsync([FromBody] AddQuizInputDto input)
        {
            if (input == null || input.StudentScores == null || !input.StudentScores.Any())
                return BadRequest("Invalid input data.");

            var currentUser = await _authRepo.GetCurrentUserAsync();
            if (currentUser.Role != UserRole.Teacher)
                return Forbid("Only teachers can add quizzes.");

            var results = new List<object>();

            foreach (var entry in input.StudentScores)
            {
                // ✅ Add quiz (through new method)
                var added = await _gradeCalculationService.AddQuizToMidtermGradeAsync(
                    entry.StudentId,
                    entry.MidtermGradeId,
                    input.QuizLabel,
                    entry.QuizScore,
                    input.QuizTotal
                );

                if (!added)
                {
                    results.Add(new
                    {
                        entry.StudentId,
                        Message = "Failed to add quiz. Midterm grade not found or quiz already exists."
                    });
                    continue;
                }

                // ✅ Recompute midterm grade
                var recomputeDto = new MidtermGradeDto
                {
                    StudentId = entry.StudentId,
                    SubjectId = input.SubjectId,
                    AcademicPeriodId = input.AcademicPeriodId
                };

                var updatedGrade = await _gradeCalculationService.CalculateAndSaveSingleMidtermGradeAsync(recomputeDto);

                results.Add(new
                {
                    StudentId = entry.StudentId,
                    QuizLabel = input.QuizLabel,
                    NewFinalGrade = updatedGrade.TotalMidtermGradeRounded,
                    Message = "Quiz added and midterm grade recalculated."
                });
            }

            return Ok(results);
        }

    }
}
