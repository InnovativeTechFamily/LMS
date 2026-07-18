using LMS.Application.Common.Interfaces.Services;

namespace LMS.IntegrationTests.Fakes;

/// <summary>Returns deterministic media without calling Cloudinary.</summary>
public class FakeMediaStorage : IMediaStorage
{
    public Task<UploadedMedia> UploadAsync(string fileOrDataUri, string folder, int? width = null, CancellationToken ct = default)
        => Task.FromResult(new UploadedMedia($"{folder}/fake-public-id", $"https://cdn.test/{folder}/fake.png"));

    public Task DeleteAsync(string publicId, CancellationToken ct = default) => Task.CompletedTask;
}

/// <summary>Stripe stand-in — always reports success.</summary>
public class FakePaymentService : IPaymentService
{
    public string PublishableKey => "pk_test_fake";

    public Task<PaymentIntentResult> CreatePaymentIntentAsync(long amount, CancellationToken ct = default)
        => Task.FromResult(new PaymentIntentResult("cs_test_fake_secret"));

    public Task<string> GetPaymentStatusAsync(string paymentIntentId, CancellationToken ct = default)
        => Task.FromResult("succeeded");
}

/// <summary>VdoCipher stand-in — returns a canned OTP payload.</summary>
public class FakeVideoService : IVideoService
{
    public Task<string> GenerateOtpAsync(string videoId, CancellationToken ct = default)
        => Task.FromResult("{\"otp\":\"fake-otp\",\"playbackInfo\":\"fake-playback\"}");
}
