using Microsoft.AspNetCore.Mvc;
using TeamTasks.Application.Interfaces;

namespace TeamTaskAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevelopersController : ControllerBase
    {
        private readonly IDeveloperService _developerService;

        public DevelopersController(IDeveloperService developerService)
        {
            _developerService = developerService;
        }

        /// <summary>
        /// Get all active developers
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetActiveDevelopers()
        {
            var developers = await _developerService.GetActiveDevelopersAsync();
            return Ok(developers);
        }
    }

}

