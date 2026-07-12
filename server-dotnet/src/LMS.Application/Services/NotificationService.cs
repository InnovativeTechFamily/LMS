using LMS.Application.Common.Exceptions;
using LMS.Application.Common.Interfaces.Persistence;
using LMS.Application.Services.Abstractions;
using LMS.Domain.Entities;

namespace LMS.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notifications;

    public NotificationService(INotificationRepository notifications)
    {
        _notifications = notifications;
    }

    public Task<IReadOnlyList<Notification>> GetAllAsync(CancellationToken ct = default)
        => _notifications.GetAllAsync(ct);

    public async Task<IReadOnlyList<Notification>> MarkAsReadAsync(string id, CancellationToken ct = default)
    {
        var notification = await _notifications.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Notification not found");

        notification.Status = NotificationStatus.Read;
        await _notifications.UpdateAsync(notification, ct);

        return await _notifications.GetAllAsync(ct);
    }

    public Task PurgeReadOlderThan30DaysAsync(CancellationToken ct = default)
    {
        var cutoff = DateTime.UtcNow.AddDays(-30);
        return _notifications.DeleteReadOlderThanAsync(cutoff, ct);
    }
}
