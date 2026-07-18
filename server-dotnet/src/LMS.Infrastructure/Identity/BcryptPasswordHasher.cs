using LMS.Application.Common.Interfaces.Services;

namespace LMS.Infrastructure.Identity;

/// <summary>BCrypt password hashing, matching the Node server's bcryptjs usage (work factor 10).</summary>
public class BcryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 10;

    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
