using System.Text.Json.Serialization;
using LMS.Domain.Common;

namespace LMS.Domain.Entities;

/// <summary>
/// Application user. Equivalent to the Mongoose <c>User</c> model.
/// </summary>
public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    /// <summary>BCrypt password hash. Null for social-auth-only accounts. Never serialized to API responses.</summary>
    [JsonIgnore]
    public string? Password { get; set; }

    public Avatar? Avatar { get; set; }

    public string Role { get; set; } = UserRoles.User;

    public bool IsVerified { get; set; }

    /// <summary>Ids of courses the user has purchased/enrolled in.</summary>
    public List<EnrolledCourse> Courses { get; set; } = new();
}

/// <summary>Cloudinary image reference (public id + secure url).</summary>
public class Avatar
{
    [JsonPropertyName("public_id")]
    public string? PublicId { get; set; }

    public string? Url { get; set; }
}

/// <summary>Reference to an enrolled course. Matches the embedded <c>{ courseId }</c> shape.</summary>
public class EnrolledCourse
{
    public string? CourseId { get; set; }
}
