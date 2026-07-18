using LMS.Application.DTOs.Courses;
using LMS.Application.Services.Abstractions;
using LMS.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.WebApi.Controllers;

/// <summary>Course CRUD, content, Q&amp;A and reviews (mirrors course.route.ts).</summary>
public class CoursesController : ApiControllerBase
{
    private readonly ICourseService _courses;

    public CoursesController(ICourseService courses) => _courses = courses;

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost("create-course")]
    public async Task<IActionResult> Create([FromBody] CourseRequest request, CancellationToken ct)
    {
        var course = await _courses.CreateAsync(request, ct);
        return StatusCode(201, new { success = true, course });
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("edit-course/{id}")]
    public async Task<IActionResult> Edit(string id, [FromBody] CourseRequest request, CancellationToken ct)
    {
        var course = await _courses.EditAsync(id, request, ct);
        return StatusCode(201, new { success = true, course });
    }

    [HttpGet("get-course/{id}")]
    public async Task<IActionResult> GetSingle(string id, CancellationToken ct)
    {
        var course = await _courses.GetSinglePublicAsync(id, ct);
        return Ok(new { success = true, course });
    }

    [HttpGet("get-courses")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var courses = await _courses.GetAllPublicAsync(ct);
        return Ok(new { success = true, courses });
    }

    [Authorize]
    [HttpGet("get-course-content/{id}")]
    public async Task<IActionResult> GetContent(string id, CancellationToken ct)
    {
        var content = await _courses.GetContentForUserAsync(CurrentUserId, id, ct);
        return Ok(new { success = true, content });
    }

    [Authorize]
    [HttpPut("add-question")]
    public async Task<IActionResult> AddQuestion([FromBody] AddQuestionRequest request, CancellationToken ct)
    {
        var course = await _courses.AddQuestionAsync(CurrentUserId, request, ct);
        return Ok(new { success = true, course });
    }

    [Authorize]
    [HttpPut("add-answer")]
    public async Task<IActionResult> AddAnswer([FromBody] AddAnswerRequest request, CancellationToken ct)
    {
        var course = await _courses.AddAnswerAsync(CurrentUserId, request, ct);
        return Ok(new { success = true, course });
    }

    [Authorize]
    [HttpPut("add-review/{id}")]
    public async Task<IActionResult> AddReview(string id, [FromBody] AddReviewRequest request, CancellationToken ct)
    {
        var course = await _courses.AddReviewAsync(CurrentUserId, id, request, ct);
        return Ok(new { success = true, course });
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("add-reply")]
    public async Task<IActionResult> AddReviewReply([FromBody] AddReviewReplyRequest request, CancellationToken ct)
    {
        var course = await _courses.AddReviewReplyAsync(CurrentUserId, request, ct);
        return Ok(new { success = true, course });
    }

    [HttpPost("getVdoCipherOTP")]
    public async Task<IActionResult> GenerateVideoUrl([FromBody] GenerateVideoUrlRequest request, CancellationToken ct)
    {
        var json = await _courses.GenerateVideoOtpAsync(request.VideoId, ct);
        // The VdoCipher response is already JSON; return it verbatim like the Node server.
        return Content(json, "application/json");
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("delete-course/{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        await _courses.DeleteAsync(id, ct);
        return Ok(new { success = true, message = "course deleted successfully" });
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet("get-admin-courses")]
    public async Task<IActionResult> GetAdminAll(CancellationToken ct)
    {
        var courses = await _courses.GetAllAdminAsync(ct);
        return StatusCode(201, new { success = true, courses });
    }
}
