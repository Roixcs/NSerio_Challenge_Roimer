using TeamTasks.Application.Dtos;
using TeamTasks.Domain.Entities;

namespace TeamTasks.Application.Interfaces
{
    public interface IProjectService
    {
        Task<Result<IEnumerable<ProjectDto>>> GetProjectsWithStatsAsync();
        Task<Result<ProjectDetailDto>> GetProjectByIdAsync(int projectId);
    }
}
