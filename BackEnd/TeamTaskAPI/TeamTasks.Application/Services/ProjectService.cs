using TeamTasks.Application.Dtos;
using TeamTasks.Application.Interfaces;
using TeamTasks.Domain.Interfaces;

namespace TeamTasks.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IDashboardRepository _dashboardRepository;

        public ProjectService(
            IProjectRepository projectRepository,
            IDashboardRepository dashboardRepository)
        {
            _projectRepository = projectRepository;
            _dashboardRepository = dashboardRepository;
        }

        public async Task<IEnumerable<ProjectDto>> GetProjectsWithStatsAsync()
        {
            var projectHealth = await _dashboardRepository.GetProjectHealthAsync();

            return projectHealth.Select(p => new ProjectDto(
                p.ProjectId,
                p.ProjectName,
                p.ClientName,
                p.ProjectStatus,
                p.TotalTasks,
                p.OpenTasks,
                p.CompletedTasks
            ));
        }

        public async Task<ProjectDetailDto?> GetProjectByIdAsync(int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
                return null;

            return new ProjectDetailDto(
                project.ProjectId,
                project.Name,
                project.ClientName,
                project.StartDate,
                project.EndDate,
                project.Status.ToString()
            );
        }
    }
}
