using TeamTasks.Application.Dtos;
using TeamTasks.Application.Interfaces;
using TeamTasks.Application.Validators;
using TeamTasks.Domain.Entities;
using TeamTasks.Domain.Enums;
using TeamTasks.Domain.Interfaces;
using TaskStatus = TeamTasks.Domain.Enums.TaskStatus;

namespace TeamTasks.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IDeveloperRepository _developerRepository;

        public TaskService(
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            IDeveloperRepository developerRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _developerRepository = developerRepository;
        }

        public async Task<PaginatedResult<TaskDto>> GetTasksByProjectAsync( int projectId, int page, int pageSize, string? status = null, int? assigneeId = null)
        {
            TaskStatus? taskStatus = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<TaskStatus>(status, true, out var parsedStatus))
            {
                taskStatus = parsedStatus;
            }

            var (items, totalCount) = await _taskRepository.GetByProjectIdPagedAsync(projectId, page, pageSize, taskStatus, assigneeId);

            var dtos = items.Select(t => new TaskDto(
                t.TaskId,
                t.ProjectId,
                t.Title,
                t.Description,
                t.AssigneeId,
                t.Assignee?.FullName,
                t.Status.ToString(),
                t.Priority.ToString(),
                t.EstimatedComplexity,
                t.DueDate,
                t.CompletionDate,
                t.CreatedAt
            ));

            return new PaginatedResult<TaskDto>(dtos, totalCount, page, pageSize);
        }

        public async Task<TaskDetailDto?> GetTaskByIdAsync(int taskId)
        {
            var task = await _taskRepository.GetByIdWithDetailsAsync(taskId);

            if (task == null)
                return null;

            return MapToDetailDto(task);
        }

        public async Task<TaskDetailDto> CreateTaskAsync(CreateTaskDto dto)
        {
            // Validate
            var validationResult = await TaskValidator.ValidateCreateAsync(dto, _projectRepository, _developerRepository);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Parse enums
            var status = Enum.Parse<TaskStatus>(dto.Status, true);
            var priority = Enum.Parse<TaskPriority>(dto.Priority, true);

            // Create entity
            var task = new TaskItem
            {
                ProjectId = dto.ProjectId,
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim(),
                AssigneeId = dto.AssigneeId,
                Status = status,
                Priority = priority,
                EstimatedComplexity = dto.EstimatedComplexity,
                DueDate = dto.DueDate,
                CreatedAt = DateTime.UtcNow
            };

            await _taskRepository.AddAsync(task);

            // Reload with details
            var created = await _taskRepository.GetByIdWithDetailsAsync(task.TaskId);
            return MapToDetailDto(created!);
        }

        public async Task<TaskDetailDto?> UpdateTaskStatusAsync(int taskId, UpdateTaskStatusDto dto)
        {
            var task = await _taskRepository.GetByIdWithDetailsAsync(taskId);

            if (task == null)
                return null;

            // Validate and parse status
            if (!Enum.TryParse<TaskStatus>(dto.Status, true, out var newStatus))
            {
                throw new ValidationException(new[] { "Invalid status value. Valid values: ToDo, InProgress, Blocked, Completed" });
            }

            task.Status = newStatus;

            // Set completion date if completing
            if (newStatus == TaskStatus.Completed && task.CompletionDate == null)
            {
                task.CompletionDate = DateTime.UtcNow;
            }
            else if (newStatus != TaskStatus.Completed)
            {
                task.CompletionDate = null;
            }

            // Optional: update priority
            if (!string.IsNullOrEmpty(dto.Priority) && Enum.TryParse<TaskPriority>(dto.Priority, true, out var newPriority))
            {
                task.Priority = newPriority;
            }

            // Optional: update complexity
            if (dto.EstimatedComplexity.HasValue && dto.EstimatedComplexity >= 1 && dto.EstimatedComplexity <= 5)
            {
                task.EstimatedComplexity = dto.EstimatedComplexity.Value;
            }

            await _taskRepository.UpdateAsync(task);

            return MapToDetailDto(task);
        }

        private static TaskDetailDto MapToDetailDto(TaskItem task)
        {
            return new TaskDetailDto(
                task.TaskId,
                task.ProjectId,
                task.Project?.Name ?? string.Empty,
                task.Title,
                task.Description,
                task.AssigneeId,
                task.Assignee?.FullName,
                task.Status.ToString(),
                task.Priority.ToString(),
                task.EstimatedComplexity,
                task.DueDate,
                task.CompletionDate,
                task.CreatedAt
            );
        }
    }
}
