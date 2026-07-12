namespace LMS.API.Models.DTOs.Courses
{
    public class CreateCourseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Categories { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? EstimatedPrice { get; set; }
        public string Tags { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string DemoUrl { get; set; } = string.Empty;
        public List<BenefitDto> Benefits { get; set; } = new();
        public List<PrerequisiteDto> Prerequisites { get; set; } = new();
    }

    public class BenefitDto
    {
        public string Title { get; set; } = string.Empty;
    }

    public class PrerequisiteDto
    {
        public string Title { get; set; } = string.Empty;
    }
}
