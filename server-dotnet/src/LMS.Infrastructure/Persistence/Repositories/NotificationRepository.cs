using LMS.Application.Common.Interfaces.Persistence;
using LMS.Domain.Entities;
using MongoDB.Driver;

namespace LMS.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly IMongoCollection<Notification> _collection;

    public NotificationRepository(MongoContext context) => _collection = context.Notifications;

    public async Task<Notification?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _collection.Find(n => n.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<Notification>> GetAllAsync(CancellationToken ct = default)
        => await _collection.Find(FilterDefinition<Notification>.Empty)
            .SortByDescending(n => n.CreatedAt)
            .ToListAsync(ct);

    public Task AddAsync(Notification notification, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        notification.CreatedAt = now;
        notification.UpdatedAt = now;
        return _collection.InsertOneAsync(notification, cancellationToken: ct);
    }

    public Task UpdateAsync(Notification notification, CancellationToken ct = default)
    {
        notification.UpdatedAt = DateTime.UtcNow;
        return _collection.ReplaceOneAsync(n => n.Id == notification.Id, notification, cancellationToken: ct);
    }

    public Task DeleteReadOlderThanAsync(DateTime cutoff, CancellationToken ct = default)
        => _collection.DeleteManyAsync(
            n => n.Status == NotificationStatus.Read && n.CreatedAt < cutoff, ct);
}
