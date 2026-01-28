using TeamTasks.Application.Dtos;
using TeamTasks.Domain.Entities;

namespace TeamTasks.Application.Interfaces
{
    public interface ITaskService
    {
        Task<Result<PaginatedResult<TaskDto>>> GetTasksByProjectAsync(int projectId, int page, int pageSize, string? status = null, int? assigneeId = null);
        Task<Result<TaskDetailDto>> GetTaskByIdAsync(int taskId);
        Task<Result<TaskDetailDto>> CreateTaskAsync(CreateTaskDto dto);
        Task<Result<TaskDetailDto>> UpdateTaskStatusAsync(int taskId, UpdateTaskStatusDto dto);
    }
}
