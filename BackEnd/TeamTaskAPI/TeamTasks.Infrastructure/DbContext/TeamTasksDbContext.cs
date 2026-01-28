using Microsoft.EntityFrameworkCore;
using TeamTasks.Domain.Entities;
using TeamTasks.Domain.Enums;
using TaskStatus = TeamTasks.Domain.Enums.TaskStatus;

namespace TeamTasks.Infrastructure.DbContext
{
    public class TeamTasksDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public TeamTasksDbContext(DbContextOptions<TeamTasksDbContext> options) : base(options)
        {
        }

        public DbSet<Developer> Developers => Set<Developer>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set default schema
            modelBuilder.HasDefaultSchema("TaskManagement");

            // Developer configuration
            modelBuilder.Entity<Developer>(entity =>
            {
                entity.ToTable("Developers");
                entity.HasKey(e => e.DeveloperId);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.Ignore(e => e.FullName); // Computed property, not mapped
            });

            // Project configuration
            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Projects");
                entity.HasKey(e => e.ProjectId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ClientName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasConversion(
                        v => v.ToString(),
                        v => Enum.Parse<ProjectStatus>(v));
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // TaskItem configuration
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.ToTable("Tasks");
                entity.HasKey(e => e.TaskId);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
                entity.Property(e => e.Description).HasColumnType("nvarchar(max)");
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasConversion(
                        v => v.ToString(),
                        v => Enum.Parse<TaskStatus>(v));
                entity.Property(e => e.Priority)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasConversion(
                        v => v.ToString(),
                        v => Enum.Parse<TaskPriority>(v));
                entity.Property(e => e.EstimatedComplexity).HasDefaultValue(3);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Relationships
                entity.HasOne(e => e.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Assignee)
                    .WithMany(d => d.AssignedTasks)
                    .HasForeignKey(e => e.AssigneeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
