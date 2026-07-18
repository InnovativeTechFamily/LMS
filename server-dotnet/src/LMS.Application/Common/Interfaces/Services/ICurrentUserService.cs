namespace LMS.Application.Common.Interfaces.Services;

/// <summary>Exposes the authenticated caller's identity, resolved from the request's JWT claims.</summary>
public interface ICurrentUserService
{
    string? UserId { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}
