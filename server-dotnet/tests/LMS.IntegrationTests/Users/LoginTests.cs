using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace LMS.IntegrationTests.Users;

public class LoginTests : IntegrationTestBase
{
    public LoginTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Login_with_valid_credentials_returns_a_token_and_sets_cookies()
    {
        var email = "login.ok@test.local";
        await RegisterAndActivateAsync("Login Ok", email);

        var res = await Client.PostAsJsonAsync("/api/v1/login", new { email, password = DefaultPassword });

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var body = await BodyAsync(res);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.False(string.IsNullOrWhiteSpace(body.GetProperty("accessToken").GetString()));
        Assert.Equal(email, body.GetProperty("user").GetProperty("email").GetString());

        // Auth cookies are issued (access_token + refresh_token).
        Assert.True(res.Headers.TryGetValues("Set-Cookie", out var cookies));
        var cookieHeader = string.Join(";", cookies!);
        Assert.Contains("access_token", cookieHeader);
        Assert.Contains("refresh_token", cookieHeader);
    }

    [Fact]
    public async Task Login_never_leaks_the_password_hash()
    {
        var email = "nopass@test.local";
        await RegisterAndActivateAsync("No Pass", email);

        var res = await Client.PostAsJsonAsync("/api/v1/login", new { email, password = DefaultPassword });
        var user = (await BodyAsync(res)).GetProperty("user");

        Assert.False(user.TryGetProperty("password", out _));
    }

    [Fact]
    public async Task Login_with_a_wrong_password_is_rejected()
    {
        var email = "badpass@test.local";
        await RegisterAndActivateAsync("Bad Pass", email);

        var res = await Client.PostAsJsonAsync("/api/v1/login", new { email, password = "wrong-password" });

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        Assert.Equal("Invalid email or password", (await BodyAsync(res)).GetProperty("message").GetString());
    }

    [Fact]
    public async Task Login_with_an_unknown_email_is_rejected()
    {
        var res = await Client.PostAsJsonAsync("/api/v1/login",
            new { email = "ghost@test.local", password = DefaultPassword });

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        Assert.Equal("Invalid email or password", (await BodyAsync(res)).GetProperty("message").GetString());
    }

    [Fact]
    public async Task Login_with_missing_fields_is_rejected()
    {
        var res = await Client.PostAsJsonAsync("/api/v1/login", new { email = "", password = "" });

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        Assert.Equal("Please enter email and password", (await BodyAsync(res)).GetProperty("message").GetString());
    }
}
