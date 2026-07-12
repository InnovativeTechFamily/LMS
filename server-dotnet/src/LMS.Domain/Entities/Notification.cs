using LMS.Domain.Common;

namespace LMS.Domain.Entities;

/// <summary>An admin-facing notification (new order, question, review, etc.).</summary>
public class Notification : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    /// <summary>"unread" (default) or "read".</summary>
    public string Status { get; set; } = NotificationStatus.Unread;

    public string? UserId { get; set; }
}

public static class NotificationStatus
{
    public const string Unread = "unread";
    public const string Read = "read";
}
