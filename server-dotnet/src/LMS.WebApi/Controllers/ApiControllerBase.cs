using LMS.Application.Common.Exceptions;
using LMS.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LMS.WebApi.Controllers;

[ApiController]
[Route("api/v1")]
public abstract class ApiControllerBase : ControllerBase
{
    private ICurrentUserService? _currentUser;

    protected ICurrentUserService CurrentUser =>
        _currentUser ??= HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();

    /// <summary>The authenticated user's id, or throws 401 when absent.</summary>
    protected string CurrentUserId =>
        CurrentUser.UserId ?? throw new UnauthorizedException();
}
