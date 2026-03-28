using WorkTrace.Application.Abstractions;
using WorkTrace.Application.Abstractions.Repositories;
using WorkTrace.Application.Commands;
using WorkTrace.Application.Results;
using WorkTrace.Application.Validation;
using WorkTrace.Domain.Shared;
using WorkTrace.Domain.Notes;

namespace WorkTrace.Application.Handlers;

public sealed class AddNoteHandler : ICommandHandler<AddNoteCommand, CommandResult<NoteId>>
{
    private readonly IWorkItemRepository _workItems;
    private readonly INoteRepository _notes;
    private readonly IClock _clock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommandValidator<AddNoteCommand> _validator;

    public AddNoteHandler(
        IWorkItemRepository workItems,
        INoteRepository notes,
        IClock clock,
        IUnitOfWork unitOfWork,
        ICommandValidator<AddNoteCommand> validator)
    {
        _workItems = workItems;
        _notes = notes;
        _clock = clock;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CommandResult<NoteId>> HandleAsync(AddNoteCommand command, CancellationToken cancellationToken = default)
    {
        var errors = _validator.Validate(command);
        if (errors.Count > 0)
        {
            return CommandResult<NoteId>.Failure(errors.ToArray());
        }

        var workItem = await _workItems.GetByIdAsync(command.WorkItemId, cancellationToken);
        if (workItem is null)
        {
            return CommandResult<NoteId>.Failure(new CommandError(AppErrorCodes.WorkItemNotFound, "The work item could not be found.", nameof(command.WorkItemId)));
        }

        var warnings = new List<CommandWarning>();
        if (workItem.IsArchived)
        {
            workItem.Restore(_clock.UtcNow);
            warnings.Add(new CommandWarning(AppWarningCodes.RestoredArchivedWorkItem, "The archived work item was restored before adding a note.", nameof(command.WorkItemId)));
            await _workItems.UpdateAsync(workItem, cancellationToken);
        }

        var note = Note.Create(workItem.Id, command.Text, command.Type, _clock.UtcNow);
        await _notes.AddAsync(note, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CommandResult<NoteId>.Success(note.Id, warnings.ToArray());
    }
}

public sealed class EditNoteHandler : ICommandHandler<EditNoteCommand, CommandResult>
{
    private readonly INoteRepository _notes;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;
    private readonly ICommandValidator<EditNoteCommand> _validator;

    public EditNoteHandler(
        INoteRepository notes,
        IUnitOfWork unitOfWork,
        IClock clock,
        ICommandValidator<EditNoteCommand> validator)
    {
        _notes = notes;
        _unitOfWork = unitOfWork;
        _clock = clock;
        _validator = validator;
    }

    public async Task<CommandResult> HandleAsync(EditNoteCommand command, CancellationToken cancellationToken = default)
    {
        var errors = _validator.Validate(command);
        if (errors.Count > 0)
        {
            return CommandResult.Failure(errors.ToArray());
        }

        var note = await _notes.GetByIdAsync(command.Id, cancellationToken);
        if (note is null)
        {
            return CommandResult.Failure(new CommandError(AppErrorCodes.NoteNotFound, "The note could not be found.", nameof(command.Id)));
        }

        note.EditText(command.Text, _clock.UtcNow);
        await _notes.UpdateAsync(note, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CommandResult.Success();
    }
}
