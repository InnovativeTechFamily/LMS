using LMS.API.Models.DTOs.Auth;
using LMS.API.Models.DTOs.Users;
using LMS.API.Models.Responses;
using LMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenService _tokenService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserService userService,
            IJwtTokenService tokenService,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterRequestDto dto)
        {
            _logger.LogInformation("Register request for email: {Email}", dto.Email);

            var token = await _userService.RegisterUserAsync(dto.Name, dto.Email, dto.Password);
            return Ok(ApiResponse.SuccessResponse("User registered successfully. Activation code sent to email."));
        }

        [HttpPost("activate")]
        public async Task<ActionResult<ApiResponse>> Activate([FromBody] ActivationRequestDto dto)
        {
            _logger.LogInformation("Activation request");

            await _userService.ActivateUserAsync(dto.ActivationToken, dto.ActivationCode);
            return Ok(ApiResponse.SuccessResponse("User activated successfully"));
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto dto)
        {
            _logger.LogInformation("Login request for email: {Email}", dto.Email);

            var (user, accessToken, refreshToken) = await _userService.LoginUserAsync(dto.Email, dto.Password);

            var response = new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new UserResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    IsVerified = user.IsVerified
                }
            };

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Login successful"));
        }

        [HttpPost("refresh-token")]
        public ActionResult<ApiResponse<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            _logger.LogInformation("Refresh token request");
            // Implementation for refresh token logic
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(null, "Token refreshed"));
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse>> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Logout request for user: {UserId}", userId);

            await _userService.LogoutUserAsync(userId!);
            return Ok(ApiResponse.SuccessResponse("Logged out successfully"));
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Get profile request for user: {UserId}", userId);

            var user = await _userService.GetUserByIdAsync(userId!);
            if (user == null)
                return NotFound(ApiResponse.FailureResponse("User not found"));

            var response = new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                IsVerified = user.IsVerified,
                Avatar = user.Avatar != null ? new AvatarDto { PublicId = user.Avatar.PublicId, Url = user.Avatar.Url } : null
            };

            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(response));
        }

        [Authorize]
        [HttpPut("update-profile")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> UpdateProfile([FromBody] UpdateUserInfoDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Update profile request for user: {UserId}", userId);

            var user = await _userService.UpdateUserInfoAsync(userId!, dto.Name!, dto.Email);
            if (user == null)
                return NotFound(ApiResponse.FailureResponse("User not found"));

            var response = new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(response, "Profile updated successfully"));
        }

        [Authorize]
        [HttpPut("update-password")]
        public async Task<ActionResult<ApiResponse>> UpdatePassword([FromBody] UpdatePasswordDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Update password request for user: {UserId}", userId);

            await _userService.UpdatePasswordAsync(userId!, dto.OldPassword, dto.NewPassword);
            return Ok(ApiResponse.SuccessResponse("Password updated successfully"));
        }

        [Authorize]
        [HttpPut("update-avatar")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> UpdateAvatar([FromBody] UpdateProfilePictureDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Update avatar request for user: {UserId}", userId);

            var user = await _userService.UpdateProfilePictureAsync(userId!, dto.Avatar);
            if (user == null)
                return NotFound(ApiResponse.FailureResponse("User not found"));

            var response = new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Avatar = user.Avatar != null ? new AvatarDto { PublicId = user.Avatar.PublicId, Url = user.Avatar.Url } : null
            };

            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(response, "Avatar updated successfully"));
        }

        [Authorize(Roles = "admin")]
        [HttpGet("all")]
        public async Task<ActionResult<ApiResponse<List<UserResponseDto>>>> GetAllUsers()
        {
            _logger.LogInformation("Get all users request");

            var users = await _userService.GetAllUsersAsync();
            var response = users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                IsVerified = u.IsVerified
            }).ToList();

            return Ok(ApiResponse<List<UserResponseDto>>.SuccessResponse(response));
        }
    }
}
