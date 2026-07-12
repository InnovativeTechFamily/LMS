using LMS.Application.DTOs.Layouts;
using LMS.Application.Services.Abstractions;
using LMS.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.WebApi.Controllers;

/// <summary>Site layout content endpoints (mirrors layout.route.ts).</summary>
public class LayoutController : ApiControllerBase
{
    private readonly ILayoutService _layout;

    public LayoutController(ILayoutService layout) => _layout = layout;

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost("create-layout")]
    public async Task<IActionResult> Create([FromBody] LayoutRequest request, CancellationToken ct)
    {
        await _layout.CreateAsync(request, ct);
        return Ok(new { success = true, message = "Layout created successfully" });
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("edit-layout")]
    public async Task<IActionResult> Edit([FromBody] LayoutRequest request, CancellationToken ct)
    {
        await _layout.EditAsync(request, ct);
        return Ok(new { success = true, message = "Layout Updated successfully" });
    }

    [HttpGet("get-layout/{type}")]
    public async Task<IActionResult> GetByType(string type, CancellationToken ct)
    {
        var layout = await _layout.GetByTypeAsync(type, ct);
        return StatusCode(201, new { success = true, layout });
    }
}
