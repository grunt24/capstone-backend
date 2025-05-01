using Capstone.Core.Entities;
using Capstone.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherRepository repo;

        public TeachersController(ITeacherRepository repo)
        {
            this.repo = repo;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await repo.GetAllTeachersAsync();
            if (result == null) { return NotFound(); }
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] Teacher teacher)
        {
            var result = await repo.CreateTeacherAsync(teacher);
            return Ok(result);
        }
    }
}
