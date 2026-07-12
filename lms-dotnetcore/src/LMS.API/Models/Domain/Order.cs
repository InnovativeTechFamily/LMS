using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LMS.API.Models.Domain
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("courseId")]
        public string CourseId { get; set; } = string.Empty;

        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("payment_info")]
        public PaymentInfo? PaymentInfo { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class PaymentInfo
    {
        [BsonElement("id")]
        public string? Id { get; set; }

        [BsonElement("status")]
        public string? Status { get; set; }

        [BsonElement("type")]
        public string? Type { get; set; }
    }
}
