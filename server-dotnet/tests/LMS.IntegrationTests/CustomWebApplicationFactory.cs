using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace LMS.IntegrationTests;

/// <summary>
/// Boots the real API in-memory but keeps it hermetic: supplies test JWT settings and removes
/// the background cleanup job so no external MongoDB/Redis connection is attempted. The endpoints
/// under test (<c>/test</c> and an unauthorized admin route) never touch the data stores.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:AccessTokenSecret"] = "integration-test-access-secret-at-least-32-chars",
                ["Jwt:RefreshTokenSecret"] = "integration-test-refresh-secret-at-least-32-chars",
                ["Jwt:ActivationSecret"] = "integration-test-activation-secret-at-least-32ch",
                ["Mongo:ConnectionString"] = "mongodb://localhost:27017",
                ["Redis:ConnectionString"] = "localhost:6379",
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IHostedService>();
        });
    }
}
