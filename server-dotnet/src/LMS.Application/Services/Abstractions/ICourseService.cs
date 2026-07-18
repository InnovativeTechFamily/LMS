using LMS.Application.DTOs.Courses;
using LMS.Domain.Entities;

namespace LMS.Application.Services.Abstractions;

public interface ICourseService
{
    Task<Course> CreateAsync(CourseRequest request, CancellationToken ct = default);
    Task<Course> EditAsync(string id, CourseRequest request, CancellationToken ct = default);
    Task<Course?> GetSinglePublicAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<Course>> GetAllPublicAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CourseData>> GetContentForUserAsync(string userId, string courseId, CancellationToken ct = default);
    Task<Course> AddQuestionAsync(string userId, AddQuestionRequest request, CancellationToken ct = default);
    Task<Course> AddAnswerAsync(string userId, AddAnswerRequest request, CancellationToken ct = default);
    Task<Course> AddReviewAsync(string userId, string courseId, AddReviewRequest request, CancellationToken ct = default);
    Task<Course> AddReviewReplyAsync(string userId, AddReviewReplyRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<Course>> GetAllAdminAsync(CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
    Task<string> GenerateVideoOtpAsync(string videoId, CancellationToken ct = default);
}
