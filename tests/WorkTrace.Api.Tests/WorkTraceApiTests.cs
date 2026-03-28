using System.Net;
using System.Net.Http.Json;

namespace WorkTrace.Api.Tests;

public sealed class WorkTraceApiTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory _factory;

    public WorkTraceApiTests(ApiTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HomePage_serves_static_ui()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("WorkTrace MVP", html);
        Assert.Contains("/app.js", html);
    }

    [Fact]
    public async Task Health_endpoint_returns_ok()
    {
        var client = _factory.CreateClient();

        var response = await client.GetFromJsonAsync<Dictionary<string, string>>("/api/health");

        Assert.Equal("ok", response?["status"]);
    }

    [Fact]
    public async Task Create_work_item_auto_generates_title_when_blank()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/work-items", new
        {
            title = "",
            kind = "Task",
            description = "Created from a blank title"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<WorkItemDetailResponse>();
        Assert.False(string.IsNullOrWhiteSpace(created?.Title));
        Assert.Contains("Untitled work item", created!.Title);

        var workItems = await client.GetFromJsonAsync<List<WorkItemListItem>>("/api/work-items");
        Assert.Contains(workItems!, item => item.Id == created.Id);
    }

    [Fact]
    public async Task Add_note_rejects_blank_text()
    {
        var client = _factory.CreateClient();
        var workItems = await client.GetFromJsonAsync<List<WorkItemListItem>>("/api/work-items");
        var workItemId = workItems!.First().Id;

        var response = await client.PostAsJsonAsync($"/api/work-items/{workItemId}/notes", new
        {
            text = "   ",
            type = "Human"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Session_flow_enforces_single_active_session_and_promotes_work_item()
    {
        var client = _factory.CreateClient();
        var workItems = await client.GetFromJsonAsync<List<WorkItemListItem>>("/api/work-items");
        var workItemId = workItems!.First().Id;

        var startResponse = await client.PostAsJsonAsync($"/api/work-items/{workItemId}/sessions", new { });
        Assert.Equal(HttpStatusCode.Created, startResponse.StatusCode);

        var startedSession = await startResponse.Content.ReadFromJsonAsync<WorkSessionResponse>();
        Assert.NotNull(startedSession);

        var conflictResponse = await client.PostAsJsonAsync($"/api/work-items/{workItemId}/sessions", new { });
        Assert.Equal(HttpStatusCode.Conflict, conflictResponse.StatusCode);

        _factory.Clock.Advance(TimeSpan.FromMinutes(15));

        var stopResponse = await client.PostAsJsonAsync($"/api/sessions/{startedSession!.Id}/stop", new { });
        Assert.Equal(HttpStatusCode.OK, stopResponse.StatusCode);

        var item = await client.GetFromJsonAsync<WorkItemDetailResponse>($"/api/work-items/{workItemId}");
        Assert.Equal("InProgress", item!.Status);
    }

    [Fact]
    public async Task Swagger_document_is_available()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("/api/work-items", json);
    }

    [Fact]
    public async Task Unknown_api_routes_return_404()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

public sealed record WorkItemListItem(
    Guid Id,
    string Title,
    string Kind,
    string? Description,
    Guid? ProjectId,
    string Status,
    bool IsArchived,
    DateTimeOffset? DoneAt,
    DateTimeOffset? ArchivedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    int SessionCount,
    int NoteCount);

public sealed record WorkSessionResponse(
    Guid Id,
    Guid UserId,
    Guid WorkItemId,
    DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record WorkItemDetailResponse(
    Guid Id,
    string Title,
    string Kind,
    string? Description,
    Guid? ProjectId,
    string Status,
    bool IsArchived,
    DateTimeOffset? DoneAt,
    DateTimeOffset? ArchivedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<WorkSessionResponse> Sessions,
    IReadOnlyList<NoteResponse> Notes);

public sealed record NoteResponse(
    Guid Id,
    Guid WorkItemId,
    string Text,
    string Type,
    DateTimeOffset CreatedAt,
    DateTimeOffset? EditedAt);
