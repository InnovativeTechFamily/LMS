using LMS.Domain.Entities;

namespace LMS.Application.Common.Interfaces.Persistence;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<Notification>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Notification notification, CancellationToken ct = default);
    Task UpdateAsync(Notification notification, CancellationToken ct = default);
    Task DeleteReadOlderThanAsync(DateTime cutoff, CancellationToken ct = default);
}
