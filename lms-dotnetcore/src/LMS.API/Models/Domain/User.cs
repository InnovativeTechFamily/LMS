using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LMS.API.Models.Domain
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("password")]
        public string? Password { get; set; }

        [BsonElement("avatar")]
        public Avatar? Avatar { get; set; }

        [BsonElement("role")]
        public string Role { get; set; } = "user";

        [BsonElement("isVerified")]
        public bool IsVerified { get; set; } = false;

        [BsonElement("courses")]
        public List<string> Courses { get; set; } = new();

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Avatar
    {
        [BsonElement("public_id")]
        public string? PublicId { get; set; }

        [BsonElement("url")]
        public string? Url { get; set; }
    }
}
