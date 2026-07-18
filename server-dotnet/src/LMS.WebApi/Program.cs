using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using LMS.Application;
using LMS.Application.Common.Interfaces.Services;
using LMS.Infrastructure;
using LMS.Infrastructure.Configuration;
using LMS.WebApi.BackgroundJobs;
using LMS.WebApi.Extensions;
using LMS.WebApi.Middleware;
using LMS.WebApi.Realtime;
using LMS.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Local, git-ignored overrides for secrets (Mongo, Redis, Stripe, Cloudinary, SMTP, etc.).
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:3000" };

// --- Services ---------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Web-layer implementations of application ports.
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<INotificationPublisher, SignalRNotificationPublisher>();
builder.Services.AddHostedService<NotificationCleanupService>();

// Authentication: JWT bearer, accepting the token from the Authorization header or the access_token cookie.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.AccessTokenSecret)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                if (string.IsNullOrEmpty(ctx.Token) &&
                    ctx.Request.Cookies.TryGetValue(AuthCookieExtensions.AccessTokenCookie, out var cookieToken))
                {
                    ctx.Token = cookieToken;
                }
                return Task.CompletedTask;
            },
            OnChallenge = ctx =>
            {
                ctx.HandleResponse();
                return WriteError(ctx.Response, StatusCodes.Status401Unauthorized, "Please login to access this resource");
            },
            OnForbidden = ctx =>
                WriteError(ctx.Response, StatusCodes.Status403Forbidden, "You are not allowed to access this resource"),
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
    options.AddPolicy("client", policy =>
        policy.WithOrigins(corsOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

// Rate limiting: 300 requests / minute per client IP. A short window keeps abuse in check while
// recovering in seconds, which suits an SPA that fans out many requests per page.
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 300,
                Window = TimeSpan.FromMinutes(1),
            }));

    // Return a friendly JSON body (and Retry-After) instead of an empty 429.
    options.OnRejected = async (context, ct) =>
    {
        var response = context.HttpContext.Response;
        response.StatusCode = StatusCodes.Status429TooManyRequests;
        response.ContentType = "application/json";
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();
        await response.WriteAsync(
            "{\"success\":false,\"message\":\"You're doing that too fast — please wait a moment and try again.\"}",
            ct);
    };
});

var app = builder.Build();

// --- Pipeline ---------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("client");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();

static Task WriteError(HttpResponse response, int statusCode, string message)
{
    if (response.HasStarted) return Task.CompletedTask;
    response.StatusCode = statusCode;
    response.ContentType = "application/json";
    var payload = JsonSerializer.Serialize(new { success = false, message },
        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    return response.WriteAsync(payload);
}

/// <summary>Exposed so the integration test host (<c>WebApplicationFactory&lt;Program&gt;</c>) can reference it.</summary>
public partial class Program { }
