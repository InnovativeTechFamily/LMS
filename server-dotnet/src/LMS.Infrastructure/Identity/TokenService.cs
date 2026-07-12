using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LMS.Application.Common.Interfaces.Services;
using LMS.Domain.Entities;
using LMS.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LMS.Infrastructure.Identity;

/// <summary>
/// JWT issuance/validation for access, refresh and activation tokens.
/// Claims mirror the Node payloads (<c>id</c> for the user id, plus <c>role</c>).
/// </summary>
public class TokenService : ITokenService
{
    public const string UserIdClaim = "id";

    private readonly JwtSettings _settings;
    private readonly JwtSecurityTokenHandler _handler = new();

    public TokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
        // Keep custom claim names (name/email/password/id) verbatim instead of remapping them to URIs.
        _handler.MapInboundClaims = false;
    }

    public string GenerateAccessToken(User user) =>
        Write(new[]
        {
            new Claim(UserIdClaim, user.Id!),
            new Claim(ClaimTypes.NameIdentifier, user.Id!),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.Email, user.Email),
        }, _settings.AccessTokenSecret, TimeSpan.FromMinutes(_settings.AccessTokenExpireMinutes));

    public string GenerateRefreshToken(User user) =>
        Write(new[] { new Claim(UserIdClaim, user.Id!) },
            _settings.RefreshTokenSecret, TimeSpan.FromDays(_settings.RefreshTokenExpireDays));

    public string? GetUserIdFromRefreshToken(string refreshToken)
    {
        var principal = Validate(refreshToken, _settings.RefreshTokenSecret);
        return principal?.FindFirst(UserIdClaim)?.Value;
    }

    public ActivationToken CreateActivationToken(PendingRegistration registration)
    {
        var code = RandomNumberGenerator.GetInt32(1000, 10000).ToString();

        var token = Write(new[]
        {
            new Claim("name", registration.Name),
            new Claim("email", registration.Email),
            new Claim("password", registration.Password),
            new Claim("activationCode", code),
        }, _settings.ActivationSecret, TimeSpan.FromMinutes(_settings.ActivationTokenExpireMinutes));

        return new ActivationToken(token, code);
    }

    public ActivationPayload? ValidateActivationToken(string token)
    {
        var principal = Validate(token, _settings.ActivationSecret);
        if (principal is null) return null;

        var name = principal.FindFirst("name")?.Value;
        var email = principal.FindFirst("email")?.Value;
        var password = principal.FindFirst("password")?.Value;
        var code = principal.FindFirst("activationCode")?.Value;

        if (name is null || email is null || password is null || code is null)
            return null;

        return new ActivationPayload(new PendingRegistration(name, email, password), code);
    }

    private string Write(IEnumerable<Claim> claims, string secret, TimeSpan lifetime)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(lifetime),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return _handler.WriteToken(token);
    }

    private ClaimsPrincipal? Validate(string token, string secret)
    {
        try
        {
            return _handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateIssuer = true,
                ValidIssuer = _settings.Issuer,
                ValidateAudience = true,
                ValidAudience = _settings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            }, out _);
        }
        catch
        {
            return null;
        }
    }
}
