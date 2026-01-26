using TeamTasks.Application.Dtos;
using TeamTasks.Domain.Enums;
using TeamTasks.Domain.Interfaces;
using TaskStatus = TeamTasks.Domain.Enums.TaskStatus;

namespace TeamTasks.Application.Validators;

public static class TaskValidator
{
    public static async Task<ValidationResult> ValidateCreateAsync(CreateTaskDto dto, IProjectRepository projectRepository, IDeveloperRepository developerRepository)
    {
        var result = new ValidationResult();

        // Validate Title
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            result.AddError("Title is required.");
        }
        else if (dto.Title.Length > 300)
        {
            result.AddError("Title cannot exceed 300 characters.");
        }

        // Validate ProjectId exists
        if (!await projectRepository.ExistsAsync(dto.ProjectId))
        {
            result.AddError($"Project with ID {dto.ProjectId} does not exist.");
        }

        // Validate AssigneeId exists and is active (if provided)
        if (dto.AssigneeId.HasValue)
        {
            if (!await developerRepository.ExistsAsync(dto.AssigneeId.Value))
            {
                result.AddError($"Developer with ID {dto.AssigneeId} does not exist.");
            }
            else if (!await developerRepository.IsActiveAsync(dto.AssigneeId.Value))
            {
                result.AddError($"Developer with ID {dto.AssigneeId} is not active.");
            }
        }

        // Validate Status
        if (string.IsNullOrWhiteSpace(dto.Status))
        {
            result.AddError("Status is required.");
        }
        else if (!Enum.TryParse<TaskStatus>(dto.Status, true, out _))
        {
            result.AddError("Invalid status. Valid values: ToDo, InProgress, Blocked, Completed.");
        }

        // Validate Priority
        if (string.IsNullOrWhiteSpace(dto.Priority))
        {
            result.AddError("Priority is required.");
        }
        else if (!Enum.TryParse<TaskPriority>(dto.Priority, true, out _))
        {
            result.AddError("Invalid priority. Valid values: Low, Medium, High.");
        }

        // Validate EstimatedComplexity
        if (dto.EstimatedComplexity < 1 || dto.EstimatedComplexity > 5)
        {
            result.AddError("EstimatedComplexity must be between 1 and 5.");
        }

        return result;
    }
}
