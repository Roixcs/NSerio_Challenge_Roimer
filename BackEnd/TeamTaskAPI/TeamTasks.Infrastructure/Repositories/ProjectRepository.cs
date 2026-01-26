using Microsoft.EntityFrameworkCore;
using TeamTasks.Domain.Entities;
using TeamTasks.Domain.Interfaces;
using TeamTasks.Infrastructure.DbContext;

namespace TeamTasks.Infrastructure.Repositories
{
    public class ProjectRepository : RepositoryBase<Project>, IProjectRepository
    {
        public ProjectRepository(TeamTasksDbContext context) : base(context)
        {
        }

        public async Task<Project?> GetByIdWithTasksAsync(int projectId)
        {
            return await _dbSet
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.Assignee)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);
        }

        public async Task<bool> ExistsAsync(int projectId)
        {
            return await _dbSet.AnyAsync(p => p.ProjectId == projectId);
        }
    }
}
