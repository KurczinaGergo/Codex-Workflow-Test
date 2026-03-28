using WorkTrace.ReadModel.Dtos;

namespace WorkTrace.ReadModel.Queries;

public interface IWorkItemListQuery
{
    Task<IReadOnlyList<WorkItemListItemDto>> GetAsync(CancellationToken cancellationToken = default);
}
