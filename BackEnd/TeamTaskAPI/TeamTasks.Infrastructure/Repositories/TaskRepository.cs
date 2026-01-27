using Microsoft.EntityFrameworkCore;
using TeamTasks.Domain.Entities;
using TeamTasks.Domain.Interfaces;
using TeamTasks.Infrastructure.DbContext;
using TaskStatus = TeamTasks.Domain.Enums.TaskStatus;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace TeamTasks.Infrastructure.Repositories
{
    public class TaskRepository : RepositoryBase<TaskItem>, ITaskRepository
    {
        private readonly string _connectionString;

        public TaskRepository(TeamTasksDbContext context, IConfiguration configuration) : base(context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
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

        public async Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetByProjectIdPagedAsync(int projectId, int page, int pageSize, TaskStatus? status = null, int? assigneeId = null)
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

        public async Task<TaskItem> AddWithSPAsync(TaskItem task)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var parameters = new DynamicParameters();
            parameters.Add("@ProjectId", task.ProjectId);
            parameters.Add("@Title", task.Title);
            parameters.Add("@Description", task.Description);
            parameters.Add("@AssigneeId", task.AssigneeId);
            parameters.Add("@Status", task.Status.ToString());
            parameters.Add("@Priority", task.Priority.ToString());
            parameters.Add("@EstimatedComplexity", task.EstimatedComplexity);
            parameters.Add("@DueDate", task.DueDate);
            parameters.Add("@NewTaskId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("TaskManagement.sp_InsertTask", parameters, commandType: CommandType.StoredProcedure);

            var newId = parameters.Get<int>("@NewTaskId");
            
            // Return full entity with details using EF
            return await GetByIdWithDetailsAsync(newId) ?? task;
        }

        public async Task<TaskItem?> UpdateStatusWithSPAsync(int taskId, string status, string? priority = null, int? complexity = null)
        {
            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@TaskId", taskId);
            parameters.Add("@Status", status);
            parameters.Add("@Priority", priority);
            parameters.Add("@EstimatedComplexity", complexity);
            parameters.Add("@UpdatedTaskId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("TaskManagement.sp_UpdateTaskStatus", parameters, commandType: CommandType.StoredProcedure);

            // Fetch updated entity via EF to return consistent object graph
            return await GetByIdWithDetailsAsync(taskId);
        }
    }
}
