namespace WorkTrace.Domain.Shared;

public abstract class WorkTraceException : Exception
{
    protected WorkTraceException(string code, string paramName, string message)
        : base(message)
    {
        Code = code;
        ParamName = paramName;
    }

    public string Code { get; }

    public string ParamName { get; }
}

public sealed class DomainException : WorkTraceException
{
    public DomainException(string code, string paramName, string message)
        : base(code, paramName, message)
    {
    }
}
