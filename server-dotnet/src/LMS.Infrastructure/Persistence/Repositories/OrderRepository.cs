using LMS.Application.Common.Interfaces.Persistence;
using LMS.Domain.Entities;
using MongoDB.Driver;

namespace LMS.Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Order> _collection;

    public OrderRepository(MongoContext context) => _collection = context.Orders;

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default)
        => await _collection.Find(FilterDefinition<Order>.Empty)
            .SortByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

    public Task AddAsync(Order order, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        order.CreatedAt = now;
        order.UpdatedAt = now;
        return _collection.InsertOneAsync(order, cancellationToken: ct);
    }

    public async Task<long> CountCreatedBetweenAsync(DateTime startInclusive, DateTime endExclusive, CancellationToken ct = default)
        => await _collection.CountDocumentsAsync(
            o => o.CreatedAt >= startInclusive && o.CreatedAt < endExclusive, cancellationToken: ct);
}
