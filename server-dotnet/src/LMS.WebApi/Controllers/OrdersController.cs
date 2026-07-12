using LMS.Application.DTOs.Orders;
using LMS.Application.Services.Abstractions;
using LMS.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.WebApi.Controllers;

/// <summary>Orders and Stripe payments (mirrors order.route.ts).</summary>
public class OrdersController : ApiControllerBase
{
    private readonly IOrderService _orders;

    public OrdersController(IOrderService orders) => _orders = orders;

    [Authorize]
    [HttpPost("create-order")]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        var order = await _orders.CreateAsync(CurrentUserId, request, ct);
        return StatusCode(201, new { success = true, order });
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet("get-orders")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var orders = await _orders.GetAllAsync(ct);
        return StatusCode(201, new { success = true, orders });
    }

    [HttpGet("payment/stripepublishablekey")]
    public IActionResult GetPublishableKey()
        => Ok(new { publishablekey = _orders.GetStripePublishableKey() });

    [Authorize]
    [HttpPost("payment")]
    public async Task<IActionResult> NewPayment([FromBody] CreatePaymentRequest request, CancellationToken ct)
    {
        var result = await _orders.CreatePaymentAsync(request, ct);
        return StatusCode(201, new { success = true, client_secret = result.ClientSecret });
    }
}
