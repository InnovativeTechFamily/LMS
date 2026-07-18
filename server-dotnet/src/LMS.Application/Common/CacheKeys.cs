namespace LMS.Application.Common;

/// <summary>Centralised cache key helpers and TTLs, mirroring the Node server's Redis usage.</summary>
public static class CacheKeys
{
    /// <summary>Sessions and cached entities are kept for 7 days, matching the original <c>EX 604800</c>.</summary>
    public static readonly TimeSpan DefaultTtl = TimeSpan.FromDays(7);

    public const string AllCourses = "allCourses";

    /// <summary>Session/entity caches are keyed directly by the entity id (user id or course id).</summary>
    public static string ForId(string id) => id;
}
