using TeamTasks.Domain.Entities;

namespace TeamTasks.Domain.Interfaces
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project?> GetByIdWithTasksAsync(int projectId);
        Task<bool> ExistsAsync(int projectId);
    }
}
