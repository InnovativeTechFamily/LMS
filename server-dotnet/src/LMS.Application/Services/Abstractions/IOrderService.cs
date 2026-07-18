using LMS.Application.Common.Interfaces.Services;
using LMS.Application.DTOs.Orders;
using LMS.Domain.Entities;

namespace LMS.Application.Services.Abstractions;

public interface IOrderService
{
    Task<Order> CreateAsync(string userId, CreateOrderRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default);
    string GetStripePublishableKey();
    Task<PaymentIntentResult> CreatePaymentAsync(CreatePaymentRequest request, CancellationToken ct = default);
}
