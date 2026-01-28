using TeamTasks.Domain.Interfaces;
using TeamTasks.Domain.Entities;

namespace TeamTasks.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<Result<IEnumerable<DeveloperWorkloadDto>>> GetDeveloperWorkloadAsync();
        Task<Result<IEnumerable<ProjectHealthDto>>> GetProjectHealthAsync();
        Task<Result<IEnumerable<DeveloperDelayRiskDto>>> GetDeveloperDelayRiskAsync();
    }
}
