using LMS.API.Models.Domain;
using MongoDB.Driver;

namespace LMS.API.Services.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(string id);
        Task<List<T>> GetAllAsync();
        Task<List<T>> FindAsync(System.Linq.Expressions.Expression<System.Func<T, bool>> filter);
        Task<T> CreateAsync(T entity);
        Task<T?> UpdateAsync(string id, T entity);
        Task<bool> DeleteAsync(string id);
        Task<long> CountAsync();
    }
}
