using Capstone.Core.Entities;
using Capstone.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentRepository repo;

        public DepartmentsController(IDepartmentRepository repo)
        {
            this.repo = repo;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await repo.GetAllAsync();
            if (!result.Any())
            {
                return NotFound("There is no Department available.");
            }
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] Department department)
        {
            var result = await repo.CreateAsync(department);
            return Ok(result);
        }
    }
}
