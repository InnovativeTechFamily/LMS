using LMS.Domain.Entities;

namespace LMS.Application.Services.Abstractions;

public interface INotificationService
{
    Task<IReadOnlyList<Notification>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Notification>> MarkAsReadAsync(string id, CancellationToken ct = default);
    Task PurgeReadOlderThan30DaysAsync(CancellationToken ct = default);
}
