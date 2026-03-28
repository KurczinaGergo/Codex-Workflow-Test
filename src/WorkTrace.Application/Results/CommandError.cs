namespace WorkTrace.Application.Results;

public sealed record CommandError(string Code, string Message, string? Target = null);
