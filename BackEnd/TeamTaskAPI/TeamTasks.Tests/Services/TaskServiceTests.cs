using FluentAssertions;
using Moq;
using TeamTasks.Application.Dtos;
using TeamTasks.Application.Services;
using TeamTasks.Domain.Entities;
using TeamTasks.Domain.Interfaces;
using Xunit;
using TaskStatus = TeamTasks.Domain.Enums.TaskStatus;
using TaskPriority = TeamTasks.Domain.Enums.TaskPriority;
using Microsoft.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace TeamTasks.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _mockTaskRepository;
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IDeveloperRepository> _mockDeveloperRepository;
        private readonly Mock<FluentValidation.IValidator<CreateTaskDto>> _mockValidator;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockProjectRepository = new Mock<IProjectRepository>();
            _mockDeveloperRepository = new Mock<IDeveloperRepository>();
            _mockValidator = new Mock<FluentValidation.IValidator<CreateTaskDto>>();

            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateTaskDto>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _taskService = new TaskService(_mockTaskRepository.Object, _mockValidator.Object);
        }

        /// <summary>
        /// Test for CreateTaskAsync method in TaskService
        /// </summary>
        [Fact]
        public async Task CreateTaskAsync_ReturnSuccess_WhenDtoIsValid_OK()
        {
            // Arrange
            var createDto = new CreateTaskDto(
                1,
                "Test Task",
                "Description",
                1,
                "InProgress",
                "Medium",
                3,
                DateTime.UtcNow.AddDays(5)
            );

            var createdTask = new TaskItem
            {
                TaskId = 10,
                ProjectId = 1,
                Title = "Test Task",
                Status = TaskStatus.InProgress,
                Priority = TaskPriority.Medium,
                Project = new Project { ProjectId = 1, Name = "Test Project" }
            };

            _mockTaskRepository.Setup(r => r.AddWithSPAsync(It.IsAny<TaskItem>())).ReturnsAsync(createdTask);

            // Act
            var result = await _taskService.CreateTaskAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Title.Should().Be("Test Task");
            result.Data.Status.Should().Be("InProgress");
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldReturnFailure_WhenStatusIsInvalid()
        {
            // Arrange
            var createDto = new CreateTaskDto
            (
                ProjectId: 0, // Provide a default or appropriate value
                Title: "Invalid Status Task",
                Description: null,
                AssigneeId: null,
                Status: "NonExistentStatus",
                Priority: "Medium",
                EstimatedComplexity: 0,
                DueDate: null
            );

            // Act
            var result = await _taskService.CreateTaskAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Invalid Status");
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldReturnFailure_WhenRepositoryThrowsSqlException()
        {
            // Arrange
            var createDto = new CreateTaskDto
            (
                ProjectId: 99, // Non-existent
                Title: "SQL Error Task",
                Description: null,
                AssigneeId: null,
                Status: "InProgress",
                Priority: "Medium",
                EstimatedComplexity: 0,
                DueDate: null
            );
            
            // Better approach for mocking exception
            var sqlException = CreateSqlException("Constraint violation");

            _mockTaskRepository.Setup(r => r.AddWithSPAsync(It.IsAny<TaskItem>()))
                .ThrowsAsync(sqlException);

            // Act
            var result = await _taskService.CreateTaskAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Constraint violation");
        }

        [Fact]
        public async Task UpdateTaskStatusAsync_ShouldReturnSuccess_WhenTaskExists()
        {
            // Arrange
            var taskId = 10;
            var updateDto = new UpdateTaskStatusDto("Completed", "High", 5);

            var updatedTask = new TaskItem
            {
                TaskId = taskId,
                Title = "Updated Task",
                Status = TaskStatus.Completed,
                Priority = TaskPriority.High,
                EstimatedComplexity = 5
            };

            _mockTaskRepository.Setup(r => r.UpdateStatusWithSPAsync(taskId, TaskStatus.Completed.ToString(), TaskPriority.High.ToString(), 5))
                .ReturnsAsync(updatedTask);

            // Act
            var result = await _taskService.UpdateTaskStatusAsync(taskId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data!.Status.Should().Be("Completed");
        }

        [Fact]
        public async Task UpdateTaskStatusAsync_ShouldReturnFailure_WhenTaskNotFound()
        {
            // Arrange
            var taskId = 999;
            var updateDto = new UpdateTaskStatusDto("Completed", "Medium", null);

            _mockTaskRepository.Setup(r => r.UpdateStatusWithSPAsync(taskId, It.IsAny<TaskStatus>().ToString(), It.IsAny<TaskPriority>().ToString(), It.IsAny<int>()))
                .ReturnsAsync((TaskItem?)null);

            // Act
            var result = await _taskService.UpdateTaskStatusAsync(taskId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Contain($"Task with ID {taskId} not found");
        }

        // Helper to create SqlException
        private static SqlException CreateSqlException(string message)
        {
            var exception = RuntimeHelpers.GetUninitializedObject(typeof(SqlException)) as SqlException;

            // Use reflection to set private field '_message'
            var messageField = typeof(SqlException).GetField("_message", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (messageField != null)
            {
                messageField.SetValue(exception, message);
            }
            else
            {
                // Fallback for different .NET versions if internal field name differs, 
                // though usually better to just rely on type or wrap in try-catch in test setup if needed.
                // For this test, verifying type catch is consistent.
            }

            return exception!;
        }
    }
}
