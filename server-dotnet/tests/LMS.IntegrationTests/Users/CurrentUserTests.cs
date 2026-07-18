using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace LMS.IntegrationTests.Users;

public class CurrentUserTests : IntegrationTestBase
{
    public CurrentUserTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Me_with_a_valid_token_returns_the_current_user()
    {
        var (client, _) = await CreateUserAsync("me@test.local", name: "Me User");

        var res = await client.GetAsync("/api/v1/me");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var body = await BodyAsync(res);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal("me@test.local", body.GetProperty("user").GetProperty("email").GetString());
    }

    [Fact]
    public async Task Me_without_a_token_returns_401()
    {
        var res = await Client.GetAsync("/api/v1/me");

        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        Assert.Equal("Please login to access this resource",
            (await BodyAsync(res)).GetProperty("message").GetString());
    }

    [Fact]
    public async Task Logout_with_a_token_succeeds()
    {
        var (client, _) = await CreateUserAsync("logout@test.local");

        var res = await client.GetAsync("/api/v1/logout");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        Assert.True((await BodyAsync(res)).GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task Logout_without_a_token_returns_401()
    {
        var res = await Client.GetAsync("/api/v1/logout");

        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Refresh_with_the_login_cookie_issues_a_new_access_token()
    {
        var email = "refresh@test.local";
        await RegisterAndActivateAsync("Refresh", email);
        // Login on the shared client so its cookie container holds the refresh cookie.
        await Client.PostAsJsonAsync("/api/v1/login", new { email, password = DefaultPassword });

        var res = await Client.GetAsync("/api/v1/refresh");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var body = await BodyAsync(res);
        Assert.Equal("success", body.GetProperty("status").GetString());
        Assert.False(string.IsNullOrWhiteSpace(body.GetProperty("accessToken").GetString()));
    }

    [Fact]
    public async Task Refresh_without_a_cookie_is_rejected()
    {
        var res = await Client.GetAsync("/api/v1/refresh");

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        Assert.Equal("Could not refresh token", (await BodyAsync(res)).GetProperty("message").GetString());
    }
}
