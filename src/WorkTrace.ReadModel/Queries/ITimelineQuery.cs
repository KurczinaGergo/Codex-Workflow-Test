using WorkTrace.ReadModel.Dtos;

namespace WorkTrace.ReadModel.Queries;

public interface ITimelineQuery
{
    Task<IReadOnlyList<TimelineEntryDto>> GetAsync(Guid workItemId, CancellationToken cancellationToken = default);
}
