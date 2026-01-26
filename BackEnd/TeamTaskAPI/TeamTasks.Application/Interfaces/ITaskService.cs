using TeamTasks.Application.Dtos;

namespace TeamTasks.Application.Interfaces
{
    public interface ITaskService
    {
        Task<PaginatedResult<TaskDto>> GetTasksByProjectAsync(
            int projectId,
            int page,
            int pageSize,
            string? status = null,
            int? assigneeId = null);

        Task<TaskDetailDto?> GetTaskByIdAsync(int taskId);
        Task<TaskDetailDto> CreateTaskAsync(CreateTaskDto dto);
        Task<TaskDetailDto?> UpdateTaskStatusAsync(int taskId, UpdateTaskStatusDto dto);
    }
}
