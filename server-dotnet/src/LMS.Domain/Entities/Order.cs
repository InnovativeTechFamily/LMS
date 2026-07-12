using LMS.Domain.Common;

namespace LMS.Domain.Entities;

/// <summary>
/// A course purchase. <see cref="PaymentInfo"/> holds the raw Stripe payment payload
/// as free-form key/value data, matching the original loosely-typed <c>payment_info</c> object.
/// </summary>
public class Order : BaseEntity
{
    public string CourseId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public Dictionary<string, object?> PaymentInfo { get; set; } = new();
}
