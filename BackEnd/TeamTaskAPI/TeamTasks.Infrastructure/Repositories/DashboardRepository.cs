using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TeamTasks.Domain.Interfaces;

namespace TeamTasks.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly string _connectionString;

        public DashboardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<DeveloperWorkloadDto>> GetDeveloperWorkloadAsync()
        {
            using var connection = CreateConnection();

            const string sql = @"
            SELECT
                DeveloperId,
                DeveloperName,
                Email,
                OpenTasksCount,
                AverageEstimatedComplexity
            FROM TaskManagement.vw_DeveloperWorkload
            ORDER BY OpenTasksCount DESC";

            return await connection.QueryAsync<DeveloperWorkloadDto>(sql);
        }

        public async Task<IEnumerable<ProjectHealthDto>> GetProjectHealthAsync()
        {
            using var connection = CreateConnection();

            const string sql = @"
            SELECT
                ProjectId,
                ProjectName,
                ClientName,
                ProjectStatus,
                TotalTasks,
                OpenTasks,
                CompletedTasks
            FROM TaskManagement.vw_ProjectHealth
            ORDER BY ProjectName";

            return await connection.QueryAsync<ProjectHealthDto>(sql);
        }

        public async Task<IEnumerable<DeveloperDelayRiskDto>> GetDeveloperDelayRiskAsync()
        {
            using var connection = CreateConnection();

            const string sql = @"
            SELECT
                DeveloperId,
                DeveloperName,
                Email,
                OpenTasksCount,
                AvgDelayDays,
                NearestDueDate,
                LatestDueDate,
                PredictedCompletionDate,
                HighRiskFlag
            FROM TaskManagement.vw_DeveloperDelayRisk
            ORDER BY HighRiskFlag DESC, AvgDelayDays DESC";

            return await connection.QueryAsync<DeveloperDelayRiskDto>(sql);
        }

        public async Task<IEnumerable<TaskDueSoonDto>> GetTasksDueSoonAsync()
        {
            using var connection = CreateConnection();

            const string sql = @"
            SELECT
                TaskId,
                Title,
                Description,
                ProjectName,
                AssigneeName,
                Status,
                Priority,
                EstimatedComplexity,
                DueDate,
                DaysUntilDue
            FROM TaskManagement.vw_TasksDueSoon
            ORDER BY DueDate";

            return await connection.QueryAsync<TaskDueSoonDto>(sql);
        }
    }

}