using Capstone.Core.Entities;
using Capstone.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentsController : ControllerBase
    {
        private readonly IAssessmentRepository _repo;

        public AssessmentsController(IAssessmentRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAssessments()
        {
            var result = await _repo.GetAllAsync();
            if (!result.Any())
            {
                return NotFound("Assessment not found");
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAssessment([FromBody] Assessment assessment)
        {
            var result = await _repo.CreateAsync(assessment);
            return Ok(result);
        }


    }
}
