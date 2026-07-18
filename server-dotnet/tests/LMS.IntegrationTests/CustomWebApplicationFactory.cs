using LMS.Application.Common.Interfaces.Services;
using LMS.Domain.Common;
using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using LMS.IntegrationTests.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Mongo2Go;
using MongoDB.Driver;
using StackExchange.Redis;

namespace LMS.IntegrationTests;

/// <summary>
/// Boots the real API against an ephemeral MongoDB (Mongo2Go) and replaces the external
/// integrations (Redis, SMTP, Cloudinary, Stripe, VdoCipher) with in-memory fakes, so the full
/// request pipeline — routing, auth, middleware, services and persistence — runs hermetically.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly MongoDbRunner _mongo;

    public InMemoryCacheService Cache { get; } = new();
    public RecordingEmailService Email { get; } = new();

    private const string TestDatabase = "lms_integration_tests";

    public CustomWebApplicationFactory()
    {
        _mongo = MongoDbRunner.Start(singleNodeReplSet: false);

        // Provide test config as environment variables so it is present when Program.cs reads
        // configuration at startup (before the web-host ConfigureAppConfiguration hooks run) — this
        // keeps the JWT signing key used by TokenService and the bearer validator identical.
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable("Jwt__AccessTokenSecret", "integration-test-access-secret-at-least-32-chars");
        Environment.SetEnvironmentVariable("Jwt__RefreshTokenSecret", "integration-test-refresh-secret-at-least-32-chars");
        Environment.SetEnvironmentVariable("Jwt__ActivationSecret", "integration-test-activation-secret-at-least-32ch");
        // Access tokens live long enough that a whole test class can reuse one.
        Environment.SetEnvironmentVariable("Jwt__AccessTokenExpireMinutes", "30");
        Environment.SetEnvironmentVariable("Mongo__ConnectionString", _mongo.ConnectionString);
        Environment.SetEnvironmentVariable("Mongo__Database", TestDatabase);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // No background jobs in tests.
            services.RemoveAll<IHostedService>();

            // Redis → in-memory cache (and drop the eager multiplexer so no connection is attempted).
            services.RemoveAll<IConnectionMultiplexer>();
            services.RemoveAll<ICacheService>();
            services.AddSingleton<ICacheService>(Cache);

            // External integrations → fakes.
            services.RemoveAll<IEmailService>();
            services.AddSingleton<IEmailService>(Email);
            services.RemoveAll<IMediaStorage>();
            services.AddSingleton<IMediaStorage, FakeMediaStorage>();
            services.RemoveAll<IPaymentService>();
            services.AddSingleton<IPaymentService, FakePaymentService>();
            services.RemoveAll<IVideoService>();
            services.AddSingleton<IVideoService, FakeVideoService>();
        });
    }

    private MongoContext Context => Services.GetRequiredService<MongoContext>();

    /// <summary>Clears all collections and the cache so each test starts from a clean slate.</summary>
    public async Task ResetAsync()
    {
        var ctx = Context;
        await ctx.Users.DeleteManyAsync(FilterDefinition<User>.Empty);
        await ctx.Courses.DeleteManyAsync(FilterDefinition<Course>.Empty);
        await ctx.Orders.DeleteManyAsync(FilterDefinition<LMS.Domain.Entities.Order>.Empty);
        await ctx.Notifications.DeleteManyAsync(FilterDefinition<Notification>.Empty);
        await ctx.Layouts.DeleteManyAsync(FilterDefinition<Layout>.Empty);
        Cache.Clear();
        Email.Clear();
    }

    /// <summary>Promotes an existing user to admin directly in the database (no admin endpoint needed to bootstrap).</summary>
    public async Task PromoteToAdminAsync(string email)
    {
        var update = Builders<User>.Update.Set(u => u.Role, UserRoles.Admin);
        await Context.Users.UpdateOneAsync(u => u.Email == email, update);
        // Invalidate any cached copy so the next token/login reflects the new role.
        await Cache.RemoveAsync(email);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _mongo.Dispose();
    }
}
