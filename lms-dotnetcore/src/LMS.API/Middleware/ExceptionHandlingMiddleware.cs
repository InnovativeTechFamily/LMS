namespace LMS.API.Middleware
{
    using LMS.API.Models.Responses;
    using System.Net;
    using Serilog;
    using LMS.API.Exceptions;
    using System.Text.Json;

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = new ApiResponse();

            if (exception is ApiException apiException)
            {
                context.Response.StatusCode = apiException.StatusCode;
                response.Success = false;
                response.Message = apiException.Message;

                if (apiException is ValidationException validationException)
                {
                    response.Errors = validationException.ValidationErrors;
                }
            }
            else if (exception is UnauthorizedAccessException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Success = false;
                response.Message = "Unauthorized access.";
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "An internal server error occurred.";
            }

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
