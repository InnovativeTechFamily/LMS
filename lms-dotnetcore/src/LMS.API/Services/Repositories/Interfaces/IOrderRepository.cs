using LMS.API.Models.Domain;

namespace LMS.API.Services.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<List<Order>> GetByUserIdAsync(string userId);
        Task<List<Order>> GetByCourseIdAsync(string courseId);
    }
}
