using TeamTasks.Application.Interfaces;
using TeamTasks.Domain.Interfaces;
using TeamTasks.Domain.Entities;

namespace TeamTasks.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository) => _dashboardRepository = dashboardRepository;
        

        public async Task<Result<IEnumerable<DeveloperWorkloadDto>>> GetDeveloperWorkloadAsync()
        {
            var result = await _dashboardRepository.GetDeveloperWorkloadAsync();
            return Result<IEnumerable<DeveloperWorkloadDto>>.Success(result);
        }

        public async Task<Result<IEnumerable<ProjectHealthDto>>> GetProjectHealthAsync()
        {
            var result = await _dashboardRepository.GetProjectHealthAsync();
            return Result<IEnumerable<ProjectHealthDto>>.Success(result);
        }

        public async Task<Result<IEnumerable<DeveloperDelayRiskDto>>> GetDeveloperDelayRiskAsync()
        {
            var result = await _dashboardRepository.GetDeveloperDelayRiskAsync();
            return Result<IEnumerable<DeveloperDelayRiskDto>>.Success(result);
        }
    }
}
