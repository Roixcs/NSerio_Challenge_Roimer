using TeamTasks.Application.Dtos;
using TeamTasks.Application.Interfaces;
using TeamTasks.Domain.Interfaces;
using TeamTasks.Domain.Entities;

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

        public async Task<Result<IEnumerable<ProjectDto>>> GetProjectsWithStatsAsync()
        {
            var projectHealth = await _dashboardRepository.GetProjectHealthAsync();

            var dtos = projectHealth.Select(p => new ProjectDto(
                p.ProjectId,
                p.ProjectName,
                p.ClientName,
                p.ProjectStatus,
                p.TotalTasks,
                p.OpenTasks,
                p.CompletedTasks
            ));

            return Result<IEnumerable<ProjectDto>>.Success(dtos);
        }

        public async Task<Result<ProjectDetailDto>> GetProjectByIdAsync(int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
                return Result<ProjectDetailDto>.Failure($"Project with ID {projectId} not found.");

            var dto = new ProjectDetailDto(
                project.ProjectId,
                project.Name,
                project.ClientName,
                project.StartDate,
                project.EndDate,
                project.Status.ToString()
            );

            return Result<ProjectDetailDto>.Success(dto);
        }
    }
}
