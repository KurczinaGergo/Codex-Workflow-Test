using System.Text.Json.Serialization;
using WorkTrace.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<WorkTraceOptions>(builder.Configuration.GetSection(WorkTraceOptions.SectionName));
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<IWorkTraceStore, MvpWorkTraceStore>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "WorkTrace API",
        Version = "v1",
        Description = "MVP API for the WorkTrace work-tracking system."
    });
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "WorkTrace API v1");
    options.RoutePrefix = "swagger";
});

app.MapWorkTraceEndpoints();
app.MapGet("/api/{**path}", () => Results.NotFound());
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program
{
}
