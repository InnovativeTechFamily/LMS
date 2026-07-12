namespace LMS.API.Models.DTOs.Layout
{
    public class LayoutResponseDto
    {
        public string? Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public List<FaqItemDto> Faq { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();
        public BannerDto? Banner { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class FaqItemDto
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }

    public class CategoryDto
    {
        public string Title { get; set; } = string.Empty;
    }

    public class BannerDto
    {
        public BannerImageDto? Image { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SubTitle { get; set; } = string.Empty;
    }

    public class BannerImageDto
    {
        public string? PublicId { get; set; }
        public string? Url { get; set; }
    }
}
