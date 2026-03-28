using WorkTrace.Application.Abstractions;
using WorkTrace.Application.Abstractions.Repositories;
using WorkTrace.Application.Commands;
using WorkTrace.Application.Results;
using WorkTrace.Application.Validation;
using WorkTrace.Domain.Shared;
using WorkTrace.Domain.WorkItems;
using WorkTrace.Domain.WorkSessions;

namespace WorkTrace.Application.Handlers;

public sealed class StartWorkSessionHandler : ICommandHandler<StartWorkSessionCommand, CommandResult<WorkSessionId>>
{
    private readonly IWorkItemRepository _workItems;
    private readonly IWorkSessionRepository _workSessions;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommandValidator<StartWorkSessionCommand> _validator;

    public StartWorkSessionHandler(
        IWorkItemRepository workItems,
        IWorkSessionRepository workSessions,
        ICurrentUser currentUser,
        IClock clock,
        IUnitOfWork unitOfWork,
        ICommandValidator<StartWorkSessionCommand> validator)
    {
        _workItems = workItems;
        _workSessions = workSessions;
        _currentUser = currentUser;
        _clock = clock;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CommandResult<WorkSessionId>> HandleAsync(StartWorkSessionCommand command, CancellationToken cancellationToken = default)
    {
        var errors = _validator.Validate(command);
        if (errors.Count > 0)
        {
            return CommandResult<WorkSessionId>.Failure(errors.ToArray());
        }

        var activeSession = await _workSessions.GetActiveByUserIdAsync(_currentUser.UserId, cancellationToken);
        if (activeSession is not null)
        {
            return CommandResult<WorkSessionId>.Failure(new CommandError(AppErrorCodes.ActiveSessionExists, "The current user already has an active work session."));
        }

        var workItem = await _workItems.GetByIdAsync(command.WorkItemId, cancellationToken);
        if (workItem is null)
        {
            return CommandResult<WorkSessionId>.Failure(new CommandError(AppErrorCodes.WorkItemNotFound, "The work item could not be found.", nameof(command.WorkItemId)));
        }

        var warnings = new List<CommandWarning>();
        if (workItem.IsArchived)
        {
            workItem.Restore(_clock.UtcNow);
            warnings.Add(new CommandWarning(AppWarningCodes.RestoredArchivedWorkItem, "The archived work item was restored before starting a session.", nameof(command.WorkItemId)));
            await _workItems.UpdateAsync(workItem, cancellationToken);
        }

        var session = WorkSession.Create(_currentUser.UserId, workItem.Id, _clock.UtcNow);
        await _workSessions.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CommandResult<WorkSessionId>.Success(session.Id, warnings.ToArray());
    }
}

public sealed class StopWorkSessionHandler : ICommandHandler<StopWorkSessionCommand, CommandResult>
{
    private readonly IWorkSessionRepository _workSessions;
    private readonly IWorkItemRepository _workItems;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommandValidator<StopWorkSessionCommand> _validator;

    public StopWorkSessionHandler(
        IWorkSessionRepository workSessions,
        IWorkItemRepository workItems,
        ICurrentUser currentUser,
        IClock clock,
        IUnitOfWork unitOfWork,
        ICommandValidator<StopWorkSessionCommand> validator)
    {
        _workSessions = workSessions;
        _workItems = workItems;
        _currentUser = currentUser;
        _clock = clock;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CommandResult> HandleAsync(StopWorkSessionCommand command, CancellationToken cancellationToken = default)
    {
        var errors = _validator.Validate(command);
        if (errors.Count > 0)
        {
            return CommandResult.Failure(errors.ToArray());
        }

        var session = await _workSessions.GetByIdAsync(command.Id, cancellationToken);
        if (session is null)
        {
            return CommandResult.Failure(new CommandError(AppErrorCodes.WorkSessionNotFound, "The work session could not be found.", nameof(command.Id)));
        }

        if (session.UserId != _currentUser.UserId)
        {
            return CommandResult.Failure(new CommandError(AppErrorCodes.InvalidState, "The work session does not belong to the current user.", nameof(command.Id)));
        }

        if (!session.IsActive)
        {
            return CommandResult.Failure(new CommandError(AppErrorCodes.InvalidState, "The work session has already been stopped.", nameof(command.Id)));
        }

        var workItem = await _workItems.GetByIdAsync(session.WorkItemId, cancellationToken);
        if (workItem is null)
        {
            return CommandResult.Failure(new CommandError(AppErrorCodes.WorkItemNotFound, "The associated work item could not be found.", nameof(command.Id)));
        }

        session.Stop(_clock.UtcNow);
        await _workSessions.UpdateAsync(session, cancellationToken);

        if (workItem.Status == WorkItemStatus.Todo)
        {
            workItem.SetStatus(WorkItemStatus.InProgress, _clock.UtcNow);
            await _workItems.UpdateAsync(workItem, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return CommandResult.Success(new CommandWarning(AppWarningCodes.PromotedToInProgress, "The work item was promoted to InProgress after recording time.", nameof(command.Id)));
        }

        if (workItem.Status == WorkItemStatus.Done)
        {
            workItem.SetStatus(WorkItemStatus.Done, _clock.UtcNow);
            await _workItems.UpdateAsync(workItem, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return CommandResult.Success();
    }
}

public sealed class ReassignWorkSessionHandler : ICommandHandler<ReassignWorkSessionCommand, CommandResult>
{
    private readonly IWorkSessionRepository _workSessions;
    private readonly IWorkItemRepository _workItems;
    private readonly IClock _clock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommandValidator<ReassignWorkSessionCommand> _validator;

    public ReassignWorkSessionHandler(
        IWorkSessionRepository workSessions,
        IWorkItemRepository workItems,
        IClock clock,
        IUnitOfWork unitOfWork,
        ICommandValidator<ReassignWorkSessionCommand> validator)
    {
        _workSessions = workSessions;
        _workItems = workItems;
        _clock = clock;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CommandResult> HandleAsync(ReassignWorkSessionCommand command, CancellationToken cancellationToken = default)
    {
        var errors = _validator.Validate(command);
        if (errors.Count > 0)
        {
            return CommandResult.Failure(errors.ToArray());
        }

        var session = await _workSessions.GetByIdAsync(command.Id, cancellationToken);
        if (session is null)
        {
            return CommandResult.Failure(new CommandError(AppErrorCodes.WorkSessionNotFound, "The work session could not be found.", nameof(command.Id)));
        }

        var workItem = await _workItems.GetByIdAsync(command.WorkItemId, cancellationToken);
        if (workItem is null)
        {
            return CommandResult.Failure(new CommandError(AppErrorCodes.WorkItemNotFound, "The target work item could not be found.", nameof(command.WorkItemId)));
        }

        session.Reassign(workItem.Id, _clock.UtcNow);
        await _workSessions.UpdateAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CommandResult.Success();
    }
}
