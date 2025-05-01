using Capstone.Core.Entities;
using Capstone.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository repo;

        public StudentsController(IStudentRepository repo)
        {
            this.repo = repo;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await repo.GetAllStudentsAsync();
            if (result == null) { return NotFound(); }
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] Student student)
        {
            var result = await repo.CreateStudentAsync(student);
            return Ok(result);
        }
    }
}
