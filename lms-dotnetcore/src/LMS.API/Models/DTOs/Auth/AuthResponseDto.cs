using LMS.API.Models.DTOs.Users;

namespace LMS.API.Models.DTOs.Auth
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public UserResponseDto? User { get; set; }
    }
}
