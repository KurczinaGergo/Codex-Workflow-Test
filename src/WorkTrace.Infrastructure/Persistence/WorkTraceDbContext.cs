using Microsoft.EntityFrameworkCore;
using WorkTrace.Infrastructure.Persistence.Entities;

namespace WorkTrace.Infrastructure.Persistence;

public sealed class WorkTraceDbContext : DbContext
{
    public WorkTraceDbContext(DbContextOptions<WorkTraceDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProjectRecord> Projects => Set<ProjectRecord>();
    public DbSet<WorkItemRecord> WorkItems => Set<WorkItemRecord>();
    public DbSet<WorkSessionRecord> WorkSessions => Set<WorkSessionRecord>();
    public DbSet<NoteRecord> Notes => Set<NoteRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectRecord>(entity =>
        {
            entity.ToTable("Projects");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.HasMany(x => x.WorkItems)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<WorkItemRecord>(entity =>
        {
            entity.ToTable("WorkItems");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(300);
            entity.Property(x => x.Description).HasMaxLength(4000);
            entity.Property(x => x.Kind).IsRequired();
            entity.Property(x => x.Status).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.UpdatedAt).IsRequired();
            entity.HasOne(x => x.Project)
                .WithMany(x => x.WorkItems)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(x => x.WorkSessions)
                .WithOne(x => x.WorkItem)
                .HasForeignKey(x => x.WorkItemId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(x => x.Notes)
                .WithOne(x => x.WorkItem)
                .HasForeignKey(x => x.WorkItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<WorkSessionRecord>(entity =>
        {
            entity.ToTable("WorkSessions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UserId).IsRequired().HasMaxLength(100);
            entity.Property(x => x.StartedAt).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.UpdatedAt).IsRequired();
            entity.HasOne(x => x.WorkItem)
                .WithMany(x => x.WorkSessions)
                .HasForeignKey(x => x.WorkItemId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => x.UserId)
                .IsUnique()
                .HasFilter("EndedAt IS NULL");
        });

        modelBuilder.Entity<NoteRecord>(entity =>
        {
            entity.ToTable("Notes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Text).IsRequired().HasMaxLength(4000);
            entity.Property(x => x.Type).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.HasOne(x => x.WorkItem)
                .WithMany(x => x.Notes)
                .HasForeignKey(x => x.WorkItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
