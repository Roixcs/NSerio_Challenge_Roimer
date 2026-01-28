using FluentAssertions;
using Moq;
using TeamTasks.Application.Services;
using TeamTasks.Domain.Entities;
using TeamTasks.Domain.Interfaces;

namespace TeamTasks.Tests.Services
{
    public class ProjectServiceTests
    {
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IDashboardRepository> _mockDashboardRepository;
        private readonly ProjectService _projectService;

        public ProjectServiceTests()
        {
            _mockProjectRepository = new Mock<IProjectRepository>();
            _mockDashboardRepository = new Mock<IDashboardRepository>();

            _projectService = new ProjectService(
                _mockProjectRepository.Object,
                _mockDashboardRepository.Object
            );
        }

        /// <summary>
        /// Test for GetProjectsWithStatsAsync method in ProjectService
        /// </summary>
        [Fact]
        public async Task GetProjectsWithStatsAsync_ReturnProjects_WhenDataExists_OK()
        {
            // Arrange
            var healthDtos = new List<ProjectHealthDto>
            {
                new ProjectHealthDto(1, "P1", "C1", "Active", 10, 5, 5),
                new ProjectHealthDto(2, "P2", "C2", "Active", 8, 2, 6)
            };

            _mockDashboardRepository.Setup(r => r.GetProjectHealthAsync())
                .ReturnsAsync(healthDtos);

            // Act
            var result = await _projectService.GetProjectsWithStatsAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data!.First().Name.Should().Be("P1");
        }

        /// <summary>
        /// Test for GetProjectByIdAsync method in ProjectService
        /// </summary>
        [Fact]
        public async Task GetProjectByIdAsync_ReturnSuccess_WhenProjectExists_OK()
        {
            // Arrange
            var project = new Project
            {
                ProjectId = 1,
                Name = "Test Project",
                ClientName = "Client A",
                StartDate = DateTime.UtcNow,
                Status = TeamTasks.Domain.Enums.ProjectStatus.InProgress
            };

            _mockProjectRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(project);

            // Act
            var result = await _projectService.GetProjectByIdAsync(1);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data!.Name.Should().Be("Test Project");
        }

        /// <summary>
        /// Test for GetProjectByIdAsync method in ProjectService when project does not exist
        /// </summary>
        [Fact]
        public async Task GetProjectByIdAsync_Failure_ProjectNotFound_Error()
        {
            // Arrange
            _mockProjectRepository.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Project?)null);

            // Act
            var result = await _projectService.GetProjectByIdAsync(99);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Contain("not found");
        }
    }
}
