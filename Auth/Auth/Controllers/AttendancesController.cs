using Capstone.Core.Entities;
using Capstone.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendancesController : ControllerBase
    {
        private readonly IAttendanceRepository repo;

        public AttendancesController(IAttendanceRepository repo)
        {
            this.repo = repo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAttendance()
        {
            var result = await repo.GetAllAsync();

            if (result == null)
            {
                {
                    return NotFound("There is no Attendance");
                }
            }
            return Ok(result);  
        }

        [HttpPost]
        public async Task<IActionResult> CreateAttendance([FromBody] Attendance attendance)
        {
            var result = await repo.CreateAsync(attendance);
            return Ok(result);
        }
    }
}
