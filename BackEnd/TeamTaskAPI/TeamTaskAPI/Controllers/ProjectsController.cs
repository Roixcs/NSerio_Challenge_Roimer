using Microsoft.AspNetCore.Mvc;
using TeamTasks.Application.Interfaces;

namespace TeamTaskAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ITaskService _taskService;

        public ProjectsController(IProjectService projectService, ITaskService taskService)
        {
            _projectService = projectService;
            _taskService = taskService;
        }

        /// <summary>
        /// Get all projects with task statistics
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var result = await _projectService.GetProjectsWithStatsAsync();
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }


        [HttpGet("{id}/tasks")]
        public async Task<IActionResult> GetProjectTasks(int id, [FromQuery] string? status = null, [FromQuery] int? assigneeId = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // Validate pagination
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50;

            var result = await _taskService.GetTasksByProjectAsync(id, page, pageSize, status, assigneeId);
            
            if (!result.IsSuccess) return BadRequest(result);
            
            return Ok(result);
        }
    }
}
