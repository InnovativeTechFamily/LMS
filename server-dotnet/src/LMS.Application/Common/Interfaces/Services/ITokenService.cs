using LMS.Domain.Entities;

namespace LMS.Application.Common.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken(User user);

    /// <summary>Validates a refresh token and returns the user id claim, or null if invalid/expired.</summary>
    string? GetUserIdFromRefreshToken(string refreshToken);

    /// <summary>Creates a short-lived activation token embedding the pending user and a 4-digit code.</summary>
    ActivationToken CreateActivationToken(PendingRegistration registration);

    /// <summary>Validates an activation token; returns the embedded payload or null when invalid/expired.</summary>
    ActivationPayload? ValidateActivationToken(string token);
}

public record ActivationToken(string Token, string ActivationCode);

public record PendingRegistration(string Name, string Email, string Password);

public record ActivationPayload(PendingRegistration User, string ActivationCode);
