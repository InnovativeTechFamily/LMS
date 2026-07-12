using LMS.Application.Common;
using LMS.Application.Common.Exceptions;
using LMS.Application.Common.Interfaces.Persistence;
using LMS.Application.Common.Interfaces.Services;
using LMS.Application.Common.Mapping;
using LMS.Application.DTOs.Courses;
using LMS.Application.Services.Abstractions;
using LMS.Domain.Entities;

namespace LMS.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courses;
    private readonly IUserRepository _users;
    private readonly INotificationRepository _notifications;
    private readonly INotificationPublisher _publisher;
    private readonly ICacheService _cache;
    private readonly IMediaStorage _media;
    private readonly IVideoService _video;
    private readonly IEmailService _email;
    private readonly IIdGenerator _ids;

    public CourseService(
        ICourseRepository courses,
        IUserRepository users,
        INotificationRepository notifications,
        INotificationPublisher publisher,
        ICacheService cache,
        IMediaStorage media,
        IVideoService video,
        IEmailService email,
        IIdGenerator ids)
    {
        _courses = courses;
        _users = users;
        _notifications = notifications;
        _publisher = publisher;
        _cache = cache;
        _media = media;
        _video = video;
        _email = email;
        _ids = ids;
    }

    public async Task<Course> CreateAsync(CourseRequest request, CancellationToken ct = default)
    {
        var course = new Course();
        course.Apply(request);
        AssignLectureIds(course);

        if (!string.IsNullOrEmpty(request.Thumbnail))
            course.Thumbnail = await ResolveThumbnailAsync(request.Thumbnail, null, ct);

        await _courses.AddAsync(course, ct);
        return course;
    }

    public async Task<Course> EditAsync(string id, CourseRequest request, CancellationToken ct = default)
    {
        var course = await _courses.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Course not found");

        course.Apply(request);
        AssignLectureIds(course);

        if (!string.IsNullOrEmpty(request.Thumbnail))
            course.Thumbnail = await ResolveThumbnailAsync(request.Thumbnail, course.Thumbnail, ct);

        await _courses.UpdateAsync(course, ct);
        return course;
    }

    public async Task<Course?> GetSinglePublicAsync(string id, CancellationToken ct = default)
    {
        var cached = await _cache.GetAsync<Course>(CacheKeys.ForId(id), ct);
        if (cached is not null)
            return cached;

        var course = await _courses.GetByIdAsync(id, ct);
        if (course is null)
            return null;

        StripPrivateFields(course);
        await _cache.SetAsync(CacheKeys.ForId(id), course, CacheKeys.DefaultTtl, ct);
        return course;
    }

    public async Task<IReadOnlyList<Course>> GetAllPublicAsync(CancellationToken ct = default)
    {
        var courses = await _courses.GetAllPublicAsync(ct);
        await _cache.SetAsync(CacheKeys.AllCourses, courses, CacheKeys.DefaultTtl, ct);
        return courses;
    }

    public async Task<IReadOnlyList<CourseData>> GetContentForUserAsync(string userId, string courseId, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(userId, ct)
            ?? throw new UnauthorizedException();

        var owns = user.Courses.Any(c => c.CourseId == courseId);
        if (!owns)
            throw new NotFoundException("You are not eligible to access this course");

        var course = await _courses.GetByIdAsync(courseId, ct)
            ?? throw new NotFoundException("Course not found");

        return course.CourseData;
    }

    public async Task<Course> AddQuestionAsync(string userId, AddQuestionRequest request, CancellationToken ct = default)
    {
        var user = await RequireUserAsync(userId, ct);
        var course = await _courses.GetByIdAsync(request.CourseId, ct)
            ?? throw new NotFoundException("Course not found");

        var content = course.CourseData.FirstOrDefault(c => c.Id == request.ContentId)
            ?? throw new BadRequestException("Invalid content id");

        content.Questions.Add(new Comment
        {
            Id = _ids.NewId(),
            User = user.ToSummary(),
            Question = request.Question,
            QuestionReplies = new(),
        });

        await _courses.UpdateAsync(course, ct);
        await NotifyAsync(userId, "New Question Received", $"You have a new question in {content.Title}", ct);
        return course;
    }

    public async Task<Course> AddAnswerAsync(string userId, AddAnswerRequest request, CancellationToken ct = default)
    {
        var user = await RequireUserAsync(userId, ct);
        var course = await _courses.GetByIdAsync(request.CourseId, ct)
            ?? throw new NotFoundException("Course not found");

        var content = course.CourseData.FirstOrDefault(c => c.Id == request.ContentId)
            ?? throw new BadRequestException("Invalid content id");

        var question = content.Questions.FirstOrDefault(q => q.Id == request.QuestionId)
            ?? throw new BadRequestException("Invalid question id");

        question.QuestionReplies.Add(new CommentReply
        {
            Id = _ids.NewId(),
            User = user.ToSummary(),
            Answer = request.Answer,
        });

        await _courses.UpdateAsync(course, ct);

        if (question.User?.Id == userId)
        {
            await NotifyAsync(userId, "New Question Reply Received",
                $"You have a new question reply in {content.Title}", ct);
        }
        else if (question.User is not null)
        {
            await _email.SendAsync(new EmailMessage(
                To: question.User.Email,
                Subject: "Question Reply",
                Template: "question-reply",
                Data: new Dictionary<string, object?>
                {
                    ["name"] = question.User.Name,
                    ["title"] = content.Title,
                }), ct);
        }

        return course;
    }

    public async Task<Course> AddReviewAsync(string userId, string courseId, AddReviewRequest request, CancellationToken ct = default)
    {
        var user = await RequireUserAsync(userId, ct);

        var owns = user.Courses.Any(c => c.CourseId == courseId);
        if (!owns)
            throw new NotFoundException("You are not eligible to access this course");

        var course = await _courses.GetByIdAsync(courseId, ct)
            ?? throw new NotFoundException("Course not found");

        course.Reviews.Add(new Review
        {
            Id = _ids.NewId(),
            User = user.ToSummary(),
            Rating = request.Rating,
            Comment = request.Review,
        });

        course.Ratings = course.Reviews.Count > 0
            ? course.Reviews.Sum(r => r.Rating) / course.Reviews.Count
            : 0;

        await _courses.UpdateAsync(course, ct);
        await _cache.SetAsync(CacheKeys.ForId(courseId), course, CacheKeys.DefaultTtl, ct);
        await NotifyAsync(userId, "New Review Received", $"{user.Name} has given a review in {course.Name}", ct);
        return course;
    }

    public async Task<Course> AddReviewReplyAsync(string userId, AddReviewReplyRequest request, CancellationToken ct = default)
    {
        var user = await RequireUserAsync(userId, ct);
        var course = await _courses.GetByIdAsync(request.CourseId, ct)
            ?? throw new NotFoundException("Course not found");

        var review = course.Reviews.FirstOrDefault(r => r.Id == request.ReviewId)
            ?? throw new NotFoundException("Review not found");

        review.CommentReplies.Add(new ReviewReply
        {
            Id = _ids.NewId(),
            User = user.ToSummary(),
            Comment = request.Comment,
        });

        await _courses.UpdateAsync(course, ct);
        await _cache.SetAsync(CacheKeys.ForId(request.CourseId), course, CacheKeys.DefaultTtl, ct);
        return course;
    }

    public Task<IReadOnlyList<Course>> GetAllAdminAsync(CancellationToken ct = default)
        => _courses.GetAllAsync(ct);

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        var course = await _courses.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("course not found");

        await _courses.DeleteAsync(course.Id!, ct);
        await _cache.RemoveAsync(CacheKeys.ForId(id), ct);
    }

    public Task<string> GenerateVideoOtpAsync(string videoId, CancellationToken ct = default)
        => _video.GenerateOtpAsync(videoId, ct);

    private async Task<User> RequireUserAsync(string userId, CancellationToken ct)
        => await _users.GetByIdAsync(userId, ct) ?? throw new UnauthorizedException();

    /// <summary>Ensures every lecture (and any existing embedded question/review) has an ObjectId id.</summary>
    private void AssignLectureIds(Course course)
    {
        foreach (var lecture in course.CourseData)
        {
            if (string.IsNullOrEmpty(lecture.Id))
                lecture.Id = _ids.NewId();

            foreach (var question in lecture.Questions.Where(q => string.IsNullOrEmpty(q.Id)))
                question.Id = _ids.NewId();
        }

        foreach (var review in course.Reviews.Where(r => string.IsNullOrEmpty(r.Id)))
            review.Id = _ids.NewId();
    }

    private async Task<MediaFile> ResolveThumbnailAsync(string thumbnail, MediaFile? existing, CancellationToken ct)
    {
        // Existing https urls are kept as-is; base64 data URIs are uploaded to Cloudinary.
        if (thumbnail.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            return existing ?? new MediaFile { Url = thumbnail };

        if (!string.IsNullOrEmpty(existing?.PublicId))
            await _media.DeleteAsync(existing.PublicId, ct);

        var uploaded = await _media.UploadAsync(thumbnail, folder: "courses", width: null, ct);
        return new MediaFile { PublicId = uploaded.PublicId, Url = uploaded.Url };
    }

    private async Task NotifyAsync(string userId, string title, string message, CancellationToken ct)
    {
        var notification = new Notification { UserId = userId, Title = title, Message = message };
        await _notifications.AddAsync(notification, ct);
        await _publisher.PublishAsync(new { title, message }, ct);
    }

    private static void StripPrivateFields(Course course)
    {
        foreach (var content in course.CourseData)
        {
            content.VideoUrl = string.Empty;
            content.Suggestion = string.Empty;
            content.Questions = new();
            content.Links = new();
        }
    }
}
