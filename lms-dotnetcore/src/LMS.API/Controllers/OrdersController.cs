using LMS.API.Models.DTOs.Orders;
using LMS.API.Models.Responses;
using LMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Create order request for user: {UserId}", userId);

            var order = await _orderService.CreateOrderAsync(dto, userId!);
            var response = MapToOrderResponse(order);

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, ApiResponse<OrderResponseDto>.SuccessResponse(response, "Order created successfully"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> GetOrderById(string id)
        {
            _logger.LogInformation("Get order request: {OrderId}", id);

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(ApiResponse.FailureResponse("Order not found"));

            var response = MapToOrderResponse(order);
            return Ok(ApiResponse<OrderResponseDto>.SuccessResponse(response));
        }

        [HttpGet("user/my-orders")]
        public async Task<ActionResult<ApiResponse<List<OrderResponseDto>>>> GetUserOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Get user orders request for user: {UserId}", userId);

            var orders = await _orderService.GetUserOrdersAsync(userId!);
            var responses = orders.Select(MapToOrderResponse).ToList();

            return Ok(ApiResponse<List<OrderResponseDto>>.SuccessResponse(responses));
        }

        [Authorize(Roles = "admin")]
        [HttpGet("all")]
        public async Task<ActionResult<ApiResponse<List<OrderResponseDto>>>> GetAllOrders()
        {
            _logger.LogInformation("Get all orders request");

            var orders = await _orderService.GetAllOrdersAsync();
            var responses = orders.Select(MapToOrderResponse).ToList();

            return Ok(ApiResponse<List<OrderResponseDto>>.SuccessResponse(responses));
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteOrder(string id)
        {
            _logger.LogInformation("Delete order request: {OrderId}", id);

            var result = await _orderService.DeleteOrderAsync(id);
            if (!result)
                return NotFound(ApiResponse.FailureResponse("Order not found"));

            return Ok(ApiResponse.SuccessResponse("Order deleted successfully"));
        }

        private OrderResponseDto MapToOrderResponse(LMS.API.Models.Domain.Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                CourseId = order.CourseId,
                UserId = order.UserId,
                PaymentInfo = order.PaymentInfo != null ? new Orders.PaymentInfoDto
                {
                    Id = order.PaymentInfo.Id,
                    Status = order.PaymentInfo.Status,
                    Type = order.PaymentInfo.Type
                } : null,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };
        }
    }
}
