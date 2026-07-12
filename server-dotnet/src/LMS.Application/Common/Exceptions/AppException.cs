using System.Net;

namespace LMS.Application.Common.Exceptions;

/// <summary>
/// Base exception carrying an HTTP status code. The API exception-handling middleware
/// translates these into the <c>{ success:false, message }</c> shape the client expects.
/// </summary>
public class AppException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public AppException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(message)
    {
        StatusCode = statusCode;
    }
}

public sealed class BadRequestException : AppException
{
    public BadRequestException(string message) : base(message, HttpStatusCode.BadRequest) { }
}

public sealed class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Please login to access this resource")
        : base(message, HttpStatusCode.Unauthorized) { }
}

public sealed class ForbiddenException : AppException
{
    public ForbiddenException(string message) : base(message, HttpStatusCode.Forbidden) { }
}

public sealed class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, HttpStatusCode.NotFound) { }
}

public sealed class ConflictException : AppException
{
    public ConflictException(string message) : base(message, HttpStatusCode.Conflict) { }
}
