namespace LMS.API.Models.DTOs.Users
{
    public class UpdateUserRoleDto
    {
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
