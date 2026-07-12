using LMS.Domain.Entities;

namespace LMS.Application.Common.Interfaces.Persistence;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(string id, CancellationToken ct = default);

    /// <summary>Public listing with private lecture fields (video url, questions, links, suggestion) stripped.</summary>
    Task<IReadOnlyList<Course>> GetAllPublicAsync(CancellationToken ct = default);

    /// <summary>Full listing for admin, newest first.</summary>
    Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct = default);

    Task AddAsync(Course course, CancellationToken ct = default);
    Task UpdateAsync(Course course, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
    Task<long> CountCreatedBetweenAsync(DateTime startInclusive, DateTime endExclusive, CancellationToken ct = default);
}
