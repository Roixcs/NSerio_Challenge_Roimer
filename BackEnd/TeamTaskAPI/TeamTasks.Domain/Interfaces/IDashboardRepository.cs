namespace TeamTasks.Domain.Interfaces
{
    public interface IDashboardRepository
    {
        Task<IEnumerable<DeveloperWorkloadDto>> GetDeveloperWorkloadAsync();
        Task<IEnumerable<ProjectHealthDto>> GetProjectHealthAsync();
        Task<IEnumerable<DeveloperDelayRiskDto>> GetDeveloperDelayRiskAsync();
        Task<IEnumerable<TaskDueSoonDto>> GetTasksDueSoonAsync();
    }

    // DTOs for dashboard queries
    public record DeveloperWorkloadDto(
        int DeveloperId,
        string DeveloperName,
        string Email,
        int OpenTasksCount,
        decimal AverageEstimatedComplexity
    );

    public record ProjectHealthDto(
        int ProjectId,
        string ProjectName,
        string ClientName,
        string ProjectStatus,
        int TotalTasks,
        int OpenTasks,
        int CompletedTasks
    );

    public record DeveloperDelayRiskDto(
        int DeveloperId,
        string DeveloperName,
        string Email,
        int OpenTasksCount,
        decimal AvgDelayDays,
        DateTime? NearestDueDate,
        DateTime? LatestDueDate,
        DateTime? PredictedCompletionDate,
        int HighRiskFlag
    );

    public record TaskDueSoonDto(
        int TaskId,
        string Title,
        string? Description,
        string ProjectName,
        string? AssigneeName,
        string Status,
        string Priority,
        int EstimatedComplexity,
        DateTime DueDate,
        int DaysUntilDue
    );
}
