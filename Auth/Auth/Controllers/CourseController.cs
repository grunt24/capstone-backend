using Capstone.Core.Entities;
using Capstone.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseRepository courseRepo;

        public CourseController(ICourseRepository courseRepo)
        {
            this.courseRepo = courseRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var result = await courseRepo.GetAllAsync();

            if (!result.Any())
                return NotFound("No courses available.");

            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] Course course)
        {
            var result = await courseRepo.CreateAsync(course);
            return Ok(result);
        }
    }
}
