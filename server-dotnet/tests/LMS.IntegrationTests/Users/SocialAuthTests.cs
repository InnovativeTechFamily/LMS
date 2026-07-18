using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace LMS.IntegrationTests.Users;

public class SocialAuthTests : IntegrationTestBase
{
    public SocialAuthTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Social_auth_creates_an_account_for_a_new_email()
    {
        var res = await Client.PostAsJsonAsync("/api/v1/social-auth",
            new { email = "social.new@test.local", name = "Social New", avatar = "https://cdn.test/pic.png" });

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var body = await BodyAsync(res);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal("social.new@test.local", body.GetProperty("user").GetProperty("email").GetString());
        Assert.False(string.IsNullOrWhiteSpace(body.GetProperty("accessToken").GetString()));
    }

    [Fact]
    public async Task Social_auth_reuses_the_same_account_on_repeat_logins()
    {
        var payload = new { email = "social.repeat@test.local", name = "Social Repeat", avatar = (string?)null };

        var first = await Client.PostAsJsonAsync("/api/v1/social-auth", payload);
        var second = await Client.PostAsJsonAsync("/api/v1/social-auth", payload);

        var firstId = (await BodyAsync(first)).GetProperty("user").GetProperty("_id").GetString();
        var secondId = (await BodyAsync(second)).GetProperty("user").GetProperty("_id").GetString();

        Assert.False(string.IsNullOrWhiteSpace(firstId));
        Assert.Equal(firstId, secondId);
    }
}
