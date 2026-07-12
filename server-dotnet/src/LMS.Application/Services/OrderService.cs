using LMS.Application.Common;
using LMS.Application.Common.Exceptions;
using LMS.Application.Common.Interfaces.Persistence;
using LMS.Application.Common.Interfaces.Services;
using LMS.Application.DTOs.Orders;
using LMS.Application.Services.Abstractions;
using LMS.Domain.Entities;

namespace LMS.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orders;
    private readonly IUserRepository _users;
    private readonly ICourseRepository _courses;
    private readonly INotificationRepository _notifications;
    private readonly INotificationPublisher _publisher;
    private readonly ICacheService _cache;
    private readonly IEmailService _email;
    private readonly IPaymentService _payment;

    public OrderService(
        IOrderRepository orders,
        IUserRepository users,
        ICourseRepository courses,
        INotificationRepository notifications,
        INotificationPublisher publisher,
        ICacheService cache,
        IEmailService email,
        IPaymentService payment)
    {
        _orders = orders;
        _users = users;
        _courses = courses;
        _notifications = notifications;
        _publisher = publisher;
        _cache = cache;
        _email = email;
        _payment = payment;
    }

    public async Task<Order> CreateAsync(string userId, CreateOrderRequest request, CancellationToken ct = default)
    {
        // Verify the Stripe payment succeeded when a payment intent id is supplied.
        if (request.PaymentInfo is not null &&
            request.PaymentInfo.TryGetValue("id", out var idValue) &&
            idValue?.ToString() is { Length: > 0 } paymentIntentId)
        {
            var status = await _payment.GetPaymentStatusAsync(paymentIntentId, ct);
            if (status != "succeeded")
                throw new BadRequestException("Payment not authorized!");
        }

        var user = await _users.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User not found");

        if (user.Courses.Any(c => c.CourseId == request.CourseId))
            throw new BadRequestException("You have already purchased this course");

        var course = await _courses.GetByIdAsync(request.CourseId, ct)
            ?? throw new NotFoundException("Course not found");

        await _email.SendAsync(new EmailMessage(
            To: user.Email,
            Subject: "Order Confirmation",
            Template: "order-confirmation",
            Data: new Dictionary<string, object?>
            {
                ["order"] = new Dictionary<string, object?>
                {
                    ["_id"] = course.Id?.Length >= 6 ? course.Id[..6] : course.Id,
                    ["name"] = course.Name,
                    ["price"] = course.Price,
                    ["date"] = DateTime.UtcNow.ToString("MMMM d, yyyy"),
                },
            }), ct);

        user.Courses.Add(new EnrolledCourse { CourseId = course.Id });
        await _cache.SetAsync(CacheKeys.ForId(user.Id!), user, CacheKeys.DefaultTtl, ct);
        await _users.UpdateAsync(user, ct);

        var notification = new Notification
        {
            UserId = user.Id,
            Title = "New Order",
            Message = $"You have a new order from {course.Name}",
        };
        await _notifications.AddAsync(notification, ct);
        await _publisher.PublishAsync(new { title = notification.Title, message = notification.Message }, ct);

        course.Purchased += 1;
        await _courses.UpdateAsync(course, ct);

        var order = new Order
        {
            CourseId = course.Id!,
            UserId = user.Id!,
            PaymentInfo = request.PaymentInfo ?? new(),
        };
        await _orders.AddAsync(order, ct);
        return order;
    }

    public Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default)
        => _orders.GetAllAsync(ct);

    public string GetStripePublishableKey() => _payment.PublishableKey;

    public Task<PaymentIntentResult> CreatePaymentAsync(CreatePaymentRequest request, CancellationToken ct = default)
        => _payment.CreatePaymentIntentAsync(request.Amount, ct);
}
