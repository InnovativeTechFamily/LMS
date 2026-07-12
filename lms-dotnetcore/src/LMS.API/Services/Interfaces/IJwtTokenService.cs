namespace LMS.API.Services.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(string userId);
        string GenerateRefreshToken(string userId);
        string GenerateActivationToken(string name, string email, string password, string activationCode);
        Dictionary<string, object> VerifyActivationToken(string token);
        Dictionary<string, object> VerifyToken(string token, string secretKey);
    }
}
