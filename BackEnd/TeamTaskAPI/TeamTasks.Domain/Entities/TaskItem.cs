using TeamTasks.Domain.Enums;
using TaskStatus = TeamTasks.Domain.Enums.TaskStatus;

namespace TeamTasks.Domain.Entities
{
    public class TaskItem
    {
        public int TaskId { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? AssigneeId { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.ToDo;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public int EstimatedComplexity { get; set; } = 3;
        public DateTime? DueDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Project Project { get; set; } = null!;
        public Developer? Assignee { get; set; }
    }
}
