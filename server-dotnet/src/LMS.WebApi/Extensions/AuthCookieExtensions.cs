using LMS.Application.Common.Models;
using LMS.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace LMS.WebApi.Extensions;

/// <summary>Writes the access/refresh token cookies, mirroring the Node <c>sendToken</c> helper.</summary>
public static class AuthCookieExtensions
{
    public const string AccessTokenCookie = "access_token";
    public const string RefreshTokenCookie = "refresh_token";

    public static void SetAuthCookies(this HttpResponse response, AuthResult auth, JwtSettings jwt)
    {
        response.Cookies.Append(AccessTokenCookie, auth.AccessToken, BuildOptions(
            TimeSpan.FromMinutes(jwt.AccessTokenExpireMinutes)));

        response.Cookies.Append(RefreshTokenCookie, auth.RefreshToken, BuildOptions(
            TimeSpan.FromDays(jwt.RefreshTokenExpireDays)));
    }

    public static void ClearAuthCookies(this HttpResponse response)
    {
        response.Cookies.Delete(AccessTokenCookie);
        response.Cookies.Delete(RefreshTokenCookie);
    }

    private static CookieOptions BuildOptions(TimeSpan lifetime) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.None,
        Expires = DateTimeOffset.UtcNow.Add(lifetime),
        MaxAge = lifetime,
    };
}
