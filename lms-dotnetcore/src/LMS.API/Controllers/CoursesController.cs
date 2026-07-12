using LMS.API.Models.DTOs.Courses;
using LMS.API.Models.Responses;
using LMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(ICourseService courseService, ILogger<CoursesController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<CourseResponseDto>>> CreateCourse([FromBody] CreateCourseDto dto)
        {
            _logger.LogInformation("Create course request: {CourseName}", dto.Name);

            var course = await _courseService.CreateCourseAsync(dto);
            var response = MapToCourseResponse(course);

            return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, ApiResponse<CourseResponseDto>.SuccessResponse(response, "Course created successfully"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CourseResponseDto>>> GetCourseById(string id)
        {
            _logger.LogInformation("Get course request: {CourseId}", id);

            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound(ApiResponse.FailureResponse("Course not found"));

            var response = MapToCourseResponse(course);
            return Ok(ApiResponse<CourseResponseDto>.SuccessResponse(response));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<CourseResponseDto>>>> GetAllCourses([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Get all courses request - Page: {Page}, Size: {PageSize}", page, pageSize);

            var courses = await _courseService.GetAllCoursesAsync(page, pageSize);
            var responses = courses.Select(MapToCourseResponse).ToList();

            var response = new PaginatedResponse<CourseResponseDto>(responses, courses.Count, page, pageSize);
            return Ok(ApiResponse<PaginatedResponse<CourseResponseDto>>.SuccessResponse(response));
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CourseResponseDto>>> UpdateCourse(string id, [FromBody] UpdateCourseDto dto)
        {
            _logger.LogInformation("Update course request: {CourseId}", id);

            var course = await _courseService.UpdateCourseAsync(id, dto);
            if (course == null)
                return NotFound(ApiResponse.FailureResponse("Course not found"));

            var response = MapToCourseResponse(course);
            return Ok(ApiResponse<CourseResponseDto>.SuccessResponse(response, "Course updated successfully"));
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteCourse(string id)
        {
            _logger.LogInformation("Delete course request: {CourseId}", id);

            var result = await _courseService.DeleteCourseAsync(id);
            if (!result)
                return NotFound(ApiResponse.FailureResponse("Course not found"));

            return Ok(ApiResponse.SuccessResponse("Course deleted successfully"));
        }

        [Authorize]
        [HttpPut("{courseId}/add-question")]
        public async Task<ActionResult<ApiResponse<CourseResponseDto>>> AddQuestion(string courseId, [FromBody] AddQuestionDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Add question request to course: {CourseId}", courseId);

            var course = await _courseService.AddQuestionAsync(courseId, dto.ContentId, dto.Question, userId!);
            if (course == null)
                return NotFound(ApiResponse.FailureResponse("Course not found"));

            var response = MapToCourseResponse(course);
            return Ok(ApiResponse<CourseResponseDto>.SuccessResponse(response, "Question added successfully"));
        }

        [Authorize]
        [HttpPut("{courseId}/add-review")]
        public async Task<ActionResult<ApiResponse<CourseResponseDto>>> AddReview(string courseId, [FromBody] AddReviewDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Add review request to course: {CourseId}", courseId);

            var course = await _courseService.AddReviewAsync(courseId, dto, userId!);
            if (course == null)
                return NotFound(ApiResponse.FailureResponse("Course not found"));

            var response = MapToCourseResponse(course);
            return Ok(ApiResponse<CourseResponseDto>.SuccessResponse(response, "Review added successfully"));
        }

        [Authorize(Roles = "admin")]
        [HttpGet("admin/all")]
        public async Task<ActionResult<ApiResponse<List<CourseResponseDto>>>> GetAdminCourses()
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Get admin courses request");

            var courses = await _courseService.GetAdminCoursesAsync(adminId!);
            var responses = courses.Select(MapToCourseResponse).ToList();

            return Ok(ApiResponse<List<CourseResponseDto>>.SuccessResponse(responses));
        }

        private CourseResponseDto MapToCourseResponse(LMS.API.Models.Domain.Course course)
        {
            return new CourseResponseDto
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                Categories = course.Categories,
                Price = course.Price,
                Tags = course.Tags,
                Level = course.Level,
                DemoUrl = course.DemoUrl,
                Ratings = course.Ratings,
                Purchased = course.Purchased,
                CreatedAt = course.CreatedAt,
                UpdatedAt = course.UpdatedAt
            };
        }
    }
}
