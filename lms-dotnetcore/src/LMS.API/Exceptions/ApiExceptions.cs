namespace LMS.API.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; set; }
        public string? ErrorCode { get; set; }

        public ApiException(string message, int statusCode = 500, string? errorCode = null)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        public ApiException(string message, Exception innerException, int statusCode = 500, string? errorCode = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }

    public class NotFoundException : ApiException
    {
        public NotFoundException(string message, string? errorCode = null)
            : base(message, 404, errorCode ?? "NOT_FOUND")
        {
        }
    }

    public class UnauthorizedException : ApiException
    {
        public UnauthorizedException(string message, string? errorCode = null)
            : base(message, 401, errorCode ?? "UNAUTHORIZED")
        {
        }
    }

    public class ForbiddenException : ApiException
    {
        public ForbiddenException(string message, string? errorCode = null)
            : base(message, 403, errorCode ?? "FORBIDDEN")
        {
        }
    }

    public class ConflictException : ApiException
    {
        public ConflictException(string message, string? errorCode = null)
            : base(message, 409, errorCode ?? "CONFLICT")
        {
        }
    }

    public class ValidationException : ApiException
    {
        public List<string> ValidationErrors { get; set; } = new();

        public ValidationException(string message, List<string>? errors = null, string? errorCode = null)
            : base(message, 400, errorCode ?? "VALIDATION_ERROR")
        {
            ValidationErrors = errors ?? new();
        }
    }

    public class BadRequestException : ApiException
    {
        public BadRequestException(string message, string? errorCode = null)
            : base(message, 400, errorCode ?? "BAD_REQUEST")
        {
        }
    }
}
