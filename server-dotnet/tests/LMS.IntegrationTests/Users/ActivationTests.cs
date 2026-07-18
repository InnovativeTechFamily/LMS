using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace LMS.IntegrationTests.Users;

public class ActivationTests : IntegrationTestBase
{
    public ActivationTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Activation_with_the_correct_code_creates_a_usable_account()
    {
        var email = "activate.me@test.local";
        var token = await RegisterAsync("Activate Me", email);

        var res = await Client.PostAsJsonAsync("/api/v1/activate-user",
            new { activation_token = token, activation_code = ActivationCode(token) });

        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        Assert.True((await BodyAsync(res)).GetProperty("success").GetBoolean());

        // The account now exists — login proves it.
        var login = await Client.PostAsJsonAsync("/api/v1/login", new { email, password = DefaultPassword });
        login.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Activation_with_a_wrong_code_is_rejected()
    {
        var token = await RegisterAsync("Wrong Code", "wrong.code@test.local");
        var wrong = ActivationCode(token) == "0000" ? "1111" : "0000";

        var res = await Client.PostAsJsonAsync("/api/v1/activate-user",
            new { activation_token = token, activation_code = wrong });

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        Assert.Equal("Invalid activation code", (await BodyAsync(res)).GetProperty("message").GetString());
    }

    [Fact]
    public async Task Activation_with_a_garbage_token_is_rejected()
    {
        var res = await Client.PostAsJsonAsync("/api/v1/activate-user",
            new { activation_token = "not-a-real-jwt", activation_code = "1234" });

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        Assert.Equal("Invalid activation token", (await BodyAsync(res)).GetProperty("message").GetString());
    }

    [Fact]
    public async Task Activating_the_same_account_twice_is_rejected()
    {
        var email = "twice@test.local";
        var token = await RegisterAsync("Twice", email);
        var code = ActivationCode(token);

        var first = await Client.PostAsJsonAsync("/api/v1/activate-user",
            new { activation_token = token, activation_code = code });
        first.EnsureSuccessStatusCode();

        var second = await Client.PostAsJsonAsync("/api/v1/activate-user",
            new { activation_token = token, activation_code = code });

        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
        Assert.Equal("Email already exist", (await BodyAsync(second)).GetProperty("message").GetString());
    }
}
