using LMS.API.Models.DTOs.Notifications;
using LMS.API.Models.Responses;
using LMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<NotificationResponseDto>>>> GetUserNotifications()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Get notifications request for user: {UserId}", userId);

            var notifications = await _notificationService.GetUserNotificationsAsync(userId!);
            var responses = notifications.Select(MapToNotificationResponse).ToList();

            return Ok(ApiResponse<List<NotificationResponseDto>>.SuccessResponse(responses));
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Get unread count request for user: {UserId}", userId);

            var count = await _notificationService.GetUnreadCountAsync(userId!);
            return Ok(ApiResponse<int>.SuccessResponse(count));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<NotificationResponseDto>>> UpdateNotification(string id, [FromBody] UpdateNotificationDto dto)
        {
            _logger.LogInformation("Update notification request: {NotificationId}", id);

            var notification = await _notificationService.UpdateNotificationAsync(id, dto);
            if (notification == null)
                return NotFound(ApiResponse.FailureResponse("Notification not found"));

            var response = MapToNotificationResponse(notification);
            return Ok(ApiResponse<NotificationResponseDto>.SuccessResponse(response, "Notification updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteNotification(string id)
        {
            _logger.LogInformation("Delete notification request: {NotificationId}", id);

            var result = await _notificationService.DeleteNotificationAsync(id);
            if (!result)
                return NotFound(ApiResponse.FailureResponse("Notification not found"));

            return Ok(ApiResponse.SuccessResponse("Notification deleted successfully"));
        }

        private NotificationResponseDto MapToNotificationResponse(LMS.API.Models.Domain.Notification notification)
        {
            return new NotificationResponseDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                Status = notification.Status,
                UserId = notification.UserId,
                CreatedAt = notification.CreatedAt,
                UpdatedAt = notification.UpdatedAt
            };
        }
    }
}
