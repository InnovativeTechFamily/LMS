using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace LMS.IntegrationTests;

/// <summary>
/// Integration tests that drive the API through its full pipeline (routing, auth, middleware).
/// </summary>
public class ApiEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiEndpointsTests(CustomWebApplicationFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Test_endpoint_reports_the_api_is_working()
    {
        var response = await _client.GetAsync("/test");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal("Api is working", body.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Admin_route_without_a_token_returns_401_in_the_expected_shape()
    {
        var response = await _client.GetAsync("/api/v1/get-users");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.False(body.GetProperty("success").GetBoolean());
        Assert.Equal("Please login to access this resource", body.GetProperty("message").GetString());
    }
}
