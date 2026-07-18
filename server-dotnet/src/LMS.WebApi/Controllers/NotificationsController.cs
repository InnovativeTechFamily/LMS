using LMS.Application.Services.Abstractions;
using LMS.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.WebApi.Controllers;

/// <summary>Admin notification endpoints (mirrors notification.route.ts).</summary>
[Authorize(Roles = UserRoles.Admin)]
public class NotificationsController : ApiControllerBase
{
    private readonly INotificationService _notifications;

    public NotificationsController(INotificationService notifications) => _notifications = notifications;

    [HttpGet("get-all-notifications")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var notifications = await _notifications.GetAllAsync(ct);
        return StatusCode(201, new { success = true, notifications });
    }

    [HttpPut("update-notification/{id}")]
    public async Task<IActionResult> MarkAsRead(string id, CancellationToken ct)
    {
        var notifications = await _notifications.MarkAsReadAsync(id, ct);
        return StatusCode(201, new { success = true, notifications });
    }
}
