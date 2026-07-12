using LMS.API.Models.Responses;
using LMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger)
        {
            _analyticsService = analyticsService;
            _logger = logger;
        }

        [HttpGet("users")]
        public async Task<ActionResult<ApiResponse<Dictionary<string, int>>>> GetUserAnalytics([FromQuery] int days = 28)
        {
            _logger.LogInformation("Get user analytics request for {Days} days", days);
            var data = await _analyticsService.GetUserAnalyticsAsync(days);
            return Ok(ApiResponse<Dictionary<string, int>>.SuccessResponse(data));
        }

        [HttpGet("orders")]
        public async Task<ActionResult<ApiResponse<Dictionary<string, int>>>> GetOrderAnalytics([FromQuery] int days = 28)
        {
            _logger.LogInformation("Get order analytics request for {Days} days", days);
            var data = await _analyticsService.GetOrderAnalyticsAsync(days);
            return Ok(ApiResponse<Dictionary<string, int>>.SuccessResponse(data));
        }

        [HttpGet("notifications")]
        public async Task<ActionResult<ApiResponse<Dictionary<string, int>>>> GetNotificationAnalytics([FromQuery] int days = 365)
        {
            _logger.LogInformation("Get notification analytics request for {Days} days", days);
            var data = await _analyticsService.GetNotificationAnalyticsAsync(days);
            return Ok(ApiResponse<Dictionary<string, int>>.SuccessResponse(data));
        }

        [HttpGet("summary")]
        public async Task<ActionResult<ApiResponse<dynamic>>> GetAnalyticsSummary()
        {
            _logger.LogInformation("Get analytics summary request");

            var totalUsers = await _analyticsService.GetTotalUsersAsync();
            var totalCourses = await _analyticsService.GetTotalCoursesAsync();
            var totalRevenue = await _analyticsService.GetTotalRevenueAsync();

            var summary = new
            {
                TotalUsers = totalUsers,
                TotalCourses = totalCourses,
                TotalRevenue = totalRevenue
            };

            return Ok(ApiResponse<dynamic>.SuccessResponse(summary));
        }
    }
}
