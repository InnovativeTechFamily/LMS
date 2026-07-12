using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LMS.API.Models.Domain
{
    public class Layout
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("faq")]
        public List<FaqItem> Faq { get; set; } = new();

        [BsonElement("categories")]
        public List<Category> Categories { get; set; } = new();

        [BsonElement("banner")]
        public Banner? Banner { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class FaqItem
    {
        [BsonElement("question")]
        public string Question { get; set; } = string.Empty;

        [BsonElement("answer")]
        public string Answer { get; set; } = string.Empty;
    }

    public class Category
    {
        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;
    }

    public class Banner
    {
        [BsonElement("image")]
        public BannerImage? Image { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("subTitle")]
        public string SubTitle { get; set; } = string.Empty;
    }

    public class BannerImage
    {
        [BsonElement("public_id")]
        public string? PublicId { get; set; }

        [BsonElement("url")]
        public string? Url { get; set; }
    }
}
