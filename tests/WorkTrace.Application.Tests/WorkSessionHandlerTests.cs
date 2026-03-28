using WorkTrace.Application.Commands;
using WorkTrace.Application.Handlers;
using WorkTrace.Application.Results;
using WorkTrace.Application.Validation;
using WorkTrace.Domain.Shared;
using WorkTrace.Domain.WorkItems;
using WorkTrace.Domain.WorkSessions;

namespace WorkTrace.Application.Tests;

public class WorkSessionHandlerTests
{
    [Fact]
    public async Task StartWorkSession_RestoresArchivedWorkItem_AndCreatesSession()
    {
        var workItems = new InMemoryWorkItemRepository();
        var sessions = new InMemoryWorkSessionRepository();
        var clock = new FakeClock { UtcNow = new DateTimeOffset(2026, 03, 28, 12, 0, 0, TimeSpan.Zero) };
        var currentUser = new FakeCurrentUser { UserId = UserId.New() };
        var unitOfWork = new FakeUnitOfWork();
        var workItem = WorkItem.Create("Build", createdAt: clock.UtcNow.AddHours(-1));
        workItem.Archive(clock.UtcNow.AddMinutes(-10));
        await workItems.AddAsync(workItem);

        var handler = new StartWorkSessionHandler(workItems, sessions, currentUser, clock, unitOfWork, new StartWorkSessionValidator());
        var result = await handler.HandleAsync(new StartWorkSessionCommand(workItem.Id));

        Assert.True(result.IsSuccess);
        Assert.Single(result.Warnings);
        Assert.False((await workItems.GetByIdAsync(workItem.Id))!.IsArchived);
        Assert.True(result.Value != WorkSessionId.Empty);
    }

    [Fact]
    public async Task StopWorkSession_PromotesTodoWorkItemToInProgress()
    {
        var workItems = new InMemoryWorkItemRepository();
        var sessions = new InMemoryWorkSessionRepository();
        var clock = new FakeClock { UtcNow = new DateTimeOffset(2026, 03, 28, 12, 0, 0, TimeSpan.Zero) };
        var currentUser = new FakeCurrentUser { UserId = UserId.New() };
        var unitOfWork = new FakeUnitOfWork();
        var workItem = WorkItem.Create("Build", createdAt: clock.UtcNow.AddHours(-1));
        await workItems.AddAsync(workItem);
        var session = WorkSession.Create(currentUser.UserId, workItem.Id, clock.UtcNow.AddMinutes(-15));
        await sessions.AddAsync(session);

        var handler = new StopWorkSessionHandler(sessions, workItems, currentUser, clock, unitOfWork, new StopWorkSessionValidator());
        clock.UtcNow = clock.UtcNow.AddMinutes(10);
        var result = await handler.HandleAsync(new StopWorkSessionCommand(session.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal(WorkItemStatus.InProgress, (await workItems.GetByIdAsync(workItem.Id))!.Status);
        Assert.Single(result.Warnings);
        Assert.Equal(AppWarningCodes.PromotedToInProgress, result.Warnings[0].Code);
    }

    [Fact]
    public async Task StartWorkSession_RejectsWhenAnotherActiveSessionExists()
    {
        var workItems = new InMemoryWorkItemRepository();
        var sessions = new InMemoryWorkSessionRepository();
        var clock = new FakeClock { UtcNow = new DateTimeOffset(2026, 03, 28, 12, 0, 0, TimeSpan.Zero) };
        var currentUser = new FakeCurrentUser { UserId = UserId.New() };
        var unitOfWork = new FakeUnitOfWork();
        var firstItem = WorkItem.Create("One", createdAt: clock.UtcNow.AddHours(-2));
        var secondItem = WorkItem.Create("Two", createdAt: clock.UtcNow.AddHours(-1));
        await workItems.AddAsync(firstItem);
        await workItems.AddAsync(secondItem);
        await sessions.AddAsync(WorkSession.Create(currentUser.UserId, firstItem.Id, clock.UtcNow.AddMinutes(-30)));

        var handler = new StartWorkSessionHandler(workItems, sessions, currentUser, clock, unitOfWork, new StartWorkSessionValidator());
        var result = await handler.HandleAsync(new StartWorkSessionCommand(secondItem.Id));

        Assert.False(result.IsSuccess);
        Assert.Equal(AppErrorCodes.ActiveSessionExists, result.Errors[0].Code);
    }
}
