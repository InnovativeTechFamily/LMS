using LMS.Application.Common;
using LMS.Application.Common.Exceptions;
using LMS.Application.Common.Interfaces.Persistence;
using LMS.Application.Common.Interfaces.Services;
using LMS.Application.DTOs.Auth;
using LMS.Application.Services.Abstractions;
using LMS.Domain.Entities;

namespace LMS.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly ICacheService _cache;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMediaStorage _media;

    public UserService(
        IUserRepository users,
        ICacheService cache,
        IPasswordHasher passwordHasher,
        IMediaStorage media)
    {
        _users = users;
        _cache = cache;
        _passwordHasher = passwordHasher;
        _media = media;
    }

    public async Task<User?> GetByIdAsync(string userId, CancellationToken ct = default)
    {
        var cached = await _cache.GetAsync<User>(CacheKeys.ForId(userId), ct);
        if (cached is not null)
            return cached;

        var user = await _users.GetByIdAsync(userId, ct);
        if (user is not null)
            await _cache.SetAsync(CacheKeys.ForId(user.Id!), user, CacheKeys.DefaultTtl, ct);

        return user;
    }

    public async Task<User> UpdateInfoAsync(string userId, UpdateUserInfoRequest request, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User not found");

        if (!string.IsNullOrWhiteSpace(request.Name))
            user.Name = request.Name;

        await _users.UpdateAsync(user, ct);
        await _cache.SetAsync(CacheKeys.ForId(user.Id!), user, CacheKeys.DefaultTtl, ct);
        return user;
    }

    public async Task<User> UpdatePasswordAsync(string userId, UpdatePasswordRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(request.OldPassword) || string.IsNullOrEmpty(request.NewPassword))
            throw new BadRequestException("Please enter old and new password");

        var user = await _users.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User not found");

        if (string.IsNullOrEmpty(user.Password))
            throw new BadRequestException("Invalid user");

        if (!_passwordHasher.Verify(request.OldPassword, user.Password))
            throw new BadRequestException("Invalid old password");

        user.Password = _passwordHasher.Hash(request.NewPassword);

        await _users.UpdateAsync(user, ct);
        await _cache.SetAsync(CacheKeys.ForId(user.Id!), user, CacheKeys.DefaultTtl, ct);
        return user;
    }

    public async Task<User> UpdateAvatarAsync(string userId, UpdateProfilePictureRequest request, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User not found");

        if (!string.IsNullOrEmpty(request.Avatar))
        {
            if (!string.IsNullOrEmpty(user.Avatar?.PublicId))
                await _media.DeleteAsync(user.Avatar.PublicId, ct);

            var uploaded = await _media.UploadAsync(request.Avatar, folder: "avatars", width: 150, ct);
            user.Avatar = new Avatar { PublicId = uploaded.PublicId, Url = uploaded.Url };
        }

        await _users.UpdateAsync(user, ct);
        await _cache.SetAsync(CacheKeys.ForId(user.Id!), user, CacheKeys.DefaultTtl, ct);
        return user;
    }

    public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default)
        => _users.GetAllAsync(ct);

    public async Task<User> UpdateRoleAsync(UpdateUserRoleRequest request, CancellationToken ct = default)
    {
        var user = await _users.GetByEmailAsync(request.Email, ct)
            ?? throw new NotFoundException("User not found");

        user.Role = request.Role;
        await _users.UpdateAsync(user, ct);
        await _cache.SetAsync(CacheKeys.ForId(user.Id!), user, CacheKeys.DefaultTtl, ct);
        return user;
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("User not found");

        await _users.DeleteAsync(user.Id!, ct);
        await _cache.RemoveAsync(CacheKeys.ForId(user.Id!), ct);
    }
}
