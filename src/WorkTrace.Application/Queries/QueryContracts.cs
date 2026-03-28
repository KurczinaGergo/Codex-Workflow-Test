using WorkTrace.Domain.Notes;
using WorkTrace.Domain.Projects;
using WorkTrace.Domain.Shared;
using WorkTrace.Domain.WorkItems;
using WorkTrace.Domain.WorkSessions;

namespace WorkTrace.Application.Queries;

public sealed record ActiveWorkDto(
    WorkSessionId? ActiveSessionId,
    WorkItemId? WorkItemId,
    string? Title,
    WorkItemStatus? Status,
    DateTimeOffset? StartedAt,
    TimeSpan? Elapsed);

public sealed record WorkItemListItemDto(
    WorkItemId Id,
    string Title,
    WorkItemKind Kind,
    WorkItemStatus Status,
    bool IsArchived,
    ProjectId? ProjectId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record WorkSessionSummaryDto(
    WorkSessionId Id,
    UserId UserId,
    DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt,
    DateTimeOffset UpdatedAt);

public sealed record NoteSummaryDto(
    NoteId Id,
    string Text,
    NoteType Type,
    DateTimeOffset CreatedAt,
    DateTimeOffset? EditedAt);

public sealed record WorkItemDetailDto(
    WorkItemId Id,
    string Title,
    WorkItemKind Kind,
    string? Description,
    ProjectId? ProjectId,
    WorkItemStatus Status,
    DateTimeOffset? DoneAt,
    bool IsArchived,
    DateTimeOffset? ArchivedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<WorkSessionSummaryDto> Sessions,
    IReadOnlyList<NoteSummaryDto> Notes);

public sealed record TimelineEntryDto(
    DateTimeOffset At,
    string Type,
    string Message,
    WorkItemId WorkItemId);

public sealed record ProjectListItemDto(
    ProjectId Id,
    string Name,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public interface IActiveWorkQuery
{
    Task<ActiveWorkDto?> ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface IWorkItemListQuery
{
    Task<IReadOnlyList<WorkItemListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface IWorkItemDetailQuery
{
    Task<WorkItemDetailDto?> ExecuteAsync(WorkItemId id, CancellationToken cancellationToken = default);
}

public interface ITimelineQuery
{
    Task<IReadOnlyList<TimelineEntryDto>> ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface IProjectListQuery
{
    Task<IReadOnlyList<ProjectListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default);
}
