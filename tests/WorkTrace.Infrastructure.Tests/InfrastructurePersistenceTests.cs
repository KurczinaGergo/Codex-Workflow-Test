using Microsoft.EntityFrameworkCore;
using WorkTrace.Infrastructure.Persistence.Entities;
using WorkTrace.Infrastructure.Repositories;
using WorkTrace.Infrastructure.UnitOfWork;

namespace WorkTrace.Infrastructure.Tests;

public sealed class InfrastructurePersistenceTests
{
    [Fact]
    public async Task WorkItemRepository_persists_and_reads_back_work_items()
    {
        using var database = new TestDatabase();
        await using var context = database.CreateContext();
        var repository = new WorkItemRepository(context);
        var unitOfWork = new EfUnitOfWork(context);

        var workItem = new WorkItemRecord
        {
            Id = Guid.NewGuid(),
            Title = "Ship MVP",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await repository.AddAsync(workItem);
        await unitOfWork.SaveChangesAsync();

        await using var verificationContext = database.CreateContext();
        var loaded = await verificationContext.WorkItems.SingleAsync(x => x.Id == workItem.Id);

        Assert.Equal("Ship MVP", loaded.Title);
    }

    [Fact]
    public async Task Database_rejects_second_active_session_for_same_user()
    {
        using var database = new TestDatabase();
        await using var context = database.CreateContext();

        var workItem = new WorkItemRecord
        {
            Id = Guid.NewGuid(),
            Title = "Build persistence layer",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        context.WorkItems.Add(workItem);
        context.WorkSessions.Add(new WorkSessionRecord
        {
            Id = Guid.NewGuid(),
            UserId = "tester",
            WorkItemId = workItem.Id,
            StartedAt = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        });
        await context.SaveChangesAsync();

        context.WorkSessions.Add(new WorkSessionRecord
        {
            Id = Guid.NewGuid(),
            UserId = "tester",
            WorkItemId = workItem.Id,
            StartedAt = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        });

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [Fact]
    public async Task Database_restricts_deleting_work_item_with_children()
    {
        using var database = new TestDatabase();
        await using var context = database.CreateContext();

        var workItem = new WorkItemRecord
        {
            Id = Guid.NewGuid(),
            Title = "Create reports",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        context.WorkItems.Add(workItem);
        context.WorkSessions.Add(new WorkSessionRecord
        {
            Id = Guid.NewGuid(),
            UserId = "tester",
            WorkItem = workItem,
            StartedAt = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        });
        context.Notes.Add(new NoteRecord
        {
            Id = Guid.NewGuid(),
            WorkItem = workItem,
            Text = "Dependent note",
            CreatedAt = DateTimeOffset.UtcNow
        });
        await context.SaveChangesAsync();

        Assert.Throws<InvalidOperationException>(() => context.WorkItems.Remove(workItem));
    }
}
