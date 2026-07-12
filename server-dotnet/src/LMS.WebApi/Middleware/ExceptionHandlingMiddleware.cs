using System.Net;
using System.Text.Json;
using LMS.Application.Common.Exceptions;

namespace LMS.WebApi.Middleware;

/// <summary>
/// Translates exceptions into the <c>{ success:false, message }</c> JSON shape the client expects,
/// replacing the Express ErrorMiddleware.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            await WriteAsync(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception processing {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteAsync(context, HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    private static Task WriteAsync(HttpContext context, HttpStatusCode status, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;
        var payload = JsonSerializer.Serialize(new { success = false, message },
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        return context.Response.WriteAsync(payload);
    }
}
