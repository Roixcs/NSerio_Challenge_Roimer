namespace TeamTasks.Application.Dtos
{
    public record ProjectDto(int ProjectId,
        string Name,
        string ClientName,
        string Status,
        int TotalTasks,
        int OpenTasks,
        int CompletedTasks
    );

    public record ProjectDetailDto(int ProjectId,
        string Name,
        string ClientName,
        DateTime StartDate,
        DateTime? EndDate,
        string Status
    );
}
