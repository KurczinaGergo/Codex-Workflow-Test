namespace WorkTrace.Application.Results;

public sealed record CommandResult
{
    private CommandResult(bool isSuccess, IReadOnlyList<CommandError> errors, IReadOnlyList<CommandWarning> warnings)
    {
        IsSuccess = isSuccess;
        Errors = errors;
        Warnings = warnings;
    }

    public bool IsSuccess { get; }

    public IReadOnlyList<CommandError> Errors { get; }

    public IReadOnlyList<CommandWarning> Warnings { get; }

    public static CommandResult Success(params CommandWarning[] warnings)
        => new(true, Array.Empty<CommandError>(), warnings);

    public static CommandResult Failure(params CommandError[] errors)
        => new(false, errors, Array.Empty<CommandWarning>());
}

public sealed record CommandResult<T>
{
    private CommandResult(bool isSuccess, T? value, IReadOnlyList<CommandError> errors, IReadOnlyList<CommandWarning> warnings)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = errors;
        Warnings = warnings;
    }

    public bool IsSuccess { get; }

    public T? Value { get; }

    public IReadOnlyList<CommandError> Errors { get; }

    public IReadOnlyList<CommandWarning> Warnings { get; }

    public static CommandResult<T> Success(T value, params CommandWarning[] warnings)
        => new(true, value, Array.Empty<CommandError>(), warnings);

    public static CommandResult<T> Failure(params CommandError[] errors)
        => new(false, default, errors, Array.Empty<CommandWarning>());
}
