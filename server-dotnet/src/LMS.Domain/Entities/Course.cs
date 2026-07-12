using LMS.Domain.Common;

namespace LMS.Domain.Entities;

/// <summary>
/// A course aggregate. Reviews, lectures (course data) and their questions are stored
/// as embedded documents, matching the original Mongoose schema.
/// </summary>
public class Course : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Categories { get; set; } = string.Empty;

    public double Price { get; set; }

    public double? EstimatedPrice { get; set; }

    public MediaFile? Thumbnail { get; set; }

    public string Tags { get; set; } = string.Empty;

    public string Level { get; set; } = string.Empty;

    public string DemoUrl { get; set; } = string.Empty;

    public List<TitleItem> Benefits { get; set; } = new();

    public List<TitleItem> Prerequisites { get; set; } = new();

    public List<Review> Reviews { get; set; } = new();

    public List<CourseData> CourseData { get; set; } = new();

    public double Ratings { get; set; }

    public int Purchased { get; set; }
}

/// <summary>Cloudinary media reference used for thumbnails and banners.</summary>
public class MediaFile
{
    public string? PublicId { get; set; }

    public string? Url { get; set; }
}

/// <summary>Simple <c>{ title }</c> value object used for benefits and prerequisites.</summary>
public class TitleItem
{
    public string Title { get; set; } = string.Empty;
}

/// <summary>A single lecture / section of a course.</summary>
public class CourseData : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string VideoUrl { get; set; } = string.Empty;

    public string VideoSection { get; set; } = string.Empty;

    public double VideoLength { get; set; }

    public string VideoPlayer { get; set; } = string.Empty;

    public List<Link> Links { get; set; } = new();

    public string Suggestion { get; set; } = string.Empty;

    public List<Comment> Questions { get; set; } = new();
}

/// <summary>External resource link attached to a lecture.</summary>
public class Link
{
    public string Title { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
}

/// <summary>A question asked on a lecture, with threaded replies.</summary>
public class Comment : BaseEntity
{
    public UserSummary? User { get; set; }

    public string Question { get; set; } = string.Empty;

    public List<CommentReply> QuestionReplies { get; set; } = new();
}

/// <summary>A reply to a question.</summary>
public class CommentReply : BaseEntity
{
    public UserSummary? User { get; set; }

    public string Answer { get; set; } = string.Empty;
}

/// <summary>A course review, with threaded admin replies.</summary>
public class Review : BaseEntity
{
    public UserSummary? User { get; set; }

    public double Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public List<ReviewReply> CommentReplies { get; set; } = new();
}

/// <summary>A reply to a review.</summary>
public class ReviewReply : BaseEntity
{
    public UserSummary? User { get; set; }

    public string Comment { get; set; } = string.Empty;
}

/// <summary>
/// Denormalised snapshot of a user embedded inside course sub-documents
/// (questions, reviews, replies) — mirrors how the Node server stored the whole user object.
/// </summary>
public class UserSummary
{
    public string? Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = UserRoles.User;

    public Avatar? Avatar { get; set; }
}
