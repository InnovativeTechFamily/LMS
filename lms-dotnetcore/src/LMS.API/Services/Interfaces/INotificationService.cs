using LMS.API.Models.DTOs.Notifications;

namespace LMS.API.Services.Interfaces
{
    public interface INotificationService
    {
        Task<Models.Domain.Notification> CreateNotificationAsync(string title, string message, string userId);
        Task<List<Models.Domain.Notification>> GetUserNotificationsAsync(string userId);
        Task<Models.Domain.Notification?> UpdateNotificationAsync(string notificationId, UpdateNotificationDto dto);
        Task<bool> DeleteNotificationAsync(string notificationId);
        Task<bool> DeleteOldNotificationsAsync(string userId, int daysOld = 30);
        Task<int> GetUnreadCountAsync(string userId);
    }
}
