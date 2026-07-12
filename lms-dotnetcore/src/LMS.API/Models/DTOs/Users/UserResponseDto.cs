namespace LMS.API.Models.DTOs.Users
{
    public class UserResponseDto
    {
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public AvatarDto? Avatar { get; set; }
        public string Role { get; set; } = "user";
        public bool IsVerified { get; set; }
        public List<string> Courses { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class AvatarDto
    {
        public string? PublicId { get; set; }
        public string? Url { get; set; }
    }
}
