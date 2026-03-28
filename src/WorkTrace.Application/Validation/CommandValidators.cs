using WorkTrace.Application.Commands;
using WorkTrace.Application.Results;
using WorkTrace.Domain.Notes;
using WorkTrace.Domain.Shared;
using WorkTrace.Domain.WorkItems;

namespace WorkTrace.Application.Validation;

public sealed class CreateWorkItemValidator : ICommandValidator<CreateWorkItemCommand>
{
    public IReadOnlyList<CommandError> Validate(CreateWorkItemCommand command)
    {
        var errors = new List<CommandError>();

        if (command.ProjectId is { } projectId && projectId == ProjectId.Empty)
        {
            errors.Add(new CommandError(AppErrorCodes.ValidationFailed, "Project id cannot be empty.", nameof(command.ProjectId)));
        }

        if (!Enum.IsDefined(typeof(WorkItemKind), command.Kind))
        {
            errors.Add(new CommandError(AppErrorCodes.ValidationFailed, "Work item kind is invalid.", nameof(command.Kind)));
        }

        return errors;
    }
}

public sealed class UpdateWorkItemValidator : ICommandValidator<UpdateWorkItemCommand>
{
    public IReadOnlyList<CommandError> Validate(UpdateWorkItemCommand command)
    {
        var errors = new List<CommandError>();

        if (command.Id == WorkItemId.Empty)
        {
            errors.Add(new CommandError(AppErrorCodes.ValidationFailed, "Work item id is required.", nameof(command.Id)));
        }

        if (string.IsNullOrWhiteSpace(command.Title))
        {
            errors.Add(new CommandError(AppErrorCodes.ValidationFailed, "Work item title is required.", nameof(command.Title)));
        }

        if (!Enum.IsDefined(typeof(WorkItemKind), command.Kind))
        {
            errors.Add(new CommandError(AppErrorCodes.ValidationFailed, "Work item kind is invalid.", nameof(command.Kind)));
        }

        if (!Enum.IsDefined(typeof(WorkItemStatus), command.Status))
        {
            errors.Add(new CommandError(AppErrorCodes.ValidationFailed, "Work item status is invalid.", nameof(command.Status)));
        }

        if (command.ProjectId is { } projectId && projectId == ProjectId.Empty)
        {
            errors.Add(new CommandError(AppErrorCodes.ValidationFailed, "Project id cannot be empty.", nameof(command.ProjectId)));
        }

        return errors;
    }
}

public sealed class ArchiveWorkItemValidator : ICommandValidator<ArchiveWorkItemCommand>
{
    public IReadOnlyList<CommandError> Validate(ArchiveWorkItemCommand command)
    {
        if (command.Id == WorkItemId.Empty)
        {
            return [new CommandError(AppErrorCodes.ValidationFailed, "Work item id is required.", nameof(command.Id))];
        }

        return [];
    }
}

public sealed class RestoreWorkItemValidator : ICommandValidator<RestoreWorkItemCommand>
{
    public IReadOnlyList<CommandError> Validate(RestoreWorkItemCommand command)
    {
        if (command.Id == WorkItemId.Empty)
        {
            return [new CommandError(AppErrorCodes.ValidationFailed, "Work item id is required.", nameof(command.Id))];
        }

        return [];
    }
}

public sealed class StartWorkSessionValidator : ICommandValidator<StartWorkSessionCommand>
{
    public IReadOnlyList<CommandError> Validate(StartWorkSessionCommand command)
    {
        if (command.WorkItemId == WorkItemId.Empty)
        {
            return [new CommandError(AppErrorCodes.ValidationFailed, "Work item id is required.", nameof(command.WorkItemId))];
        }

        return [];
    }
}

public sealed class StopWorkSessionValidator : ICommandValidator<StopWorkSessionCommand>
{
    public IReadOnlyList<CommandError> Validate(StopWorkSessionCommand command)
    {
        if (command.Id == WorkSessionId.Empty)
        {
            return [new CommandError(AppErrorCodes.ValidationFailed, "Work session id is required.", nameof(command.Id))];
        }

        return [];
    }
}

public sealed class ReassignWorkSessionValidator : ICommandValidator<ReassignWorkSessionCommand>
{
    public IReadOnlyList<CommandError> Validate(ReassignWorkSessionCommand command)
    {
        var errors = new List<CommandError>();

        if (command.Id == WorkSessionId.Empty)
        {
            errors.Add(new CommandError(AppErrorCodes.ValidationFailed, "Work session id is required.", nameof(command.Id)));
        }

        if (command.WorkItemId == WorkItemId.Empty)
        {
            errors.Add(new CommandError(AppErrorCodes.ValidationFailed, "Work item id is required.", nameof(command.WorkItemId)));
        }

        return errors;
    }
}

public sealed class AddNoteValidator : ICommandValidator<AddNoteCommand>
{
    public IReadOnlyList<CommandError> Validate(AddNoteCommand command)
    {
        var errors = new List<CommandError>();

        if (command.WorkItemId == WorkItemId.Empty)
        {
            errors.Add(new CommandError(AppErrorCodes.ValidationFailed, "Work item id is required.", nameof(command.WorkItemId)));
        }

        if (string.IsNullOrWhiteSpace(command.Text))
        {
            errors.Add(new CommandError(AppErrorCodes.ValidationFailed, "Note text is required.", nameof(command.Text)));
        }

        if (!Enum.IsDefined(typeof(NoteType), command.Type))
        {
            errors.Add(new CommandError(AppErrorCodes.ValidationFailed, "Note type is invalid.", nameof(command.Type)));
        }

        return errors;
    }
}

public sealed class EditNoteValidator : ICommandValidator<EditNoteCommand>
{
    public IReadOnlyList<CommandError> Validate(EditNoteCommand command)
    {
        var errors = new List<CommandError>();

        if (command.Id == NoteId.Empty)
        {
            errors.Add(new CommandError(AppErrorCodes.ValidationFailed, "Note id is required.", nameof(command.Id)));
        }

        if (string.IsNullOrWhiteSpace(command.Text))
        {
            errors.Add(new CommandError(AppErrorCodes.ValidationFailed, "Note text is required.", nameof(command.Text)));
        }

        return errors;
    }
}

public sealed class CreateProjectValidator : ICommandValidator<CreateProjectCommand>
{
    public IReadOnlyList<CommandError> Validate(CreateProjectCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return [new CommandError(AppErrorCodes.ValidationFailed, "Project name is required.", nameof(command.Name))];
        }

        return [];
    }
}

public sealed class RenameProjectValidator : ICommandValidator<RenameProjectCommand>
{
    public IReadOnlyList<CommandError> Validate(RenameProjectCommand command)
    {
        var errors = new List<CommandError>();

        if (command.Id == ProjectId.Empty)
        {
            errors.Add(new CommandError(AppErrorCodes.ValidationFailed, "Project id is required.", nameof(command.Id)));
        }

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            errors.Add(new CommandError(AppErrorCodes.ValidationFailed, "Project name is required.", nameof(command.Name)));
        }

        return errors;
    }
}
