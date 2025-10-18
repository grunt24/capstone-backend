using BackendApi.Context;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/enrollments")]
public class EnrollmentController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public EnrollmentController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("enroll-current")]
    public async Task<IActionResult> EnrollStudentsInCurrentSemester([FromBody] EnrollStudentsRequest request)
    {
        if (request.StudentIds == null || !request.StudentIds.Any())
        {
            return BadRequest("No student IDs provided.");
        }

        // ✅ Get current academic period
        var currentPeriod = await _dbContext.AcademicPeriods
            .FirstOrDefaultAsync(p => p.IsCurrent == true);

        if (currentPeriod == null)
        {
            return BadRequest("No current academic period is set.");
        }

        var enrolled = new List<int>();
        var skipped = new List<object>();

        foreach (var studentId in request.StudentIds)
        {
            var student = await _dbContext.Users.FindAsync(studentId);
            if (student == null)
            {
                skipped.Add(new { studentId, reason = "Student not found." });
                continue;
            }

            bool alreadyEnrolled = await _dbContext.StudentEnrollments.AnyAsync(e =>
                e.StudentId == studentId &&
                e.AcademicPeriodId == currentPeriod.Id);

            if (alreadyEnrolled)
            {
                skipped.Add(new { studentId, reason = "Already enrolled." });
                continue;
            }

            var enrollment = new StudentEnrollment
            {
                StudentId = studentId,
                AcademicPeriodId = currentPeriod.Id,
                IsEnrolled = true,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.StudentEnrollments.Add(enrollment);
            enrolled.Add(studentId);
        }

        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Enrollment process completed.",
            academicYear = currentPeriod.AcademicYear,
            semester = currentPeriod.Semester,
            enrolled,
            skipped
        });
    }
    [HttpGet("enrolled-current")]
    public async Task<IActionResult> GetEnrolledStudentsForCurrentSemester()
    {
        // ✅ Get current academic year + semester
        var currentPeriod = await _dbContext.AcademicPeriods
            .FirstOrDefaultAsync(p => p.IsCurrent);

        if (currentPeriod == null)
        {
            return NotFound("No current academic period is set.");
        }

        var enrolledStudents = await _dbContext.StudentEnrollments
            .Where(e => e.AcademicPeriodId == currentPeriod.Id && e.IsEnrolled)
            .Include(e => e.Student)
            .Select(e => new
            {
                e.Student.Id,
                e.Student.StudentNumber,
                e.Student.Fullname,
                e.Student.Department,
                e.Student.YearLevel,
                AcademicYear = currentPeriod.AcademicYear,
                Semester = currentPeriod.Semester,
                EnrolledAt = e.CreatedAt
            })
            .ToListAsync();

        return Ok(enrolledStudents);
    }

}
