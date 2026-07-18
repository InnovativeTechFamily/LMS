using LMS.Domain.Common;

namespace LMS.Domain.Entities;

/// <summary>
/// Site layout content keyed by <see cref="Type"/> ("Banner", "FAQ" or "Categories").
/// Mirrors the single-document-per-type Mongoose <c>Layout</c> model.
/// </summary>
public class Layout : BaseEntity
{
    public string Type { get; set; } = string.Empty;

    public List<FaqItem> Faq { get; set; } = new();

    public List<Category> Categories { get; set; } = new();

    public Banner? Banner { get; set; }
}

public class FaqItem
{
    public string Question { get; set; } = string.Empty;

    public string Answer { get; set; } = string.Empty;
}

public class Category
{
    public string Title { get; set; } = string.Empty;
}

public class Banner
{
    public MediaFile? Image { get; set; }

    public string Title { get; set; } = string.Empty;

    public string SubTitle { get; set; } = string.Empty;
}

public static class LayoutTypes
{
    public const string Banner = "Banner";
    public const string Faq = "FAQ";
    public const string Categories = "Categories";
}
