using LMS.Application.Common.Interfaces.Persistence;
using LMS.Domain.Entities;
using MongoDB.Driver;

namespace LMS.Infrastructure.Persistence.Repositories;

public class LayoutRepository : ILayoutRepository
{
    private readonly IMongoCollection<Layout> _collection;

    public LayoutRepository(MongoContext context) => _collection = context.Layouts;

    public async Task<Layout?> GetByTypeAsync(string type, CancellationToken ct = default)
        => await _collection.Find(l => l.Type == type).FirstOrDefaultAsync(ct);

    public Task AddAsync(Layout layout, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        layout.CreatedAt = now;
        layout.UpdatedAt = now;
        return _collection.InsertOneAsync(layout, cancellationToken: ct);
    }

    public Task UpdateAsync(Layout layout, CancellationToken ct = default)
    {
        layout.UpdatedAt = DateTime.UtcNow;
        return _collection.ReplaceOneAsync(l => l.Id == layout.Id, layout, cancellationToken: ct);
    }
}
