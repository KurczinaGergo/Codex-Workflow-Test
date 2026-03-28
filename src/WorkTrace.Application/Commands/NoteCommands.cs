using WorkTrace.Domain.Notes;
using WorkTrace.Domain.Shared;

namespace WorkTrace.Application.Commands;

public sealed record AddNoteCommand(WorkItemId WorkItemId, string Text, NoteType Type = NoteType.Human);

public sealed record EditNoteCommand(NoteId Id, string Text);
