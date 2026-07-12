using LMS.Domain.Entities;

namespace LMS.Application.Common.Interfaces.Persistence;

public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    Task<long> CountCreatedBetweenAsync(DateTime startInclusive, DateTime endExclusive, CancellationToken ct = default);
}
