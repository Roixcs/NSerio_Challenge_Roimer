using TeamTasks.Domain.Interfaces;

namespace TeamTasks.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<IEnumerable<DeveloperWorkloadDto>> GetDeveloperWorkloadAsync();
        Task<IEnumerable<ProjectHealthDto>> GetProjectHealthAsync();
        Task<IEnumerable<DeveloperDelayRiskDto>> GetDeveloperDelayRiskAsync();
    }
}
