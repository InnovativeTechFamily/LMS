using LMS.API.Models.Domain;
using LMS.API.Models.DTOs.Notifications;
using LMS.API.Services.Interfaces;
using MongoDB.Driver;

namespace LMS.API.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IMongoCollection<Notification> _notificationsCollection;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IMongoCollection<Notification> notificationsCollection,
            ILogger<NotificationService> logger)
        {
            _notificationsCollection = notificationsCollection;
            _logger = logger;
        }

        public async Task<Notification> CreateNotificationAsync(string title, string message, string userId)
        {
            _logger.LogInformation("Creating notification for user: {UserId}", userId);

            var notification = new Notification
            {
                Title = title,
                Message = message,
                UserId = userId,
                Status = "unread"
            };

            await _notificationsCollection.InsertOneAsync(notification);
            return notification;
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(string userId)
        {
            _logger.LogInformation("Fetching notifications for user: {UserId}", userId);
            return await _notificationsCollection.Find(n => n.UserId == userId)
                .SortByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<Notification?> UpdateNotificationAsync(string notificationId, UpdateNotificationDto dto)
        {
            _logger.LogInformation("Updating notification: {NotificationId}", notificationId);

            var update = Builders<Notification>.Update
                .Set(n => n.Status, dto.Status)
                .Set(n => n.UpdatedAt, DateTime.UtcNow);

            return await _notificationsCollection.FindOneAndUpdateAsync<Notification>(
                n => n.Id == notificationId,
                update,
                new FindOneAndUpdateOptions<Notification> { ReturnDocument = ReturnDocument.After }
            );
        }

        public async Task<bool> DeleteNotificationAsync(string notificationId)
        {
            _logger.LogInformation("Deleting notification: {NotificationId}", notificationId);
            var result = await _notificationsCollection.DeleteOneAsync(n => n.Id == notificationId);
            return result.DeletedCount > 0;
        }

        public async Task<bool> DeleteOldNotificationsAsync(string userId, int daysOld = 30)
        {
            _logger.LogInformation("Deleting old notifications for user: {UserId}, older than {DaysOld} days", userId, daysOld);

            var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
            var filter = Builders<Notification>.Filter.And(
                Builders<Notification>.Filter.Eq(n => n.UserId, userId),
                Builders<Notification>.Filter.Eq(n => n.Status, "read"),
                Builders<Notification>.Filter.Lt(n => n.CreatedAt, cutoffDate)
            );

            var result = await _notificationsCollection.DeleteManyAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            _logger.LogInformation("Getting unread count for user: {UserId}", userId);
            var count = await _notificationsCollection.CountDocumentsAsync(
                n => n.UserId == userId && n.Status == "unread"
            );
            return (int)count;
        }
    }
}
