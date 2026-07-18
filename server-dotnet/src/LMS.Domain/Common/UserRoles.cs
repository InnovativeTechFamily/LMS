namespace LMS.Domain.Common;

/// <summary>
/// Well-known role names. Mirrors the string-based roles used by the Node server
/// (default "user", elevated "admin").
/// </summary>
public static class UserRoles
{
    public const string User = "user";
    public const string Admin = "admin";
}
