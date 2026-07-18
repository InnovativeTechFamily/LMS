namespace LMS.Application.Common.Interfaces.Services;

/// <summary>VdoCipher OTP generation for secure video playback.</summary>
public interface IVideoService
{
    Task<string> GenerateOtpAsync(string videoId, CancellationToken ct = default);
}
