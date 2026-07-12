namespace LMS.API.Models.DTOs.Courses
{
    public class AddAnswerDto
    {
        public string CourseId { get; set; } = string.Empty;
        public string ContentId { get; set; } = string.Empty;
        public string QuestionId { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }
}
