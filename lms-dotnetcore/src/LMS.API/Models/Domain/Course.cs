using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LMS.API.Models.Domain
{
    public class Course
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("categories")]
        public string Categories { get; set; } = string.Empty;

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("estimatedPrice")]
        public decimal? EstimatedPrice { get; set; }

        [BsonElement("thumbnail")]
        public FileData? Thumbnail { get; set; }

        [BsonElement("tags")]
        public string Tags { get; set; } = string.Empty;

        [BsonElement("level")]
        public string Level { get; set; } = string.Empty;

        [BsonElement("demoUrl")]
        public string DemoUrl { get; set; } = string.Empty;

        [BsonElement("benefits")]
        public List<Benefit> Benefits { get; set; } = new();

        [BsonElement("prerequisites")]
        public List<Prerequisite> Prerequisites { get; set; } = new();

        [BsonElement("reviews")]
        public List<Review> Reviews { get; set; } = new();

        [BsonElement("courseData")]
        public List<CourseContent> CourseData { get; set; } = new();

        [BsonElement("ratings")]
        public double Ratings { get; set; } = 0;

        [BsonElement("purchased")]
        public int Purchased { get; set; } = 0;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CourseContent
    {
        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("videoUrl")]
        public string VideoUrl { get; set; } = string.Empty;

        [BsonElement("videoSection")]
        public string VideoSection { get; set; } = string.Empty;

        [BsonElement("videoLength")]
        public int VideoLength { get; set; }

        [BsonElement("videoPlayer")]
        public string VideoPlayer { get; set; } = string.Empty;

        [BsonElement("links")]
        public List<Link> Links { get; set; } = new();

        [BsonElement("suggestion")]
        public string Suggestion { get; set; } = string.Empty;

        [BsonElement("questions")]
        public List<Comment> Questions { get; set; } = new();

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Link
    {
        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("url")]
        public string Url { get; set; } = string.Empty;
    }

    public class Comment
    {
        [BsonElement("user")]
        public UserReference? User { get; set; }

        [BsonElement("question")]
        public string Question { get; set; } = string.Empty;

        [BsonElement("questionReplies")]
        public List<Comment> QuestionReplies { get; set; } = new();

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Review
    {
        [BsonElement("user")]
        public UserReference? User { get; set; }

        [BsonElement("rating")]
        public int Rating { get; set; } = 0;

        [BsonElement("comment")]
        public string Comment { get; set; } = string.Empty;

        [BsonElement("commentReplies")]
        public List<Review> CommentReplies { get; set; } = new();

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Benefit
    {
        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;
    }

    public class Prerequisite
    {
        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;
    }

    public class UserReference
    {
        [BsonElement("_id")]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("avatar")]
        public Avatar? Avatar { get; set; }
    }

    public class FileData
    {
        [BsonElement("public_id")]
        public string? PublicId { get; set; }

        [BsonElement("url")]
        public string? Url { get; set; }
    }
}
