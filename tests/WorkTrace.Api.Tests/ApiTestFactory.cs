using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WorkTrace.Api;

namespace WorkTrace.Api.Tests;

public sealed class ApiTestFactory : WebApplicationFactory<Program>
{
    public MutableClock Clock { get; } = new(new DateTimeOffset(2026, 3, 28, 12, 0, 0, TimeSpan.Zero));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IClock>();
            services.AddSingleton<IClock>(Clock);

            services.PostConfigure<WorkTraceOptions>(options =>
            {
                options.CurrentUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
                options.CurrentUserName = "Test User";
            });
        });
    }
}

public sealed class MutableClock : IClock
{
    public MutableClock(DateTimeOffset current)
    {
        UtcNow = current;
    }

    public DateTimeOffset UtcNow { get; private set; }

    public void Advance(TimeSpan amount)
    {
        UtcNow = UtcNow.Add(amount);
    }
}
