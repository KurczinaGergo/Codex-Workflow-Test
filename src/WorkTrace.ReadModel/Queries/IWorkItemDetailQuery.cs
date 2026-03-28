using WorkTrace.ReadModel.Dtos;

namespace WorkTrace.ReadModel.Queries;

public interface IWorkItemDetailQuery
{
    Task<WorkItemDetailDto?> GetAsync(Guid workItemId, CancellationToken cancellationToken = default);
}
