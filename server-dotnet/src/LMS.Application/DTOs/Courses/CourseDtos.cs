namespace LMS.Application.DTOs.Courses;

/// <summary>Create/edit payload for a course. Thumbnail is a base64 data URI (new) or an existing https url.</summary>
public record CourseRequest(
    string? Name,
    string? Description,
    string? Categories,
    double? Price,
    double? EstimatedPrice,
    string? Thumbnail,
    string? Tags,
    string? Level,
    string? DemoUrl,
    List<TitleItemInput>? Benefits,
    List<TitleItemInput>? Prerequisites,
    List<CourseDataInput>? CourseData);

public record TitleItemInput(string Title);

public record LinkInput(string Title, string Url);

public record CourseDataInput(
    string? Title,
    string? Description,
    string? VideoUrl,
    string? VideoSection,
    double? VideoLength,
    string? VideoPlayer,
    List<LinkInput>? Links,
    string? Suggestion);

public record AddQuestionRequest(string Question, string CourseId, string ContentId);

public record AddAnswerRequest(string Answer, string CourseId, string ContentId, string QuestionId);

public record AddReviewRequest(string Review, double Rating);

public record AddReviewReplyRequest(string Comment, string CourseId, string ReviewId);

public record GenerateVideoUrlRequest(string VideoId);
