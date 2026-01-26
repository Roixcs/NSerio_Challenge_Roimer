using TeamTasks.Domain.Entities;
using TaskStatus = TeamTasks.Domain.Enums.TaskStatus;

namespace TeamTasks.Domain.Interfaces
{
    public interface ITaskRepository : IRepository<TaskItem>
    {
        Task<IEnumerable<TaskItem>> GetByProjectIdAsync(int projectId, TaskStatus? status = null, int? assigneeId = null);
        Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetByProjectIdPagedAsync(
            int projectId,
            int page,
            int pageSize,
            TaskStatus? status = null,
            int? assigneeId = null);
        Task<TaskItem?> GetByIdWithDetailsAsync(int taskId);
    }
}
