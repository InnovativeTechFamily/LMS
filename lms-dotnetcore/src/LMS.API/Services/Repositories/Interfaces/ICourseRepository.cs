using LMS.API.Models.Domain;

namespace LMS.API.Services.Repositories.Interfaces
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<List<Course>> GetByCategoryAsync(string category);
        Task<List<Course>> GetByLevelAsync(string level);
    }
}
