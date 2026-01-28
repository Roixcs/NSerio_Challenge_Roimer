using Microsoft.AspNetCore.Mvc;
using TeamTasks.Application.Dtos;
using TeamTasks.Application.Interfaces;

namespace TeamTaskAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// Get task by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var result = await _taskService.GetTaskByIdAsync(id);

            // Return full Result object for consistency
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
        {
            var result = await _taskService.CreateTaskAsync(dto);
            
            // Return full Result object
            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetTask), new { id = result.Data!.TaskId }, result);
        }

        /// <summary>
        /// Update task status (and optionally priority/complexity)
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusDto dto)
        {
            var result = await _taskService.UpdateTaskStatusAsync(id, dto);

            // Return full Result object
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
