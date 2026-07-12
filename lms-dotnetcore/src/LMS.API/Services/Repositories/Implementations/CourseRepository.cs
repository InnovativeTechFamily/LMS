using LMS.API.Models.Domain;
using LMS.API.Services.Repositories.Interfaces;
using MongoDB.Driver;

namespace LMS.API.Services.Repositories.Implementations
{
    public class CourseRepository : BaseRepository<Course>, ICourseRepository
    {
        public CourseRepository(IMongoCollection<Course> collection, ILogger<BaseRepository<Course>> logger)
            : base(collection, logger)
        {
        }

        public async Task<List<Course>> GetByCategoryAsync(string category)
        {
            Logger.LogInformation("Getting courses by category: {Category}", category);
            return await Collection.Find(c => c.Categories == category).ToListAsync();
        }

        public async Task<List<Course>> GetByLevelAsync(string level)
        {
            Logger.LogInformation("Getting courses by level: {Level}", level);
            return await Collection.Find(c => c.Level == level).ToListAsync();
        }
    }
}
