using LMS.API.Models.Domain;
using LMS.API.Services.Repositories.Interfaces;
using MongoDB.Driver;

namespace LMS.API.Services.Repositories.Implementations
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(IMongoCollection<Order> collection, ILogger<BaseRepository<Order>> logger)
            : base(collection, logger)
        {
        }

        public async Task<List<Order>> GetByUserIdAsync(string userId)
        {
            Logger.LogInformation("Getting orders by user ID: {UserId}", userId);
            return await Collection.Find(o => o.UserId == userId).ToListAsync();
        }

        public async Task<List<Order>> GetByCourseIdAsync(string courseId)
        {
            Logger.LogInformation("Getting orders by course ID: {CourseId}", courseId);
            return await Collection.Find(o => o.CourseId == courseId).ToListAsync();
        }
    }
}
