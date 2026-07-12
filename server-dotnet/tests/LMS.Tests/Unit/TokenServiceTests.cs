using LMS.Application.Common.Interfaces.Services;
using LMS.Domain.Entities;
using LMS.Infrastructure.Configuration;
using LMS.Infrastructure.Identity;
using Microsoft.Extensions.Options;
using Xunit;

namespace LMS.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="TokenService"/> — exercises the real JWT signing/validation logic
/// with no external dependencies (only in-memory options).
/// </summary>
public class TokenServiceTests
{
    private static TokenService CreateSut()
    {
        var settings = new JwtSettings
        {
            AccessTokenSecret = "unit-test-access-secret-key-at-least-32-chars-long",
            RefreshTokenSecret = "unit-test-refresh-secret-key-at-least-32-chars-long",
            ActivationSecret = "unit-test-activation-secret-key-at-least-32-chars",
            Issuer = "LMS",
            Audience = "LMS",
        };
        return new TokenService(Options.Create(settings));
    }

    [Fact]
    public void GenerateRefreshToken_then_read_id_roundtrips()
    {
        var sut = CreateSut();
        var user = new User { Id = "64b7f0000000000000000001", Email = "a@b.com", Role = "admin" };

        var token = sut.GenerateRefreshToken(user);
        var userId = sut.GetUserIdFromRefreshToken(token);

        Assert.Equal(user.Id, userId);
    }

    [Fact]
    public void Activation_token_roundtrips_and_carries_the_code()
    {
        var sut = CreateSut();
        var registration = new PendingRegistration("Jane", "jane@example.com", "hashed-pw");

        var activation = sut.CreateActivationToken(registration);
        var payload = sut.ValidateActivationToken(activation.Token);

        Assert.NotNull(payload);
        Assert.Equal(registration.Email, payload!.User.Email);
        Assert.Equal(registration.Name, payload.User.Name);
        Assert.Equal(activation.ActivationCode, payload.ActivationCode);
        Assert.Matches(@"^\d{4}$", activation.ActivationCode);
    }

    [Fact]
    public void ValidateActivationToken_returns_null_for_a_tampered_token()
    {
        var sut = CreateSut();

        var payload = sut.ValidateActivationToken("not-a-valid-jwt");

        Assert.Null(payload);
    }
}
