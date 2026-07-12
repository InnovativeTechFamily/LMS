using LMS.API.Models.DTOs.Layout;

namespace LMS.API.Services.Interfaces
{
    public interface ILayoutService
    {
        Task<Models.Domain.Layout> GetLayoutByTypeAsync(string type);
        Task<Models.Domain.Layout> CreateOrUpdateLayoutAsync(string type, LayoutResponseDto dto);
        Task<Models.Domain.Layout?> AddFaqAsync(string layoutId, CreateFaqDto dto);
        Task<Models.Domain.Layout?> UpdateFaqAsync(string layoutId, int faqIndex, CreateFaqDto dto);
        Task<bool> DeleteFaqAsync(string layoutId, int faqIndex);
        Task<Models.Domain.Layout?> AddCategoryAsync(string layoutId, CreateCategoryDto dto);
        Task<Models.Domain.Layout?> UpdateCategoryAsync(string layoutId, int categoryIndex, CreateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(string layoutId, int categoryIndex);
        Task<Models.Domain.Layout?> UpdateBannerAsync(string layoutId, CreateBannerDto dto);
    }
}
