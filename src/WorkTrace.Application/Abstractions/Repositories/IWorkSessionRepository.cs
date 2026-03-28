using WorkTrace.Domain.Shared;
using WorkTrace.Domain.WorkSessions;

namespace WorkTrace.Application.Abstractions.Repositories;

public interface IWorkSessionRepository
{
    Task<WorkSession?> GetByIdAsync(WorkSessionId id, CancellationToken cancellationToken = default);

    Task<WorkSession?> GetActiveByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);

    Task AddAsync(WorkSession workSession, CancellationToken cancellationToken = default);

    Task UpdateAsync(WorkSession workSession, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkSession>> ListAsync(CancellationToken cancellationToken = default);
}
