using LMS.Domain.Entities;

namespace LMS.Application.Common.Interfaces.Persistence;

public interface ILayoutRepository
{
    Task<Layout?> GetByTypeAsync(string type, CancellationToken ct = default);
    Task AddAsync(Layout layout, CancellationToken ct = default);
    Task UpdateAsync(Layout layout, CancellationToken ct = default);
}
