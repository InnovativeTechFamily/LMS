using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace LMS.IntegrationTests;

/// <summary>General pipeline smoke tests (health check). Endpoint-specific cases live under folders like Users/.</summary>
public class ApiEndpointsTests : IntegrationTestBase
{
    public ApiEndpointsTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Test_endpoint_reports_the_api_is_working()
    {
        var response = await Client.GetAsync("/test");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal("Api is working", body.GetProperty("message").GetString());
    }
}
