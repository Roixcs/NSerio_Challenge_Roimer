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
            var workload = await _dashboardService.GetDeveloperWorkloadAsync();
            return Ok(workload);
        }

        /// <summary>
        /// Get health summary per project (task counts)
        /// </summary>
        [HttpGet("project-health")]
        public async Task<IActionResult> GetProjectHealth()
        {
            var health = await _dashboardService.GetProjectHealthAsync();
            return Ok(health);
        }

        /// <summary>
        /// Get delay risk prediction per developer
        /// </summary>
        [HttpGet("developer-delay-risk")]
        public async Task<IActionResult> GetDeveloperDelayRisk()
        {
            var risk = await _dashboardService.GetDeveloperDelayRiskAsync();
            return Ok(risk);
        }
    }

}