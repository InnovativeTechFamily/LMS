using LMS.API.Configuration;
using LMS.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace LMS.API.Services.Implementations
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<JwtTokenService> _logger;

        public JwtTokenService(IOptions<JwtSettings> jwtSettings, ILogger<JwtTokenService> logger)
        {
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public string GenerateAccessToken(string userId)
        {
            _logger.LogInformation("Generating access token for user: {UserId}", userId);
            return GenerateToken(
                userId,
                _jwtSettings.AccessTokenSecret,
                _jwtSettings.AccessTokenExpirationMinutes
            );
        }

        public string GenerateRefreshToken(string userId)
        {
            _logger.LogInformation("Generating refresh token for user: {UserId}", userId);
            return GenerateToken(
                userId,
                _jwtSettings.RefreshTokenSecret,
                _jwtSettings.RefreshTokenExpirationDays * 24 * 60
            );
        }

        public string GenerateActivationToken(string name, string email, string password, string activationCode)
        {
            _logger.LogInformation("Generating activation token for email: {Email}", email);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.ActivationTokenSecret);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, name),
                new(ClaimTypes.Email, email),
                new("password", password),
                new("activationCode", activationCode)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ActivationTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public Dictionary<string, object> VerifyActivationToken(string token)
        {
            _logger.LogInformation("Verifying activation token");
            return VerifyToken(token, _jwtSettings.ActivationTokenSecret);
        }

        public Dictionary<string, object> VerifyToken(string token, string secretKey)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(secretKey);

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var claims = principal.Claims.ToDictionary(c => c.Type, c => (object)c.Value);
                return claims;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying token");
                throw new InvalidOperationException("Invalid token", ex);
            }
        }

        private string GenerateToken(string userId, string secretKey, int expirationMinutes)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
