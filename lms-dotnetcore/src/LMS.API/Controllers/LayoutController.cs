using LMS.API.Models.DTOs.Layout;
using LMS.API.Models.Responses;
using LMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LayoutController : ControllerBase
    {
        private readonly ILayoutService _layoutService;
        private readonly ILogger<LayoutController> _logger;

        public LayoutController(ILayoutService layoutService, ILogger<LayoutController> logger)
        {
            _layoutService = layoutService;
            _logger = logger;
        }

        [HttpGet("{type}")]
        public async Task<ActionResult<ApiResponse<LayoutResponseDto>>> GetLayoutByType(string type)
        {
            _logger.LogInformation("Get layout request for type: {Type}", type);

            var layout = await _layoutService.GetLayoutByTypeAsync(type);
            var response = MapToLayoutResponse(layout);

            return Ok(ApiResponse<LayoutResponseDto>.SuccessResponse(response));
        }

        [Authorize(Roles = "admin")]
        [HttpPost("{type}")]
        public async Task<ActionResult<ApiResponse<LayoutResponseDto>>> CreateOrUpdateLayout(string type, [FromBody] LayoutResponseDto dto)
        {
            _logger.LogInformation("Create/Update layout request for type: {Type}", type);

            var layout = await _layoutService.CreateOrUpdateLayoutAsync(type, dto);
            var response = MapToLayoutResponse(layout);

            return Ok(ApiResponse<LayoutResponseDto>.SuccessResponse(response, "Layout saved successfully"));
        }

        [Authorize(Roles = "admin")]
        [HttpPost("{layoutId}/faq")]
        public async Task<ActionResult<ApiResponse<LayoutResponseDto>>> AddFaq(string layoutId, [FromBody] CreateFaqDto dto)
        {
            _logger.LogInformation("Add FAQ request for layout: {LayoutId}", layoutId);

            var layout = await _layoutService.AddFaqAsync(layoutId, dto);
            if (layout == null)
                return NotFound(ApiResponse.FailureResponse("Layout not found"));

            var response = MapToLayoutResponse(layout);
            return Ok(ApiResponse<LayoutResponseDto>.SuccessResponse(response, "FAQ added successfully"));
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{layoutId}/faq/{faqIndex}")]
        public async Task<ActionResult<ApiResponse<LayoutResponseDto>>> UpdateFaq(string layoutId, int faqIndex, [FromBody] CreateFaqDto dto)
        {
            _logger.LogInformation("Update FAQ request for layout: {LayoutId} at index: {Index}", layoutId, faqIndex);

            var layout = await _layoutService.UpdateFaqAsync(layoutId, faqIndex, dto);
            if (layout == null)
                return NotFound(ApiResponse.FailureResponse("Layout not found"));

            var response = MapToLayoutResponse(layout);
            return Ok(ApiResponse<LayoutResponseDto>.SuccessResponse(response, "FAQ updated successfully"));
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{layoutId}/faq/{faqIndex}")]
        public async Task<ActionResult<ApiResponse>> DeleteFaq(string layoutId, int faqIndex)
        {
            _logger.LogInformation("Delete FAQ request for layout: {LayoutId} at index: {Index}", layoutId, faqIndex);

            var result = await _layoutService.DeleteFaqAsync(layoutId, faqIndex);
            if (!result)
                return NotFound(ApiResponse.FailureResponse("FAQ not found"));

            return Ok(ApiResponse.SuccessResponse("FAQ deleted successfully"));
        }

        private LayoutResponseDto MapToLayoutResponse(LMS.API.Models.Domain.Layout layout)
        {
            return new LayoutResponseDto
            {
                Id = layout.Id,
                Type = layout.Type,
                Faq = layout.Faq.Select(f => new FaqItemDto { Question = f.Question, Answer = f.Answer }).ToList(),
                Categories = layout.Categories.Select(c => new CategoryDto { Title = c.Title }).ToList(),
                CreatedAt = layout.CreatedAt,
                UpdatedAt = layout.UpdatedAt
            };
        }
    }
}
