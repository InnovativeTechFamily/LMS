namespace LMS.API.Models.DTOs.Courses
{
    public class CourseResponseDto
    {
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Categories { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? EstimatedPrice { get; set; }
        public FileDataDto? Thumbnail { get; set; }
        public string Tags { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string DemoUrl { get; set; } = string.Empty;
        public List<BenefitDto> Benefits { get; set; } = new();
        public List<PrerequisiteDto> Prerequisites { get; set; } = new();
        public List<ReviewDto> Reviews { get; set; } = new();
        public double Ratings { get; set; }
        public int Purchased { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class FileDataDto
    {
        public string? PublicId { get; set; }
        public string? Url { get; set; }
    }

    public class ReviewDto
    {
        public UserReferenceDto? User { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class UserReferenceDto
    {
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
