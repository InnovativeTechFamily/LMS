using LMS.Application.DTOs.Auth;
using LMS.Application.Services.Abstractions;
using LMS.Domain.Common;
using LMS.Infrastructure.Configuration;
using LMS.WebApi.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LMS.WebApi.Controllers;

/// <summary>Authentication and user-management endpoints (mirrors user.route.ts).</summary>
public class UsersController : ApiControllerBase
{
    private readonly IAuthService _auth;
    private readonly IUserService _users;
    private readonly JwtSettings _jwt;

    public UsersController(IAuthService auth, IUserService users, IOptions<JwtSettings> jwt)
    {
        _auth = auth;
        _users = users;
        _jwt = jwt.Value;
    }

    [HttpPost("registration")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequest request, CancellationToken ct)
    {
        var result = await _auth.RegisterAsync(request, ct);
        return StatusCode(201, new
        {
            success = true,
            message = $"Please check your email: {result.Email} to activate your account!",
            activationToken = result.ActivationToken,
        });
    }

    [HttpPost("activate-user")]
    public async Task<IActionResult> Activate([FromBody] ActivationRequest request, CancellationToken ct)
    {
        await _auth.ActivateAsync(request, ct);
        return StatusCode(201, new { success = true });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var auth = await _auth.LoginAsync(request, ct);
        Response.SetAuthCookies(auth, _jwt);
        return Ok(new { success = true, user = auth.User, accessToken = auth.AccessToken });
    }

    [Authorize]
    [HttpGet("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        await _auth.LogoutAsync(CurrentUserId, ct);
        Response.ClearAuthCookies();
        return Ok(new { success = true, message = "Logged out successfully" });
    }

    [HttpGet("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        var refreshToken = Request.Cookies[AuthCookieExtensions.RefreshTokenCookie];
        var auth = await _auth.RefreshAsync(refreshToken, ct);
        Response.SetAuthCookies(auth, _jwt);
        return Ok(new { status = "success", accessToken = auth.AccessToken });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(CurrentUserId, ct);
        return Ok(new { success = true, user });
    }

    [HttpPost("social-auth")]
    public async Task<IActionResult> SocialAuth([FromBody] SocialAuthRequest request, CancellationToken ct)
    {
        var auth = await _auth.SocialAuthAsync(request, ct);
        Response.SetAuthCookies(auth, _jwt);
        return Ok(new { success = true, user = auth.User, accessToken = auth.AccessToken });
    }

    [Authorize]
    [HttpPut("update-user-info")]
    public async Task<IActionResult> UpdateInfo([FromBody] UpdateUserInfoRequest request, CancellationToken ct)
    {
        var user = await _users.UpdateInfoAsync(CurrentUserId, request, ct);
        return StatusCode(201, new { success = true, user });
    }

    [Authorize]
    [HttpPut("update-user-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request, CancellationToken ct)
    {
        var user = await _users.UpdatePasswordAsync(CurrentUserId, request, ct);
        return StatusCode(201, new { success = true, user });
    }

    [Authorize]
    [HttpPut("update-user-avatar")]
    public async Task<IActionResult> UpdateAvatar([FromBody] UpdateProfilePictureRequest request, CancellationToken ct)
    {
        var user = await _users.UpdateAvatarAsync(CurrentUserId, request, ct);
        return Ok(new { success = true, user });
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet("get-users")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var users = await _users.GetAllAsync(ct);
        return StatusCode(201, new { success = true, users });
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("update-user")]
    public async Task<IActionResult> UpdateRole([FromBody] UpdateUserRoleRequest request, CancellationToken ct)
    {
        var user = await _users.UpdateRoleAsync(request, ct);
        return StatusCode(201, new { success = true, user });
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("delete-user/{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        await _users.DeleteAsync(id, ct);
        return Ok(new { success = true, message = "User deleted successfully" });
    }
}
