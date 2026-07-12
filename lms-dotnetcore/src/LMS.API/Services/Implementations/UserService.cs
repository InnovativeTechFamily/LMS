using LMS.API.Models.Domain;
using LMS.API.Services.Interfaces;
using LMS.API.Exceptions;
using LMS.API.Helpers;
using MongoDB.Driver;
using System.Security.Claims;

namespace LMS.API.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IJwtTokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly ICacheService _cacheService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IMongoCollection<User> usersCollection,
            IJwtTokenService tokenService,
            IEmailService emailService,
            ICacheService cacheService,
            ICloudinaryService cloudinaryService,
            ILogger<UserService> logger)
        {
            _usersCollection = usersCollection;
            _tokenService = tokenService;
            _emailService = emailService;
            _cacheService = cacheService;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        public async Task<string> RegisterUserAsync(string name, string email, string password)
        {
            _logger.LogInformation("Registering user with email: {Email}", email);

            if (!ValidationHelper.IsValidEmail(email))
                throw new ValidationException("Invalid email format");

            if (!ValidationHelper.IsValidPassword(password))
                throw new ValidationException("Password must be at least 6 characters long");

            var existingUser = await _usersCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
            if (existingUser != null)
                throw new ConflictException("Email already exists");

            var activationCode = TokenGenerator.GenerateActivationCode();
            var token = _tokenService.GenerateActivationToken(name, email, password, activationCode);

            try
            {
                await _emailService.SendActivationEmailAsync(email, name, activationCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send activation email");
                throw new ApiException("Failed to send activation email", ex);
            }

            return token;
        }

        public async Task ActivateUserAsync(string token, string code)
        {
            _logger.LogInformation("Activating user with code: {Code}", code);

            try
            {
                var payload = _tokenService.VerifyActivationToken(token);

                if (payload["activationCode"].ToString() != code)
                    throw new ValidationException("Invalid activation code");

                var name = payload["name"].ToString();
                var email = payload["email"].ToString();
                var password = payload["password"].ToString();

                var existingUser = await _usersCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
                if (existingUser != null)
                    throw new ConflictException("Email already exists");

                var hashedPassword = PasswordHasher.HashPassword(password);
                var newUser = new User
                {
                    Name = name,
                    Email = email,
                    Password = hashedPassword,
                    IsVerified = true,
                    Role = "user"
                };

                await _usersCollection.InsertOneAsync(newUser);
                _logger.LogInformation("User activated successfully: {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user");
                throw;
            }
        }

        public async Task<(User user, string accessToken, string refreshToken)> LoginUserAsync(string email, string password)
        {
            _logger.LogInformation("User login attempt: {Email}", email);

            var user = await _usersCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null)
                throw new UnauthorizedException("Invalid email or password");

            if (string.IsNullOrEmpty(user.Password) || !PasswordHasher.VerifyPassword(password, user.Password))
                throw new UnauthorizedException("Invalid email or password");

            var accessToken = _tokenService.GenerateAccessToken(user.Id!);
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id!);

            // Cache user
            await _cacheService.SetAsync(user.Id!, user, TimeSpan.FromDays(7));

            _logger.LogInformation("User logged in successfully: {Email}", email);
            return (user, accessToken, refreshToken);
        }

        public async Task LogoutUserAsync(string userId)
        {
            _logger.LogInformation("User logout: {UserId}", userId);
            await _cacheService.DeleteAsync(userId);
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            // Try to get from cache first
            var cachedUser = await _cacheService.GetAsync<User>(userId);
            if (cachedUser != null)
                return cachedUser;

            // Get from database
            var user = await _usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user != null)
            {
                await _cacheService.SetAsync(userId, user, TimeSpan.FromDays(7));
            }

            return user;
        }

        public async Task<User?> UpdateUserInfoAsync(string userId, string name, string? email)
        {
            _logger.LogInformation("Updating user info: {UserId}", userId);

            var update = Builders<User>.Update
                .Set(u => u.Name, name)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);

            if (!string.IsNullOrEmpty(email))
            {
                if (!ValidationHelper.IsValidEmail(email))
                    throw new ValidationException("Invalid email format");

                var existingUser = await _usersCollection.Find(u => u.Email == email && u.Id != userId).FirstOrDefaultAsync();
                if (existingUser != null)
                    throw new ConflictException("Email already exists");

                update = update.Set(u => u.Email, email);
            }

            //var user = await _usersCollection.FindOneAndUpdateAsync(
            //    u => u.Id == userId,
            //    update,
            //    new FindOneAndUpdateOptions<User> { ReturnDocument = ReturnDocument.After }
            //);
            var user = await _usersCollection.FindOneAndUpdateAsync<User>(
                u => u.Id == userId,
                update,
                new FindOneAndUpdateOptions<User> { ReturnDocument = ReturnDocument.After },
                CancellationToken.None
            );

            if (user != null)
                await _cacheService.SetAsync(userId, user, TimeSpan.FromDays(7));

            return user;
        }

        public async Task<User?> UpdatePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            _logger.LogInformation("Updating password for user: {UserId}", userId);

            if (!ValidationHelper.IsValidPassword(newPassword))
                throw new ValidationException("New password must be at least 6 characters long");

            var user = await _usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                throw new NotFoundException("User not found");

            if (string.IsNullOrEmpty(user.Password) || !PasswordHasher.VerifyPassword(oldPassword, user.Password))
                throw new UnauthorizedException("Invalid old password");

            var hashedPassword = PasswordHasher.HashPassword(newPassword);
            var update = Builders<User>.Update
                .Set(u => u.Password, hashedPassword)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);

            user = await _usersCollection.FindOneAndUpdateAsync<User>(
                  u => u.Id == userId,
                  update,
                  new FindOneAndUpdateOptions<User> { ReturnDocument = ReturnDocument.After },
                  CancellationToken.None
              );
            if (user != null)
                await _cacheService.SetAsync(userId, user, TimeSpan.FromDays(7));


            
            return user;
        }

        public async Task<User?> UpdateProfilePictureAsync(string userId, string avatarUrl)
        {
            _logger.LogInformation("Updating profile picture for user: {UserId}", userId);

            var user = await _usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                throw new NotFoundException("User not found");

            try
            {
                // Delete old avatar if exists
                if (!string.IsNullOrEmpty(user.Avatar?.PublicId))
                {
                    await _cloudinaryService.DeleteImageAsync(user.Avatar.PublicId);
                }

                // Upload new avatar
                var (publicId, url) = await _cloudinaryService.UploadImageAsync(avatarUrl, "avatars");

                var update = Builders<User>.Update
                    .Set(u => u.Avatar, new Models.Domain.Avatar { PublicId = publicId, Url = url })
                    .Set(u => u.UpdatedAt, DateTime.UtcNow);

                user = await _usersCollection.FindOneAndUpdateAsync<User>(
                 u => u.Id == userId,
                 update,
                 new FindOneAndUpdateOptions<User> { ReturnDocument = ReturnDocument.After },
                 CancellationToken.None
             );

                if (user != null)
                    await _cacheService.SetAsync(userId, user, TimeSpan.FromDays(7));

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile picture");
                throw new ApiException("Failed to update profile picture", ex);
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            _logger.LogInformation("Fetching all users");
            return await _usersCollection.Find(_ => true).ToListAsync();
        }

        public async Task UpdateUserRoleAsync(string userId, string role)
        {
            _logger.LogInformation("Updating user role: {UserId} to {Role}", userId, role);

            var validRoles = new[] { "user", "admin" };
            if (!validRoles.Contains(role))
                throw new ValidationException("Invalid role");

            var update = Builders<User>.Update
                .Set(u => u.Role, role)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);

            var user = await _usersCollection.FindOneAndUpdateAsync<User>(
                u => u.Id == userId,
                update,
                new FindOneAndUpdateOptions<User> { ReturnDocument = ReturnDocument.After },
                CancellationToken.None
            );
            user = await _usersCollection.FindOneAndUpdateAsync<User>(
                u => u.Id == userId,
                update,
                new FindOneAndUpdateOptions<User> { ReturnDocument = ReturnDocument.After },
                CancellationToken.None
            );

            if (user != null)
                await _cacheService.SetAsync(userId, user, TimeSpan.FromDays(7));
        }

        public async Task DeleteUserAsync(string userId)
        {
            _logger.LogInformation("Deleting user: {UserId}", userId);

            await _usersCollection.DeleteOneAsync(u => u.Id == userId);
            await _cacheService.DeleteAsync(userId);
        }

        public async Task<User?> SocialAuthAsync(string email, string name, string? avatar)
        {
            _logger.LogInformation("Social auth for user: {Email}", email);

            var user = await _usersCollection.Find(u => u.Email == email).FirstOrDefaultAsync();

            if (user == null)
            {
                user = new User
                {
                    Name = name,
                    Email = email,
                    IsVerified = true,
                    Role = "user"
                };

                if (!string.IsNullOrEmpty(avatar))
                {
                    try
                    {
                        var (publicId, url) = await _cloudinaryService.UploadImageAsync(avatar, "avatars");
                        user.Avatar = new Models.Domain.Avatar { PublicId = publicId, Url = url };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error uploading avatar during social auth");
                    }
                }

                await _usersCollection.InsertOneAsync(user);
            }

            await _cacheService.SetAsync(user.Id!, user, TimeSpan.FromDays(7));
            return user;
        }
    }
}
