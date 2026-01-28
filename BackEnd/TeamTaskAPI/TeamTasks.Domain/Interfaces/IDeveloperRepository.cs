using TeamTasks.Domain.Entities;

namespace TeamTasks.Domain.Interfaces
{
    public interface IDeveloperRepository : IRepository<Developer>
    {
        Task<IEnumerable<Developer>> GetActiveDevelopersAsync();
        Task<bool> ExistsAsync(int developerId);
        Task<bool> IsActiveAsync(int developerId);
    }
}