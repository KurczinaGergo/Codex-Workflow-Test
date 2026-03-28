using WorkTrace.Domain.Shared;

namespace WorkTrace.Domain.WorkSessions;

public sealed class WorkSession
{
    private WorkSession(
        WorkSessionId id,
        UserId userId,
        WorkItemId workItemId,
        DateTimeOffset startedAt,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        UserId = userId;
        WorkItemId = workItemId;
        StartedAt = startedAt;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public WorkSessionId Id { get; }

    public UserId UserId { get; private set; }

    public WorkItemId WorkItemId { get; private set; }

    public DateTimeOffset StartedAt { get; }

    public DateTimeOffset? EndedAt { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public bool IsActive => EndedAt is null;

    public static WorkSession Create(UserId userId, WorkItemId workItemId, DateTimeOffset startedAt)
    {
        EnsureUserId(userId);
        EnsureWorkItemId(workItemId);

        return new WorkSession(
            WorkSessionId.New(),
            userId,
            workItemId,
            startedAt,
            startedAt,
            startedAt);
    }

    public void Stop(DateTimeOffset endedAt)
    {
        if (EndedAt is not null)
        {
            throw new DomainException(
                DomainErrorCodes.InvalidOperation,
                nameof(endedAt),
                "The work session has already been stopped.");
        }

        if (endedAt <= StartedAt)
        {
            throw new DomainException(
                DomainErrorCodes.InvalidDuration,
                nameof(endedAt),
                "The work session duration must be positive.");
        }

        EndedAt = endedAt;
        UpdatedAt = endedAt;
    }

    public void Reassign(WorkItemId workItemId, DateTimeOffset updatedAt)
    {
        EnsureWorkItemId(workItemId);

        if (WorkItemId != workItemId)
        {
            WorkItemId = workItemId;
            UpdatedAt = updatedAt;
        }
    }

    private static void EnsureUserId(UserId userId)
    {
        if (userId == UserId.Empty)
        {
            throw new DomainException(DomainErrorCodes.RequiredValue, nameof(userId), "A user id is required.");
        }
    }

    private static void EnsureWorkItemId(WorkItemId workItemId)
    {
        if (workItemId == WorkItemId.Empty)
        {
            throw new DomainException(DomainErrorCodes.RequiredValue, nameof(workItemId), "A work item id is required.");
        }
    }
}
