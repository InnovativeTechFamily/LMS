using LMS.Application.Common.Interfaces.Services;
using LMS.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Stripe;

namespace LMS.Infrastructure.Payments;

/// <summary>Stripe implementation of <see cref="IPaymentService"/>.</summary>
public class StripePaymentService : IPaymentService
{
    private readonly StripeSettings _settings;
    private readonly PaymentIntentService _paymentIntents;

    public StripePaymentService(IOptions<StripeSettings> options)
    {
        _settings = options.Value;
        StripeConfiguration.ApiKey = _settings.SecretKey;
        _paymentIntents = new PaymentIntentService();
    }

    public string PublishableKey => _settings.PublishableKey;

    public async Task<PaymentIntentResult> CreatePaymentIntentAsync(long amount, CancellationToken ct = default)
    {
        var intent = await _paymentIntents.CreateAsync(new PaymentIntentCreateOptions
        {
            Amount = amount,
            Currency = "inr",
            Description = "E-learning course services",
            Metadata = new Dictionary<string, string> { ["company"] = "E-Learning" },
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true },
        }, cancellationToken: ct);

        return new PaymentIntentResult(intent.ClientSecret);
    }

    public async Task<string> GetPaymentStatusAsync(string paymentIntentId, CancellationToken ct = default)
    {
        var intent = await _paymentIntents.GetAsync(paymentIntentId, cancellationToken: ct);
        return intent.Status;
    }
}
