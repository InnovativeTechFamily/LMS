namespace LMS.API.Middleware
{
    using Serilog;
    using System.Diagnostics;

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var request = context.Request;

            // Log request
            _logger.LogInformation(
                "HTTP {Method} {Path} started at {StartTime}",
                request.Method,
                request.Path,
                DateTime.UtcNow
            );

            await _next(context);

            stopwatch.Stop();
            var response = context.Response;

            // Log response
            _logger.LogInformation(
                "HTTP {Method} {Path} completed with status {StatusCode} in {ElapsedMilliseconds}ms",
                request.Method,
                request.Path,
                response.StatusCode,
                stopwatch.ElapsedMilliseconds
            );
        }
    }
}
