using WorkTrace.Infrastructure.Abstractions;

namespace WorkTrace.Infrastructure.CurrentUser;

public sealed class FixedCurrentUser : ICurrentUser
{
    public FixedCurrentUser(string userId)
    {
        UserId = string.IsNullOrWhiteSpace(userId) ? throw new ArgumentException("User id is required.", nameof(userId)) : userId;
    }

    public string UserId { get; }
}
