using LMS.API.Models.DTOs.Courses;

namespace LMS.API.Services.Interfaces
{
    public interface ICourseService
    {
        Task<Models.Domain.Course> CreateCourseAsync(CreateCourseDto dto);
        Task<Models.Domain.Course?> GetCourseByIdAsync(string courseId);
        Task<List<Models.Domain.Course>> GetAllCoursesAsync(int page = 1, int pageSize = 10);
        Task<Models.Domain.Course?> UpdateCourseAsync(string courseId, UpdateCourseDto dto);
        Task<bool> DeleteCourseAsync(string courseId);
        Task<Models.Domain.Course?> AddQuestionAsync(string courseId, string contentId, string question, string userId);
        Task<Models.Domain.Course?> AddAnswerAsync(string courseId, string contentId, string questionId, string answer, string userId);
        Task<Models.Domain.Course?> AddReviewAsync(string courseId, AddReviewDto dto, string userId);
        Task<List<Models.Domain.Course>> GetAdminCoursesAsync(string adminId);
        Task<Models.Domain.Course?> GetCourseContentAsync(string courseId, string userId);
    }
}
