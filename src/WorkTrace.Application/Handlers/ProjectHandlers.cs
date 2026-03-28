using WorkTrace.Application.Abstractions;
using WorkTrace.Application.Abstractions.Repositories;
using WorkTrace.Application.Commands;
using WorkTrace.Application.Results;
using WorkTrace.Application.Validation;
using WorkTrace.Domain.Projects;
using WorkTrace.Domain.Shared;

namespace WorkTrace.Application.Handlers;

public sealed class CreateProjectHandler : ICommandHandler<CreateProjectCommand, CommandResult<ProjectId>>
{
    private readonly IProjectRepository _projects;
    private readonly IClock _clock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommandValidator<CreateProjectCommand> _validator;

    public CreateProjectHandler(
        IProjectRepository projects,
        IClock clock,
        IUnitOfWork unitOfWork,
        ICommandValidator<CreateProjectCommand> validator)
    {
        _projects = projects;
        _clock = clock;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CommandResult<ProjectId>> HandleAsync(CreateProjectCommand command, CancellationToken cancellationToken = default)
    {
        var errors = _validator.Validate(command);
        if (errors.Count > 0)
        {
            return CommandResult<ProjectId>.Failure(errors.ToArray());
        }

        var project = Project.Create(command.Name, _clock.UtcNow);
        await _projects.AddAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CommandResult<ProjectId>.Success(project.Id);
    }
}

public sealed class RenameProjectHandler : ICommandHandler<RenameProjectCommand, CommandResult>
{
    private readonly IProjectRepository _projects;
    private readonly IClock _clock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommandValidator<RenameProjectCommand> _validator;

    public RenameProjectHandler(
        IProjectRepository projects,
        IClock clock,
        IUnitOfWork unitOfWork,
        ICommandValidator<RenameProjectCommand> validator)
    {
        _projects = projects;
        _clock = clock;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CommandResult> HandleAsync(RenameProjectCommand command, CancellationToken cancellationToken = default)
    {
        var errors = _validator.Validate(command);
        if (errors.Count > 0)
        {
            return CommandResult.Failure(errors.ToArray());
        }

        var project = await _projects.GetByIdAsync(command.Id, cancellationToken);
        if (project is null)
        {
            return CommandResult.Failure(new CommandError(AppErrorCodes.ProjectNotFound, "The project could not be found.", nameof(command.Id)));
        }

        project.Rename(command.Name, _clock.UtcNow);
        await _projects.UpdateAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CommandResult.Success();
    }
}
