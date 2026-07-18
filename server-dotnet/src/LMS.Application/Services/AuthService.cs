using LMS.Application.Common;
using LMS.Application.Common.Exceptions;
using LMS.Application.Common.Interfaces.Persistence;
using LMS.Application.Common.Interfaces.Services;
using LMS.Application.Common.Models;
using LMS.Application.DTOs.Auth;
using LMS.Application.Services.Abstractions;
using LMS.Domain.Common;
using LMS.Domain.Entities;

namespace LMS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly ICacheService _cache;
    private readonly ITokenService _tokens;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _email;

    public AuthService(
        IUserRepository users,
        ICacheService cache,
        ITokenService tokens,
        IPasswordHasher passwordHasher,
        IEmailService email)
    {
        _users = users;
        _cache = cache;
        _tokens = tokens;
        _passwordHasher = passwordHasher;
        _email = email;
    }

    public async Task<RegistrationResult> RegisterAsync(RegistrationRequest request, CancellationToken ct = default)
    {
        if (await _users.EmailExistsAsync(request.Email, ct))
            throw new BadRequestException("Email already exist");

        var pending = new PendingRegistration(request.Name, request.Email, request.Password);
        var activation = _tokens.CreateActivationToken(pending);

        await _email.SendAsync(new EmailMessage(
            To: request.Email,
            Subject: "Activate your account",
            Template: "activation-mail",
            Data: new Dictionary<string, object?>
            {
                ["user"] = new { name = request.Name },
                ["activationCode"] = activation.ActivationCode,
            }), ct);

        return new RegistrationResult(activation.Token, request.Email);
    }

    public async Task ActivateAsync(ActivationRequest request, CancellationToken ct = default)
    {
        var payload = _tokens.ValidateActivationToken(request.ActivationToken)
            ?? throw new BadRequestException("Invalid activation token");

        if (payload.ActivationCode != request.ActivationCode)
            throw new BadRequestException("Invalid activation code");

        if (await _users.EmailExistsAsync(payload.User.Email, ct))
            throw new BadRequestException("Email already exist");

        var user = new User
        {
            Name = payload.User.Name,
            Email = payload.User.Email,
            Password = _passwordHasher.Hash(payload.User.Password),
        };

        await _users.AddAsync(user, ct);
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw new BadRequestException("Please enter email and password");

        var user = await _users.GetByEmailAsync(request.Email, ct);
        if (user is null || string.IsNullOrEmpty(user.Password))
            throw new BadRequestException("Invalid email or password");

        if (!_passwordHasher.Verify(request.Password, user.Password))
            throw new BadRequestException("Invalid email or password");

        return await IssueTokensAsync(user, ct);
    }

    public async Task<AuthResult> SocialAuthAsync(SocialAuthRequest request, CancellationToken ct = default)
    {
        var user = await _users.GetByEmailAsync(request.Email, ct);
        if (user is null)
        {
            user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Avatar = string.IsNullOrEmpty(request.Avatar) ? null : new Avatar { Url = request.Avatar },
            };
            await _users.AddAsync(user, ct);
        }

        return await IssueTokensAsync(user, ct);
    }

    public async Task<AuthResult> RefreshAsync(string? refreshToken, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(refreshToken))
            throw new BadRequestException("Could not refresh token");

        var userId = _tokens.GetUserIdFromRefreshToken(refreshToken)
            ?? throw new BadRequestException("Could not refresh token");

        var user = await _cache.GetAsync<User>(CacheKeys.ForId(userId), ct)
            ?? throw new BadRequestException("Please login for access this resources!");

        return await IssueTokensAsync(user, ct);
    }

    public Task LogoutAsync(string userId, CancellationToken ct = default)
        => _cache.RemoveAsync(CacheKeys.ForId(userId), ct);

    private async Task<AuthResult> IssueTokensAsync(User user, CancellationToken ct)
    {
        var accessToken = _tokens.GenerateAccessToken(user);
        var refreshToken = _tokens.GenerateRefreshToken(user);

        // Store the session so /refresh and the current-user lookup can resolve it, mirroring the Node server.
        await _cache.SetAsync(CacheKeys.ForId(user.Id!), user, CacheKeys.DefaultTtl, ct);

        return new AuthResult(user, accessToken, refreshToken);
    }
}
