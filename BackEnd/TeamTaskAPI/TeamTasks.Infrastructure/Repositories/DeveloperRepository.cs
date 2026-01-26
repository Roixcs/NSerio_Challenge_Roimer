using Microsoft.EntityFrameworkCore;
using TeamTasks.Domain.Entities;
using TeamTasks.Domain.Interfaces;
using TeamTasks.Infrastructure.DbContext;

namespace TeamTasks.Infrastructure.Repositories
{
    public class DeveloperRepository : RepositoryBase<Developer>, IDeveloperRepository
    {
        public DeveloperRepository(TeamTasksDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Developer>> GetActiveDevelopersAsync()
        {
            return await _dbSet
                .Where(d => d.IsActive)
                .OrderBy(d => d.FirstName)
                .ThenBy(d => d.LastName)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int developerId)
        {
            return await _dbSet.AnyAsync(d => d.DeveloperId == developerId);
        }

        public async Task<bool> IsActiveAsync(int developerId)
        {
            return await _dbSet.AnyAsync(d => d.DeveloperId == developerId && d.IsActive);
        }
    }
}
