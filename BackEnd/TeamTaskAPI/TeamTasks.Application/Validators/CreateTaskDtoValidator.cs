using FluentValidation;
using TeamTasks.Application.Dtos;
using TeamTasks.Domain.Enums;
using TaskStatus = TeamTasks.Domain.Enums.TaskStatus;

namespace TeamTasks.Application.Validators
{
    public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(300).WithMessage("Title cannot exceed 300 characters.");

            RuleFor(x => x.ProjectId)
                .GreaterThan(0).WithMessage("ProjectId is required.");

            RuleFor(x => x.Status)
                .IsEnumName(typeof(TaskStatus), caseSensitive: false)
                .WithMessage("Invalid status. Valid values: ToDo, InProgress, Blocked, Completed.");

            RuleFor(x => x.Priority)
                .IsEnumName(typeof(TaskPriority), caseSensitive: false)
                .WithMessage("Invalid priority. Valid values: Low, Medium, High.");

            RuleFor(x => x.EstimatedComplexity)
                .InclusiveBetween(1, 5).WithMessage("EstimatedComplexity must be between 1 and 5.");
        }
    }
}
