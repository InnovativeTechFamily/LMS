using LMS.Application.Services.Abstractions;
using LMS.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.WebApi.Controllers;

/// <summary>Admin analytics endpoints (mirrors analytics.route.ts).</summary>
[Authorize(Roles = UserRoles.Admin)]
public class AnalyticsController : ApiControllerBase
{
    private readonly IAnalyticsService _analytics;

    public AnalyticsController(IAnalyticsService analytics) => _analytics = analytics;

    [HttpGet("get-users-analytics")]
    public async Task<IActionResult> Users(CancellationToken ct)
        => Ok(new { success = true, users = await _analytics.GetUsersAnalyticsAsync(ct) });

    [HttpGet("get-orders-analytics")]
    public async Task<IActionResult> Orders(CancellationToken ct)
        => Ok(new { success = true, orders = await _analytics.GetOrdersAnalyticsAsync(ct) });

    [HttpGet("get-courses-analytics")]
    public async Task<IActionResult> Courses(CancellationToken ct)
        => Ok(new { success = true, courses = await _analytics.GetCoursesAnalyticsAsync(ct) });
}
