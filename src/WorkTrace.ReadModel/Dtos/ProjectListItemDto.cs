namespace WorkTrace.ReadModel.Dtos;

public sealed record ProjectListItemDto(
    Guid Id,
    string Name,
    int WorkItemCount,
    int ActiveWorkItemCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
