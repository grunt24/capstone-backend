using Capstone.Core.Entities;
using Capstone.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherSubjectsController : ControllerBase
    {
        private readonly ITeacherSubjectRepository repo;

        public TeacherSubjectsController(ITeacherSubjectRepository repo)
        {
            this.repo = repo;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await repo.GetAllAsync();
            if (result == null) { return NotFound(); }
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] TeacherSubject obj)
        {
            var result = await repo.CreateAsync(obj);
            return Ok(result);
        }
    }
}
