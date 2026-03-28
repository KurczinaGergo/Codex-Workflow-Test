using WorkTrace.Domain.Notes;
using WorkTrace.Domain.Shared;

namespace WorkTrace.Application.Abstractions.Repositories;

public interface INoteRepository
{
    Task<Note?> GetByIdAsync(NoteId id, CancellationToken cancellationToken = default);

    Task AddAsync(Note note, CancellationToken cancellationToken = default);

    Task UpdateAsync(Note note, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Note>> ListByWorkItemIdAsync(WorkItemId workItemId, CancellationToken cancellationToken = default);
}
