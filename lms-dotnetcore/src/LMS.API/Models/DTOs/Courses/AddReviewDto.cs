namespace LMS.API.Models.DTOs.Courses
{
    public class AddReviewDto
    {
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
