using LMS.API.Models.Domain;
using LMS.API.Services.Repositories.Interfaces;
using MongoDB.Driver;

namespace LMS.API.Services.Repositories.Implementations
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IMongoCollection<User> collection, ILogger<BaseRepository<User>> logger)
            : base(collection, logger)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            Logger.LogInformation("Getting user by email: {Email}", email);
            return await Collection.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetByRoleAsync(string role)
        {
            Logger.LogInformation("Getting users by role: {Role}", role);
            return await Collection.Find(u => u.Role == role).ToListAsync();
        }
    }
}
