using LMS.API.Models.Domain;
using LMS.API.Services.Repositories.Interfaces;
using MongoDB.Driver;

namespace LMS.API.Services.Repositories.Implementations
{
    public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(IMongoCollection<Notification> collection, ILogger<BaseRepository<Notification>> logger)
            : base(collection, logger)
        {
        }

        public async Task<List<Notification>> GetByUserIdAsync(string userId)
        {
            Logger.LogInformation("Getting notifications by user ID: {UserId}", userId);
            return await Collection.Find(n => n.UserId == userId)
                .SortByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            Logger.LogInformation("Getting unread count for user ID: {UserId}", userId);
            var count = await Collection.CountDocumentsAsync(n => n.UserId == userId && n.Status == "unread");
            return (int)count;
        }

        public async Task<bool> MarkAsReadAsync(string id)
        {
            Logger.LogInformation("Marking notification as read: {Id}", id);
            var update = Builders<Notification>.Update.Set(n => n.Status, "read");
            var result = await Collection.UpdateOneAsync(
                n => n.Id == id,
                update
            );
            return result.ModifiedCount > 0;
        }
    }
}
