using TeamTasks.Application.Dtos;

namespace TeamTasks.Application.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetProjectsWithStatsAsync();
        Task<ProjectDetailDto?> GetProjectByIdAsync(int projectId);
    }
}
