using WorkTrace.Infrastructure.Clock;
using WorkTrace.Infrastructure.CurrentUser;
using WorkTrace.Infrastructure.Persistence.Entities;
using WorkTrace.Infrastructure.Persistence.Enums;
using WorkTrace.ReadModel.Queries;

namespace WorkTrace.Infrastructure.Tests;

public sealed class ReadModelQueryTests
{
    [Fact]
    public async Task ActiveWorkQuery_returns_current_session_for_configured_user()
    {
        using var database = new TestDatabase();
        await using var context = database.CreateContext();

        var project = new ProjectRecord
        {
            Id = Guid.NewGuid(),
            Name = "WorkTrace",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var workItem = new WorkItemRecord
        {
            Id = Guid.NewGuid(),
            Title = "Implement dashboards",
            Project = project,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var startedAt = DateTimeOffset.UtcNow.AddMinutes(-30);
        context.Add(project);
        context.Add(workItem);
        context.Add(new WorkSessionRecord
        {
            Id = Guid.NewGuid(),
            UserId = "worktrace-user",
            WorkItem = workItem,
            StartedAt = startedAt,
            CreatedAt = startedAt,
            UpdatedAt = startedAt
        });
        await context.SaveChangesAsync();

        var query = new ActiveWorkQuery(context, new FixedCurrentUser("worktrace-user"), new SystemClock());
        var result = await query.GetCurrentAsync();

        Assert.NotNull(result);
        Assert.Equal(workItem.Id, result!.WorkItemId);
        Assert.Equal("Implement dashboards", result.WorkItemTitle);
        Assert.Equal("Task", result.WorkItemKind);
        Assert.Equal("Todo", result.WorkItemStatus);
    }

    [Fact]
    public async Task WorkItemListQuery_returns_summary_rows()
    {
        using var database = new TestDatabase();
        await using var context = database.CreateContext();

        var project = new ProjectRecord
        {
            Id = Guid.NewGuid(),
            Name = "WorkTrace",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var workItem = new WorkItemRecord
        {
            Id = Guid.NewGuid(),
            Title = "Write tests",
            Project = project,
            Status = WorkItemStatusRecord.InProgress,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        context.Add(project);
        context.Add(workItem);
        context.Add(new NoteRecord
        {
            Id = Guid.NewGuid(),
            WorkItem = workItem,
            Text = "Seed note",
            CreatedAt = DateTimeOffset.UtcNow,
            Type = NoteTypeRecord.Human
        });
        await context.SaveChangesAsync();

        var query = new WorkItemListQuery(context);
        var rows = await query.GetAsync();

        Assert.Single(rows);
        Assert.Equal("Write tests", rows[0].Title);
        Assert.Equal("InProgress", rows[0].Status);
        Assert.Equal("WorkTrace", rows[0].ProjectName);
        Assert.Equal(1, rows[0].NoteCount);
    }

    [Fact]
    public async Task WorkItemDetailQuery_returns_session_and_note_history()
    {
        using var database = new TestDatabase();
        await using var context = database.CreateContext();

        var workItem = new WorkItemRecord
        {
            Id = Guid.NewGuid(),
            Title = "Detail view",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        context.Add(workItem);
        context.Add(new WorkSessionRecord
        {
            Id = Guid.NewGuid(),
            UserId = "worktrace-user",
            WorkItem = workItem,
            StartedAt = DateTimeOffset.UtcNow.AddHours(-1),
            EndedAt = DateTimeOffset.UtcNow.AddMinutes(-30),
            CreatedAt = DateTimeOffset.UtcNow.AddHours(-1),
            UpdatedAt = DateTimeOffset.UtcNow.AddMinutes(-30)
        });
        context.Add(new NoteRecord
        {
            Id = Guid.NewGuid(),
            WorkItem = workItem,
            Text = "Captured a note",
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-20),
            EditedAt = DateTimeOffset.UtcNow.AddMinutes(-10)
        });
        await context.SaveChangesAsync();

        var query = new WorkItemDetailQuery(context);
        var detail = await query.GetAsync(workItem.Id);

        Assert.NotNull(detail);
        Assert.Single(detail!.Sessions);
        Assert.Single(detail.Notes);
    }

    [Fact]
    public async Task TimelineQuery_returns_combined_entries_in_order()
    {
        using var database = new TestDatabase();
        await using var context = database.CreateContext();

        var workItem = new WorkItemRecord
        {
            Id = Guid.NewGuid(),
            Title = "Timeline",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        context.Add(workItem);
        context.Add(new WorkSessionRecord
        {
            Id = Guid.NewGuid(),
            UserId = "worktrace-user",
            WorkItem = workItem,
            StartedAt = DateTimeOffset.UtcNow.AddMinutes(-50),
            EndedAt = DateTimeOffset.UtcNow.AddMinutes(-40),
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-50),
            UpdatedAt = DateTimeOffset.UtcNow.AddMinutes(-40)
        });
        context.Add(new NoteRecord
        {
            Id = Guid.NewGuid(),
            WorkItem = workItem,
            Text = "Entry",
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-45),
            EditedAt = DateTimeOffset.UtcNow.AddMinutes(-35)
        });
        await context.SaveChangesAsync();

        var query = new TimelineQuery(context);
        var entries = await query.GetAsync(workItem.Id);

        Assert.Equal(4, entries.Count);
        Assert.True(entries[0].OccurredAt <= entries[1].OccurredAt);
    }

    [Fact]
    public async Task ProjectListQuery_returns_counts()
    {
        using var database = new TestDatabase();
        await using var context = database.CreateContext();

        var project = new ProjectRecord
        {
            Id = Guid.NewGuid(),
            Name = "Project A",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var workItem = new WorkItemRecord
        {
            Id = Guid.NewGuid(),
            Title = "Count work items",
            Project = project,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        context.Add(project);
        context.Add(workItem);
        context.Add(new WorkSessionRecord
        {
            Id = Guid.NewGuid(),
            UserId = "worktrace-user",
            WorkItem = workItem,
            StartedAt = DateTimeOffset.UtcNow.AddMinutes(-5),
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-5),
            UpdatedAt = DateTimeOffset.UtcNow.AddMinutes(-5)
        });
        await context.SaveChangesAsync();

        var query = new ProjectListQuery(context);
        var rows = await query.GetAsync();

        Assert.Single(rows);
        Assert.Equal(1, rows[0].WorkItemCount);
        Assert.Equal(1, rows[0].ActiveWorkItemCount);
    }
}
