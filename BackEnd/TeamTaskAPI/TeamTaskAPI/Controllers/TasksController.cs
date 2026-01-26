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
        /// Get a specific task by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);

            if (task == null)
                return NotFound(new { message = $"Task with ID {id} not found." });

            return Ok(task);
        }

        /// <summary>
        /// Create a new task
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
        {
            var task = await _taskService.CreateTaskAsync(dto);
            return CreatedAtAction(nameof(GetTask), new { id = task.TaskId }, task);
        }

        /// <summary>
        /// Update task status (and optionally priority/complexity)
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusDto dto)
        {
            var task = await _taskService.UpdateTaskStatusAsync(id, dto);

            if (task == null)
                return NotFound(new { message = $"Task with ID {id} not found." });

            return Ok(task);
        }
    }
}
