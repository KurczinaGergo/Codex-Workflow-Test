using WorkTrace.Domain.Shared;
using WorkTrace.Domain.WorkItems;

namespace WorkTrace.Application.Abstractions.Repositories;

public interface IWorkItemRepository
{
    Task<WorkItem?> GetByIdAsync(WorkItemId id, CancellationToken cancellationToken = default);

    Task AddAsync(WorkItem workItem, CancellationToken cancellationToken = default);

    Task UpdateAsync(WorkItem workItem, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkItem>> ListAsync(CancellationToken cancellationToken = default);
}
