namespace LMS.Application.Common.Interfaces.Services;

/// <summary>Stripe payment abstraction.</summary>
public interface IPaymentService
{
    string PublishableKey { get; }
    Task<PaymentIntentResult> CreatePaymentIntentAsync(long amount, CancellationToken ct = default);
    Task<string> GetPaymentStatusAsync(string paymentIntentId, CancellationToken ct = default);
}

public record PaymentIntentResult(string ClientSecret);
