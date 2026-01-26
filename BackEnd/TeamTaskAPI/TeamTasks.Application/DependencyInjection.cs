using Microsoft.Extensions.DependencyInjection;
using TeamTasks.Application.Interfaces;
using TeamTasks.Application.Services;

namespace TeamTasks.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IDeveloperService, DeveloperService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IDashboardService, DashboardService>();

            return services;
        }
    }
}
