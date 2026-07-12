using LMS.API.Models.Domain;
using LMS.API.Models.DTOs.Orders;
using LMS.API.Services.Interfaces;
using LMS.API.Exceptions;
using MongoDB.Driver;

namespace LMS.API.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<Order> _ordersCollection;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IMongoCollection<Order> ordersCollection,
            ILogger<OrderService> logger)
        {
            _ordersCollection = ordersCollection;
            _logger = logger;
        }

        public async Task<Order> CreateOrderAsync(CreateOrderDto dto, string userId)
        {
            _logger.LogInformation("Creating order for user: {UserId}, course: {CourseId}", userId, dto.CourseId);

            var order = new Order
            {
                CourseId = dto.CourseId,
                UserId = userId,
                PaymentInfo = dto.PaymentInfo != null ? new PaymentInfo
                {
                    Id = dto.PaymentInfo.Id,
                    Status = dto.PaymentInfo.Status,
                    Type = dto.PaymentInfo.Type
                } : null
            };

            await _ordersCollection.InsertOneAsync(order);
            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(string orderId)
        {
            _logger.LogInformation("Fetching order: {OrderId}", orderId);
            return await _ordersCollection.Find(o => o.Id == orderId).FirstOrDefaultAsync();
        }

        public async Task<List<Order>> GetUserOrdersAsync(string userId)
        {
            _logger.LogInformation("Fetching orders for user: {UserId}", userId);
            return await _ordersCollection.Find(o => o.UserId == userId).ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            _logger.LogInformation("Fetching all orders");
            return await _ordersCollection.Find(_ => true).ToListAsync();
        }

        public async Task<bool> DeleteOrderAsync(string orderId)
        {
            _logger.LogInformation("Deleting order: {OrderId}", orderId);
            var result = await _ordersCollection.DeleteOneAsync(o => o.Id == orderId);
            return result.DeletedCount > 0;
        }
    }
}
