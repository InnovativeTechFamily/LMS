using System.Text.Json.Serialization;

namespace LMS.Application.DTOs.Orders;

public record CreateOrderRequest(
    string CourseId,
    [property: JsonPropertyName("payment_info")] Dictionary<string, object?>? PaymentInfo);

public record CreatePaymentRequest(long Amount);
