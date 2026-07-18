using System.Net.Http.Headers;
using System.Text;
using LMS.Application.Common.Interfaces.Services;
using LMS.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace LMS.Infrastructure.Video;

/// <summary>Requests a playback OTP from VdoCipher for a given video id.</summary>
public class VdoCipherVideoService : IVideoService
{
    private readonly HttpClient _http;
    private readonly VdoCipherSettings _settings;

    public VdoCipherVideoService(HttpClient http, IOptions<VdoCipherSettings> options)
    {
        _http = http;
        _settings = options.Value;
    }

    public async Task<string> GenerateOtpAsync(string videoId, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post, $"https://dev.vdocipher.com/api/videos/{videoId}/otp")
        {
            Content = new StringContent("{\"ttl\":300}", Encoding.UTF8, "application/json"),
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.TryAddWithoutValidation("Authorization", $"Apisecret {_settings.ApiSecret}");

        var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(ct);
    }
}
