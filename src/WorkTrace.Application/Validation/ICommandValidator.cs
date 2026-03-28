using WorkTrace.Application.Results;

namespace WorkTrace.Application.Validation;

public interface ICommandValidator<in TCommand>
{
    IReadOnlyList<CommandError> Validate(TCommand command);
}
