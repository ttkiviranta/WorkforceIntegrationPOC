using Microsoft.EntityFrameworkCore;
using WorkforceIntegrationPOC.Domain.Entities;

namespace WorkforceIntegrationPOC.Infrastructure.Data;

/// <summary>
/// EF Core DbContext for workforce data.
/// The app uses SQLite by default; switching to SQL Server is a configuration change.
/// </summary>
public class WorkforceDbContext : DbContext
{
    public WorkforceDbContext(DbContextOptions<WorkforceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<Shift> Shifts => Set<Shift>();

    public DbSet<Assignment> Assignments => Set<Assignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Skills).IsRequired();
            entity.Property(x => x.Availability).IsRequired();
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Location).IsRequired().HasMaxLength(200);
            entity.Property(x => x.RequiredSkill).IsRequired().HasMaxLength(200);
            entity.Property(x => x.StartTime).IsRequired();
            entity.Property(x => x.EndTime).IsRequired();
        });

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.Employee)
                .WithMany(x => x.Assignments)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Shift)
                .WithMany(x => x.Assignments)
                .HasForeignKey(x => x.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
