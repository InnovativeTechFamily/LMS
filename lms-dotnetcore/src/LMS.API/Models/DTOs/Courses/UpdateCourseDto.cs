using LMS.API.Models.DTOs.Courses;

namespace LMS.API.Models.DTOs.Courses
{
    public class UpdateCourseDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Categories { get; set; }
        public decimal? Price { get; set; }
        public decimal? EstimatedPrice { get; set; }
        public string? Tags { get; set; }
        public string? Level { get; set; }
        public string? DemoUrl { get; set; }
        public List<BenefitDto>? Benefits { get; set; }
        public List<PrerequisiteDto>? Prerequisites { get; set; }
    }
}
