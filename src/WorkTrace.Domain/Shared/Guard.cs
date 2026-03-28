namespace WorkTrace.Domain.Shared;

public static class Guard
{
    public static string AgainstNullOrWhiteSpace(string? value, string paramName, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(DomainErrorCodes.RequiredValue, paramName, message);
        }

        return value.Trim();
    }

    public static TEnum AgainstInvalidEnum<TEnum>(TEnum value, string paramName, string message)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(typeof(TEnum), value))
        {
            throw new DomainException(DomainErrorCodes.InvalidEnum, paramName, message);
        }

        return value;
    }

    public static void AgainstEmpty(Guid value, string paramName, string message)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException(DomainErrorCodes.RequiredValue, paramName, message);
        }
    }
}
