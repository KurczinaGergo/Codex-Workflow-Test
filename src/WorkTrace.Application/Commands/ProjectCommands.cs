using WorkTrace.Domain.Shared;

namespace WorkTrace.Application.Commands;

public sealed record CreateProjectCommand(string Name);

public sealed record RenameProjectCommand(ProjectId Id, string Name);
