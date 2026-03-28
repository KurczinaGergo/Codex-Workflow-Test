using WorkTrace.Domain.Shared;
using WorkTrace.Domain.WorkItems;
using WorkTrace.Domain.WorkSessions;

namespace WorkTrace.Domain.Tests;

public class WorkSessionTests
{
    [Fact]
    public void Stop_RejectsNonPositiveDuration()
    {
        var startedAt = new DateTimeOffset(2026, 03, 28, 12, 0, 0, TimeSpan.Zero);
        var session = WorkSession.Create(UserId.New(), WorkItemId.New(), startedAt);

        var ex = Assert.Throws<DomainException>(() => session.Stop(startedAt));

        Assert.Equal(DomainErrorCodes.InvalidDuration, ex.Code);
    }

    [Fact]
    public void Stop_CannotBeCalledTwice()
    {
        var startedAt = new DateTimeOffset(2026, 03, 28, 12, 0, 0, TimeSpan.Zero);
        var session = WorkSession.Create(UserId.New(), WorkItemId.New(), startedAt);
        session.Stop(startedAt.AddMinutes(5));

        var ex = Assert.Throws<DomainException>(() => session.Stop(startedAt.AddMinutes(10)));

        Assert.Equal(DomainErrorCodes.InvalidOperation, ex.Code);
    }

    [Fact]
    public void Reassign_UpdatesWorkItemId()
    {
        var startedAt = new DateTimeOffset(2026, 03, 28, 12, 0, 0, TimeSpan.Zero);
        var session = WorkSession.Create(UserId.New(), WorkItemId.New(), startedAt);
        var newWorkItemId = WorkItemId.New();
        var updatedAt = startedAt.AddMinutes(3);

        session.Reassign(newWorkItemId, updatedAt);

        Assert.Equal(newWorkItemId, session.WorkItemId);
        Assert.Equal(updatedAt, session.UpdatedAt);
    }
}
