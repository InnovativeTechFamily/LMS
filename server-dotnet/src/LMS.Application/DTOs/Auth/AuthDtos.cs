using System.Text.Json.Serialization;

namespace LMS.Application.DTOs.Auth;

public record RegistrationRequest(string Name, string Email, string Password, string? Avatar);

public record ActivationRequest(
    [property: JsonPropertyName("activation_token")] string ActivationToken,
    [property: JsonPropertyName("activation_code")] string ActivationCode);

public record LoginRequest(string Email, string Password);

public record SocialAuthRequest(string Email, string Name, string? Avatar);

public record UpdateUserInfoRequest(string? Name);

public record UpdatePasswordRequest(string OldPassword, string NewPassword);

public record UpdateProfilePictureRequest(string Avatar);

public record UpdateUserRoleRequest(string Email, string Role);
