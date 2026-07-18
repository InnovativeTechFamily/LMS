using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;

namespace LMS.IntegrationTests;

/// <summary>
/// Base class for integration tests. Resets the database before each test and provides helpers for
/// the common auth flows (register → activate → login) so individual tests stay focused.
/// </summary>
[Collection(IntegrationCollection.Name)]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public Task InitializeAsync() => Factory.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    protected const string DefaultPassword = "Passw0rd!";

    /// <summary>POST /registration — returns the activation token from the response.</summary>
    protected async Task<string> RegisterAsync(string name, string email, string password = DefaultPassword)
    {
        var res = await Client.PostAsJsonAsync("/api/v1/registration", new { name, email, password });
        res.EnsureSuccessStatusCode();
        var body = await res.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("activationToken").GetString()!;
    }

    /// <summary>Reads the activation code from the (unsigned) activation-token payload.</summary>
    protected static string ActivationCode(string token)
    {
        var payload = token.Split('.')[1].Replace('-', '+').Replace('_', '/');
        payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
        var json = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
        return JsonDocument.Parse(json).RootElement.GetProperty("activationCode").GetString()!;
    }

    protected async Task RegisterAndActivateAsync(string name, string email, string password = DefaultPassword)
    {
        var token = await RegisterAsync(name, email, password);
        var res = await Client.PostAsJsonAsync("/api/v1/activate-user",
            new { activation_token = token, activation_code = ActivationCode(token) });
        res.EnsureSuccessStatusCode();
    }

    /// <summary>POST /login on the given client (default shared client) — returns the access token.</summary>
    protected async Task<string> LoginAsync(string email, string password = DefaultPassword, HttpClient? client = null)
    {
        var res = await (client ?? Client).PostAsJsonAsync("/api/v1/login", new { email, password });
        res.EnsureSuccessStatusCode();
        var body = await res.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("accessToken").GetString()!;
    }

    /// <summary>A fresh client that sends the given bearer token on every request.</summary>
    protected HttpClient AuthedClient(string token)
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    /// <summary>Creates + logs in a normal user, returning an authenticated client and the token.</summary>
    protected async Task<(HttpClient client, string token)> CreateUserAsync(
        string email, string name = "Test User", string password = DefaultPassword)
    {
        await RegisterAndActivateAsync(name, email, password);
        var token = await LoginAsync(email, password);
        return (AuthedClient(token), token);
    }

    /// <summary>Creates an admin (register → activate → promote → login) and returns an authenticated client.</summary>
    protected async Task<HttpClient> CreateAdminAsync(string email = "admin@test.local", string password = "Admin@123")
    {
        await RegisterAndActivateAsync("Admin", email, password);
        await Factory.PromoteToAdminAsync(email);
        var token = await LoginAsync(email, password);
        return AuthedClient(token);
    }

    protected static async Task<JsonElement> BodyAsync(HttpResponseMessage res)
        => await res.Content.ReadFromJsonAsync<JsonElement>();
}
