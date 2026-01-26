using TeamTasks.Application.Dtos;

namespace TeamTasks.Application.Interfaces
{
    public interface IDeveloperService
    {
        Task<IEnumerable<DeveloperDto>> GetActiveDevelopersAsync();
    }
}
