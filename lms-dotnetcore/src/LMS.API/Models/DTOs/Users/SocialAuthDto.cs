namespace LMS.API.Models.DTOs.Users
{
    public class SocialAuthDto
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Avatar { get; set; }
    }
}
