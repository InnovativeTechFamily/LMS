using LMS.Application.Common.Interfaces.Persistence;
using LMS.Domain.Entities;
using MongoDB.Driver;

namespace LMS.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _collection;

    public UserRepository(MongoContext context) => _collection = context.Users;

    public async Task<User?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _collection.Find(u => u.Id == id).FirstOrDefaultAsync(ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _collection.Find(u => u.Email == email).FirstOrDefaultAsync(ct);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        => await _collection.Find(u => u.Email == email).AnyAsync(ct);

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default)
        => await _collection.Find(FilterDefinition<User>.Empty)
            .SortByDescending(u => u.CreatedAt)
            .ToListAsync(ct);

    public Task AddAsync(User user, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        user.CreatedAt = now;
        user.UpdatedAt = now;
        return _collection.InsertOneAsync(user, cancellationToken: ct);
    }

    public Task UpdateAsync(User user, CancellationToken ct = default)
    {
        user.UpdatedAt = DateTime.UtcNow;
        return _collection.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken: ct);
    }

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _collection.DeleteOneAsync(u => u.Id == id, ct);

    public async Task<long> CountCreatedBetweenAsync(DateTime startInclusive, DateTime endExclusive, CancellationToken ct = default)
        => await _collection.CountDocumentsAsync(
            u => u.CreatedAt >= startInclusive && u.CreatedAt < endExclusive, cancellationToken: ct);
}
