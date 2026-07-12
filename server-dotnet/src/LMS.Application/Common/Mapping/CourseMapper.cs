using LMS.Application.DTOs.Courses;
using LMS.Domain.Entities;

namespace LMS.Application.Common.Mapping;

/// <summary>Maps course request DTOs onto the <see cref="Course"/> aggregate.</summary>
public static class CourseMapper
{
    public static UserSummary ToSummary(this User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        Role = user.Role,
        Avatar = user.Avatar,
    };

    /// <summary>Applies the non-null fields of <paramref name="request"/> onto <paramref name="course"/>.</summary>
    public static void Apply(this Course course, CourseRequest request)
    {
        if (request.Name is not null) course.Name = request.Name;
        if (request.Description is not null) course.Description = request.Description;
        if (request.Categories is not null) course.Categories = request.Categories;
        if (request.Price is not null) course.Price = request.Price.Value;
        if (request.EstimatedPrice is not null) course.EstimatedPrice = request.EstimatedPrice;
        if (request.Tags is not null) course.Tags = request.Tags;
        if (request.Level is not null) course.Level = request.Level;
        if (request.DemoUrl is not null) course.DemoUrl = request.DemoUrl;

        if (request.Benefits is not null)
            course.Benefits = request.Benefits.Select(b => new TitleItem { Title = b.Title }).ToList();

        if (request.Prerequisites is not null)
            course.Prerequisites = request.Prerequisites.Select(p => new TitleItem { Title = p.Title }).ToList();

        if (request.CourseData is not null)
            course.CourseData = request.CourseData.Select(MapCourseData).ToList();
    }

    private static CourseData MapCourseData(CourseDataInput input) => new()
    {
        Title = input.Title ?? string.Empty,
        Description = input.Description ?? string.Empty,
        VideoUrl = input.VideoUrl ?? string.Empty,
        VideoSection = input.VideoSection ?? string.Empty,
        VideoLength = input.VideoLength ?? 0,
        VideoPlayer = input.VideoPlayer ?? string.Empty,
        Suggestion = input.Suggestion ?? string.Empty,
        Links = (input.Links ?? new()).Select(l => new Link { Title = l.Title, Url = l.Url }).ToList(),
    };
}
