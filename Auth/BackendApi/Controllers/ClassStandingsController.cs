using BackendApi.Context;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassStandingsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IGradeCalculationService _gradeCalculationService;
        private readonly IAuthRepository _authRepo;

        public ClassStandingsController(AppDbContext dbContext, IGradeCalculationService gradeCalculationService, IAuthRepository authRepo)
        {
            _dbContext = dbContext;
            _gradeCalculationService = gradeCalculationService;
            _authRepo = authRepo;
        }

        [HttpPost("add-class-standing")]
        public async Task<ActionResult> AddClassStandingAsync([FromBody] AddClassStandingInputDto input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Label) || input.StudentScores == null || !input.StudentScores.Any())
                return BadRequest("Invalid input data. Please provide label, subject, and student scores.");

            var currentUser = await _authRepo.GetCurrentUserAsync();
            if (currentUser.Role != UserRole.Teacher)
                return Forbid("Only teachers can add class standing items.");

            // Step 1: Get the Teacher entity based on current user ID
            var teacher = await _dbContext.Teachers
                .FirstOrDefaultAsync(t => t.UserID == currentUser.Id);

            if (teacher == null)
                return NotFound("Teacher record not found for the current user.");

            // Step 2: Find the subject assigned to this teacher
            var subject = await _dbContext.Subjects
                .FirstOrDefaultAsync(s => s.Id == input.SubjectId && s.TeacherId == teacher.Id);

            if (subject == null)
                return NotFound("Subject not found or you are not assigned to this subject.");

            var results = new List<object>();

            foreach (var entry in input.StudentScores)
            {
                var added = await _gradeCalculationService.AddClassStandingToMidtermGradeAsync(
                    entry.StudentId,
                    entry.MidtermGradeId,
                    input.Label,
                    entry.Score,
                    input.Total
                );

                if (!added)
                {
                    results.Add(new
                    {
                        StudentId = entry.StudentId,
                        Label = input.Label,
                        Message = "Failed to add class standing item. Midterm grade not found or item already exists."
                    });
                    continue;
                }

                results.Add(new
                {
                    StudentId = entry.StudentId,
                    Label = input.Label,
                    Message = "Class standing item added successfully."
                });
            }

            return Ok(results);
        }
    }
}
