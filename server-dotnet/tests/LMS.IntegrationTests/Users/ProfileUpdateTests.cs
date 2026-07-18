using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace LMS.IntegrationTests.Users;

public class ProfileUpdateTests : IntegrationTestBase
{
    public ProfileUpdateTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Update_user_info_changes_the_name()
    {
        var (client, _) = await CreateUserAsync("info@test.local", name: "Old Name");

        var res = await client.PutAsJsonAsync("/api/v1/update-user-info", new { name = "New Name" });

        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        var body = await BodyAsync(res);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal("New Name", body.GetProperty("user").GetProperty("name").GetString());
    }

    [Fact]
    public async Task Update_user_info_without_a_token_returns_401()
    {
        var res = await Client.PutAsJsonAsync("/api/v1/update-user-info", new { name = "Nope" });
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Update_password_with_the_correct_old_password_succeeds()
    {
        var email = "pwok@test.local";
        var (client, _) = await CreateUserAsync(email);

        var res = await client.PutAsJsonAsync("/api/v1/update-user-password",
            new { oldPassword = DefaultPassword, newPassword = "BrandNew1!" });

        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        Assert.True((await BodyAsync(res)).GetProperty("success").GetBoolean());

        // The new password now works, the old one no longer does.
        var withNew = await Client.PostAsJsonAsync("/api/v1/login", new { email, password = "BrandNew1!" });
        Assert.Equal(HttpStatusCode.OK, withNew.StatusCode);
        var withOld = await Client.PostAsJsonAsync("/api/v1/login", new { email, password = DefaultPassword });
        Assert.Equal(HttpStatusCode.BadRequest, withOld.StatusCode);
    }

    [Fact]
    public async Task Update_password_with_a_wrong_old_password_is_rejected()
    {
        var (client, _) = await CreateUserAsync("pwbad@test.local");

        var res = await client.PutAsJsonAsync("/api/v1/update-user-password",
            new { oldPassword = "not-my-password", newPassword = "BrandNew1!" });

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        Assert.Equal("Invalid old password", (await BodyAsync(res)).GetProperty("message").GetString());
    }

    [Fact]
    public async Task Update_password_with_missing_fields_is_rejected()
    {
        var (client, _) = await CreateUserAsync("pwmissing@test.local");

        var res = await client.PutAsJsonAsync("/api/v1/update-user-password",
            new { oldPassword = "", newPassword = "" });

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        Assert.Equal("Please enter old and new password",
            (await BodyAsync(res)).GetProperty("message").GetString());
    }

    [Fact]
    public async Task Update_avatar_stores_the_uploaded_image()
    {
        var (client, _) = await CreateUserAsync("avatar@test.local");

        var res = await client.PutAsJsonAsync("/api/v1/update-user-avatar",
            new { avatar = "data:image/png;base64,AAAA" });

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var avatar = (await BodyAsync(res)).GetProperty("user").GetProperty("avatar");
        Assert.Equal("https://cdn.test/avatars/fake.png", avatar.GetProperty("url").GetString());
        Assert.Equal("avatars/fake-public-id", avatar.GetProperty("public_id").GetString());
    }

    [Fact]
    public async Task Update_avatar_without_a_token_returns_401()
    {
        var res = await Client.PutAsJsonAsync("/api/v1/update-user-avatar", new { avatar = "x" });
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }
}
