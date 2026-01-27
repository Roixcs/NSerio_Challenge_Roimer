using TeamTasks.Application.Dtos;
using TeamTasks.Domain.Entities;

namespace TeamTasks.Application.Interfaces
{
    public interface IDeveloperService
    {
        Task<Result<IEnumerable<DeveloperDto>>> GetActiveDevelopersAsync();
    }
}
