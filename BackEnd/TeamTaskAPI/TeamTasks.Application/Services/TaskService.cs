using TeamTasks.Application.Dtos;
using TeamTasks.Application.Interfaces;
using TeamTasks.Application.Validators;
using TeamTasks.Domain.Entities;
using TeamTasks.Domain.Enums;
using TeamTasks.Domain.Interfaces;
using TaskStatus = TeamTasks.Domain.Enums.TaskStatus;
using Microsoft.Data.SqlClient;

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

        public async Task<Result<PaginatedResult<TaskDto>>> GetTasksByProjectAsync(int projectId, int page, int pageSize, string? status = null, int? assigneeId = null)
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

            return Result<PaginatedResult<TaskDto>>.Success(new PaginatedResult<TaskDto>(dtos, totalCount, page, pageSize));
        }

        public async Task<Result<TaskDetailDto>> GetTaskByIdAsync(int taskId)
        {
            var task = await _taskRepository.GetByIdWithDetailsAsync(taskId);

            if (task == null)
                return Result<TaskDetailDto>.Failure($"Task with ID {taskId} not found.");

            return Result<TaskDetailDto>.Success(MapToDetailDto(task));
        }

        public async Task<Result<TaskDetailDto>> CreateTaskAsync(CreateTaskDto dto)
        {
            try
            {
                // Parse enums
                if (!Enum.TryParse<TaskStatus>(dto.Status, true, out var status))
                    return Result<TaskDetailDto>.Failure("Invalid Status.");

                if (!Enum.TryParse<TaskPriority>(dto.Priority, true, out var priority))
                    return Result<TaskDetailDto>.Failure("Invalid Priority.");

                // Create entity (basic mapping)
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

                // Use SP for insertion + business validation
                // Explicitly relying on SP for ProjectId/AssigneeId validation
                var createdTask = await _taskRepository.AddWithSPAsync(task);

                return Result<TaskDetailDto>.Success(MapToDetailDto(createdTask));
            }
            catch (SqlException ex)
            {
                // Capture SP validation errors (RAISERROR from SP)
                return Result<TaskDetailDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                // General error fallback
                return Result<TaskDetailDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Result<TaskDetailDto>> UpdateTaskStatusAsync(int taskId, UpdateTaskStatusDto dto)
        {
            try
            {
                // Use SP for updates
                var updatedTask = await _taskRepository.UpdateStatusWithSPAsync(taskId, dto.Status, dto.Priority, dto.EstimatedComplexity);

                if (updatedTask == null)
                    return Result<TaskDetailDto>.Failure($"Task with ID {taskId} not found.");

                return Result<TaskDetailDto>.Success(MapToDetailDto(updatedTask));
            }
            catch (SqlException ex)
            {
                return Result<TaskDetailDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<TaskDetailDto>.Failure(ex.Message);
            }
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
