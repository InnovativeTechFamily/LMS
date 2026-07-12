namespace LMS.API.Models.DTOs.Courses
{
    public class AddQuestionDto
    {
        public string CourseId { get; set; } = string.Empty;
        public string ContentId { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
    }
}
