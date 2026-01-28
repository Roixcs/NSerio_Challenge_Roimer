using Microsoft.AspNetCore.Mvc;
using TeamTasks.Application.Interfaces;

namespace TeamTaskAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Get workload summary per active developer
        /// </summary>
        [HttpGet("developer-workload")]
        public async Task<IActionResult> GetDeveloperWorkload()
        {
            var result = await _dashboardService.GetDeveloperWorkloadAsync();
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Get health summary per project (task counts)
        /// </summary>
        [HttpGet("project-health")]
        public async Task<IActionResult> GetProjectHealth()
        {
            var result = await _dashboardService.GetProjectHealthAsync();
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Get delay risk prediction per developer
        /// </summary>
        [HttpGet("developer-delay-risk")]
        public async Task<IActionResult> GetDeveloperDelayRisk()
        {
            var result = await _dashboardService.GetDeveloperDelayRiskAsync();
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
    }

}