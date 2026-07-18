using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace LMS.IntegrationTests.Users;

public class RegistrationTests : IntegrationTestBase
{
    public RegistrationTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Registration_succeeds_and_returns_an_activation_token()
    {
        var email = "new.user@test.local";

        var res = await Client.PostAsJsonAsync("/api/v1/registration",
            new { name = "New User", email, password = DefaultPassword });

        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        var body = await BodyAsync(res);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Contains(email, body.GetProperty("message").GetString());
        Assert.False(string.IsNullOrWhiteSpace(body.GetProperty("activationToken").GetString()));
    }

    [Fact]
    public async Task Registration_sends_an_activation_email_with_a_code()
    {
        var email = "emailed.user@test.local";

        await Client.PostAsJsonAsync("/api/v1/registration",
            new { name = "Emailed User", email, password = DefaultPassword });

        var mail = Factory.Email.LastTo(email);
        Assert.NotNull(mail);
        Assert.Equal("Activate your account", mail!.Subject);
        Assert.Equal("activation-mail", mail.Template);
        Assert.True(mail.Data.ContainsKey("activationCode"));
    }

    [Fact]
    public async Task Registration_with_an_already_activated_email_is_rejected()
    {
        var email = "dupe@test.local";
        await RegisterAndActivateAsync("Dupe", email);

        var res = await Client.PostAsJsonAsync("/api/v1/registration",
            new { name = "Dupe Again", email, password = DefaultPassword });

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        var body = await BodyAsync(res);
        Assert.False(body.GetProperty("success").GetBoolean());
        Assert.Equal("Email already exist", body.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Two_registrations_before_activation_both_return_tokens()
    {
        // No user exists until activation, so registering twice is allowed.
        var email = "pending@test.local";
        var first = await RegisterAsync("Pending", email);
        var second = await RegisterAsync("Pending", email);

        Assert.False(string.IsNullOrWhiteSpace(first));
        Assert.False(string.IsNullOrWhiteSpace(second));
    }
}
