namespace LMS.Application.DTOs.Layouts;

/// <summary>Create/edit payload for site layout content. Only the fields relevant to <see cref="Type"/> are used.</summary>
public record LayoutRequest(
    string Type,
    string? Image,
    string? Title,
    string? SubTitle,
    List<FaqInput>? Faq,
    List<CategoryInput>? Categories);

public record FaqInput(string Question, string Answer);

public record CategoryInput(string Title);
