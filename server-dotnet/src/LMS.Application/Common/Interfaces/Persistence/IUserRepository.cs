using LMS.Domain.Entities;

namespace LMS.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
    Task<long> CountCreatedBetweenAsync(DateTime startInclusive, DateTime endExclusive, CancellationToken ct = default);
}
