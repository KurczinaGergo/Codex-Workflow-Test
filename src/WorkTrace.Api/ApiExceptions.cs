namespace WorkTrace.Api;

public abstract class ApiProblemException : Exception
{
    protected ApiProblemException(string message, int statusCode, string title)
        : base(message)
    {
        StatusCode = statusCode;
        Title = title;
    }

    public int StatusCode { get; }

    public string Title { get; }
}

public sealed class ApiValidationException : ApiProblemException
{
    public ApiValidationException(string message)
        : base(message, StatusCodes.Status400BadRequest, "Validation failed")
    {
    }
}

public sealed class ApiNotFoundException : ApiProblemException
{
    public ApiNotFoundException(string message)
        : base(message, StatusCodes.Status404NotFound, "Not found")
    {
    }
}

public sealed class ApiConflictException : ApiProblemException
{
    public ApiConflictException(string message)
        : base(message, StatusCodes.Status409Conflict, "Conflict")
    {
    }
}
