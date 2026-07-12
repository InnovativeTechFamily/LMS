using LMS.API.Extensions;
using LMS.API.Hubs;
using LMS.API.Middleware;
using Serilog;

// Replace this line:
// var builder = WebApplicationBuilder.CreateBuilder(args);

// With this line:
var builder = WebApplication.CreateBuilder(args);

// Environment variables
DotNetEnv.Env.Load();

// Serilog Configuration
builder.Host.UseSerilog((context, configuration) =>
    configuration.WriteTo.Console()
        .WriteTo.File("logs/lms-.txt", rollingInterval: RollingInterval.Day)
);

// Add services
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddCorsServices();
builder.Services.AddSwagger();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();
