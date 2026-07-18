using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace LMS.IntegrationTests.Users;

public class AdminUserManagementTests : IntegrationTestBase
{
    public AdminUserManagementTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Get_users_as_admin_returns_the_user_list()
    {
        await RegisterAndActivateAsync("Member", "member@test.local");
        var admin = await CreateAdminAsync();

        var res = await admin.GetAsync("/api/v1/get-users");

        Assert.Equal(HttpStatusCode.Created, res.StatusCode); // controller returns 201
        var body = await BodyAsync(res);
        Assert.True(body.GetProperty("success").GetBoolean());
        var emails = body.GetProperty("users").EnumerateArray()
            .Select(u => u.GetProperty("email").GetString()).ToList();
        Assert.Contains("member@test.local", emails);
        Assert.Contains("admin@test.local", emails);
    }

    [Fact]
    public async Task Get_users_as_a_normal_user_is_forbidden()
    {
        var (user, _) = await CreateUserAsync("plain@test.local");

        var res = await user.GetAsync("/api/v1/get-users");

        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        Assert.Equal("You are not allowed to access this resource",
            (await BodyAsync(res)).GetProperty("message").GetString());
    }

    [Fact]
    public async Task Get_users_without_a_token_returns_401()
    {
        var res = await Client.GetAsync("/api/v1/get-users");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Update_user_role_as_admin_promotes_the_user()
    {
        await RegisterAndActivateAsync("Promote Me", "promote@test.local");
        var admin = await CreateAdminAsync();

        var res = await admin.PutAsJsonAsync("/api/v1/update-user",
            new { email = "promote@test.local", role = "admin" });

        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        var body = await BodyAsync(res);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal("admin", body.GetProperty("user").GetProperty("role").GetString());
    }

    [Fact]
    public async Task Update_user_role_as_a_normal_user_is_forbidden()
    {
        var (user, _) = await CreateUserAsync("noadmin@test.local");

        var res = await user.PutAsJsonAsync("/api/v1/update-user",
            new { email = "noadmin@test.local", role = "admin" });

        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
    }

    [Fact]
    public async Task Delete_user_as_admin_removes_the_account()
    {
        await RegisterAndActivateAsync("Delete Me", "delete@test.local");
        var admin = await CreateAdminAsync();

        var id = await FindUserIdAsync(admin, "delete@test.local");
        var res = await admin.DeleteAsync($"/api/v1/delete-user/{id}");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        Assert.True((await BodyAsync(res)).GetProperty("success").GetBoolean());

        // The user is gone from the list.
        var after = await admin.GetAsync("/api/v1/get-users");
        var emails = (await BodyAsync(after)).GetProperty("users").EnumerateArray()
            .Select(u => u.GetProperty("email").GetString());
        Assert.DoesNotContain("delete@test.local", emails);
    }

    [Fact]
    public async Task Delete_user_as_a_normal_user_is_forbidden()
    {
        var (user, _) = await CreateUserAsync("cantdelete@test.local");

        var res = await user.DeleteAsync("/api/v1/delete-user/000000000000000000000000");

        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
    }

    private static async Task<string> FindUserIdAsync(HttpClient admin, string email)
    {
        var res = await admin.GetAsync("/api/v1/get-users");
        var body = await res.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("users").EnumerateArray()
            .First(u => u.GetProperty("email").GetString() == email)
            .GetProperty("_id").GetString()!;
    }
}
