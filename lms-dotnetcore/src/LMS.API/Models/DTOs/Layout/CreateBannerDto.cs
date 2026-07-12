namespace LMS.API.Models.DTOs.Layout
{
    public class CreateBannerDto
    {
        public string Title { get; set; } = string.Empty;
        public string SubTitle { get; set; } = string.Empty;
        public string? BannerImage { get; set; }
    }
}
