using Microsoft.EntityFrameworkCore;
using TeamTasks.Domain.Entities;
using TeamTasks.Domain.Interfaces;
using TeamTasks.Infrastructure.DbContext;
using TaskStatus = TeamTasks.Domain.Enums.TaskStatus;

namespace TeamTasks.Infrastructure.Repositories
{
    public class TaskRepository : RepositoryBase<TaskItem>, ITaskRepository
    {
        public TaskRepository(TeamTasksDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TaskItem>> GetByProjectIdAsync(
            int projectId,
            TaskStatus? status = null,
            int? assigneeId = null)
        {
            var query = _dbSet
                .Include(t => t.Assignee)
                .Where(t => t.ProjectId == projectId);

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            if (assigneeId.HasValue)
                query = query.Where(t => t.AssigneeId == assigneeId.Value);

            return await query
                .OrderBy(t => t.DueDate)
                .ThenBy(t => t.Priority)
                .ToListAsync();
        }

        public async Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetByProjectIdPagedAsync(
            int projectId,
            int page,
            int pageSize,
            TaskStatus? status = null,
            int? assigneeId = null)
        {
            var query = _dbSet
                .Include(t => t.Assignee)
                .Where(t => t.ProjectId == projectId);

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            if (assigneeId.HasValue)
                query = query.Where(t => t.AssigneeId == assigneeId.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(t => t.DueDate)
                .ThenByDescending(t => t.Priority)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<TaskItem?> GetByIdWithDetailsAsync(int taskId)
        {
            return await _dbSet
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .FirstOrDefaultAsync(t => t.TaskId == taskId);
        }
    }
}
