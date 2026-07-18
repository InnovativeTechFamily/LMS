using LMS.Application.DTOs.Auth;
using LMS.Domain.Entities;

namespace LMS.Application.Services.Abstractions;

public interface IUserService
{
    Task<User?> GetByIdAsync(string userId, CancellationToken ct = default);
    Task<User> UpdateInfoAsync(string userId, UpdateUserInfoRequest request, CancellationToken ct = default);
    Task<User> UpdatePasswordAsync(string userId, UpdatePasswordRequest request, CancellationToken ct = default);
    Task<User> UpdateAvatarAsync(string userId, UpdateProfilePictureRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default);
    Task<User> UpdateRoleAsync(UpdateUserRoleRequest request, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}
