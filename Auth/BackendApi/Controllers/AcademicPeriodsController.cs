using BackendApi.Context;
using BackendApi.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicPeriodsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public AcademicPeriodsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentAcademicPeriod()
        {
            var current = await _dbContext.AcademicPeriods
                .FirstOrDefaultAsync(p => p.IsCurrent);

            if (current == null)
                return NotFound("No current academic period set.");

            return Ok(new
            {
                startYear = current.StartYear,
                endYear = current.EndYear,
                semester = current.Semester,
                academicYear = $"{current.StartYear}-{current.EndYear}"
            });
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAcademicPeriod()
        {
            var periods = await _dbContext.AcademicPeriods
                .OrderByDescending(p => p.StartYear)
                .ThenByDescending(p => p.Semester)
                .ToListAsync();

            return Ok(periods);
        }

        [HttpPost("set-current")]
        public async Task<IActionResult> SetCurrentAcademicPeriod([FromBody] AcademicPeriodDto dto)
        {
            // Optional: validate input, check for overlaps

            // Unset previous current
            var existing = await _dbContext.AcademicPeriods
                .Where(p => p.IsCurrent)
                .ToListAsync();

            existing.ForEach(p => p.IsCurrent = false);

            var newPeriod = new AcademicPeriod
            {
                StartYear = dto.StartYear,
                EndYear = dto.EndYear,
                Semester = dto.Semester,
                IsCurrent = true
            };

            _dbContext.AcademicPeriods.Add(newPeriod);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Academic period set successfully." });
        }

        public class AcademicPeriodDto
        {
            public int StartYear { get; set; }
            public int EndYear { get; set; }
            public string Semester { get; set; }
        }
    }
}
