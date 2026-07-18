using System.Security.Claims;
using LMS.Application.Common.Interfaces.Services;

namespace LMS.WebApi.Services;

/// <summary>Resolves the authenticated caller from the request's JWT claims.</summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor) => _accessor = accessor;

    private ClaimsPrincipal? User => _accessor.HttpContext?.User;

    public string? UserId =>
        User?.FindFirst("id")?.Value ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? Role => User?.FindFirst(ClaimTypes.Role)?.Value;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
