namespace TeamTasks.Application.Dtos
{
    public record TaskDto(
        int TaskId,
        int ProjectId,
        string Title,
        string? Description,
        int? AssigneeId,
        string? AssigneeName,
        string Status,
        string Priority,
        int EstimatedComplexity,
        DateTime? DueDate,
        DateTime? CompletionDate,
        DateTime CreatedAt
    );

    public record TaskDetailDto(
        int TaskId,
        int ProjectId,
        string ProjectName,
        string Title,
        string? Description,
        int? AssigneeId,
        string? AssigneeName,
        string Status,
        string Priority,
        int EstimatedComplexity,
        DateTime? DueDate,
        DateTime? CompletionDate,
        DateTime CreatedAt
    );

    public record CreateTaskDto(
        int ProjectId,
        string Title,
        string? Description,
        int? AssigneeId,
        string Status,
        string Priority,
        int EstimatedComplexity,
        DateTime? DueDate
    );

    public record UpdateTaskStatusDto(
        string Status,
        string? Priority = null,
        int? EstimatedComplexity = null
    );
}
