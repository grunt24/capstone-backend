using Capstone.Core.Entities;
using Capstone.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoresController : ControllerBase
    {
        private readonly IScoreRepository repo;

        public ScoresController(IScoreRepository repo)
        {
            this.repo = repo;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await repo.GetAllScoresAsync();
            if (result == null) { return NotFound(); }
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] Score score)
        {
            var result = await repo.CreateScoreAsync(score);
            return Ok(result);
        }
    }
}
