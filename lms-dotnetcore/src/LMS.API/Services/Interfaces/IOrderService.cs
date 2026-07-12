using LMS.API.Models.DTOs.Orders;

namespace LMS.API.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Models.Domain.Order> CreateOrderAsync(CreateOrderDto dto, string userId);
        Task<Models.Domain.Order?> GetOrderByIdAsync(string orderId);
        Task<List<Models.Domain.Order>> GetUserOrdersAsync(string userId);
        Task<List<Models.Domain.Order>> GetAllOrdersAsync();
        Task<bool> DeleteOrderAsync(string orderId);
    }
}
