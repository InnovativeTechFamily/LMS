using LMS.API.Models.Domain;

namespace LMS.API.Services.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<List<User>> GetByRoleAsync(string role);
    }
}
