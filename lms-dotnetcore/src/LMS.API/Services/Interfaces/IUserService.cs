using LMS.API.Models.DTOs.Users;

namespace LMS.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<string> RegisterUserAsync(string name, string email, string password);
        Task ActivateUserAsync(string token, string code);
        Task<(Models.Domain.User user, string accessToken, string refreshToken)> LoginUserAsync(string email, string password);
        Task LogoutUserAsync(string userId);
        Task<Models.Domain.User?> GetUserByIdAsync(string userId);
        Task<Models.Domain.User?> UpdateUserInfoAsync(string userId, string name, string? email);
        Task<Models.Domain.User?> UpdatePasswordAsync(string userId, string oldPassword, string newPassword);
        Task<Models.Domain.User?> UpdateProfilePictureAsync(string userId, string avatarUrl);
        Task<List<Models.Domain.User>> GetAllUsersAsync();
        Task UpdateUserRoleAsync(string userId, string role);
        Task DeleteUserAsync(string userId);
        Task<Models.Domain.User?> SocialAuthAsync(string email, string name, string? avatar);
    }
}
