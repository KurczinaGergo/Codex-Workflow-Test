namespace WorkTrace.Application.Results;

public sealed record CommandWarning(string Code, string Message, string? Target = null);
