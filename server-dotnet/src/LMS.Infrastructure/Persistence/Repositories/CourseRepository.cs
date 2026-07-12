using LMS.Application.Common.Interfaces.Persistence;
using LMS.Domain.Entities;
using MongoDB.Driver;

namespace LMS.Infrastructure.Persistence.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly IMongoCollection<Course> _collection;

    public CourseRepository(MongoContext context) => _collection = context.Courses;

    public async Task<Course?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _collection.Find(c => c.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<Course>> GetAllPublicAsync(CancellationToken ct = default)
        => await _collection.Find(FilterDefinition<Course>.Empty)
            .Project<Course>(PublicProjection)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct = default)
        => await _collection.Find(FilterDefinition<Course>.Empty)
            .SortByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

    public Task AddAsync(Course course, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        course.CreatedAt = now;
        course.UpdatedAt = now;
        return _collection.InsertOneAsync(course, cancellationToken: ct);
    }

    public Task UpdateAsync(Course course, CancellationToken ct = default)
    {
        course.UpdatedAt = DateTime.UtcNow;
        return _collection.ReplaceOneAsync(c => c.Id == course.Id, course, cancellationToken: ct);
    }

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _collection.DeleteOneAsync(c => c.Id == id, ct);

    public async Task<long> CountCreatedBetweenAsync(DateTime startInclusive, DateTime endExclusive, CancellationToken ct = default)
        => await _collection.CountDocumentsAsync(
            c => c.CreatedAt >= startInclusive && c.CreatedAt < endExclusive, cancellationToken: ct);

    // Mirrors the Node `-courseData.videoUrl -courseData.suggestion -courseData.questions -courseData.links` exclusion.
    private static readonly ProjectionDefinition<Course> PublicProjection =
        Builders<Course>.Projection
            .Exclude("courseData.videoUrl")
            .Exclude("courseData.suggestion")
            .Exclude("courseData.questions")
            .Exclude("courseData.links");
}
