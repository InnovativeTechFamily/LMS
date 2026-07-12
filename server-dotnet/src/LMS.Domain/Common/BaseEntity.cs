namespace LMS.Domain.Common;

/// <summary>
/// Base type for all persisted aggregate roots. The <see cref="Id"/> maps to the
/// MongoDB <c>_id</c> field (an ObjectId represented as a string) and timestamps
/// mirror the Mongoose <c>{ timestamps: true }</c> behaviour of the original server.
/// </summary>
public abstract class BaseEntity
{
    public string? Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
