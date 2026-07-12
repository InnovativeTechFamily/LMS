using LMS.Application.Common.Models;
using LMS.Application.DTOs.Auth;

namespace LMS.Application.Services.Abstractions;

public interface IAuthService
{
    Task<RegistrationResult> RegisterAsync(RegistrationRequest request, CancellationToken ct = default);
    Task ActivateAsync(ActivationRequest request, CancellationToken ct = default);
    Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<AuthResult> SocialAuthAsync(SocialAuthRequest request, CancellationToken ct = default);
    Task<AuthResult> RefreshAsync(string? refreshToken, CancellationToken ct = default);
    Task LogoutAsync(string userId, CancellationToken ct = default);
}
