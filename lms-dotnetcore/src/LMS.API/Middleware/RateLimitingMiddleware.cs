using Microsoft.AspNetCore.RateLimiting;
using System.Globalization;

namespace LMS.API.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private readonly Dictionary<string, (int count, DateTime resetTime)> _requestCounts = new();
        private readonly int _windowMs = 15 * 60 * 1000; // 15 minutes
        private readonly int _maxRequests = 100; // Max 100 requests per window

        public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var now = DateTime.UtcNow;
            bool rateLimitExceeded = false;

            lock (_requestCounts)
            {
                if (_requestCounts.TryGetValue(clientIp, out var record))
                {
                    if ((now - record.resetTime).TotalMilliseconds > _windowMs)
                    {
                        // Reset counter
                        _requestCounts[clientIp] = (1, now);
                    }
                    else if (record.count >= _maxRequests)
                    {
                        // Rate limit exceeded
                        rateLimitExceeded = true;
                    }
                    else
                    {
                        // Increment counter
                        _requestCounts[clientIp] = (record.count + 1, record.resetTime);
                    }
                }
                else
                {
                    // First request from this IP
                    _requestCounts[clientIp] = (1, now);
                }
            }

            if (rateLimitExceeded)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.ContentType = "application/json";
                _logger.LogWarning("Rate limit exceeded for IP: {ClientIp}", clientIp);
                await context.Response.WriteAsJsonAsync(new { message = "Rate limit exceeded. Too many requests." });
                return;
            }

            await _next(context);
        }
    }
}
