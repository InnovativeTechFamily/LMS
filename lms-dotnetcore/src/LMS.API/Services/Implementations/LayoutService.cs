using LMS.API.Models.Domain;
using LMS.API.Models.DTOs.Layout;
using LMS.API.Services.Interfaces;
using LMS.API.Exceptions;
using MongoDB.Driver;

namespace LMS.API.Services.Implementations
{
    public class LayoutService : ILayoutService
    {
        private readonly IMongoCollection<Layout> _layoutsCollection;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<LayoutService> _logger;

        public LayoutService(
            IMongoCollection<Layout> layoutsCollection,
            ICloudinaryService cloudinaryService,
            ILogger<LayoutService> logger)
        {
            _layoutsCollection = layoutsCollection;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        public async Task<Layout> GetLayoutByTypeAsync(string type)
        {
            _logger.LogInformation("Fetching layout of type: {Type}", type);

            var layout = await _layoutsCollection.Find(l => l.Type == type).FirstOrDefaultAsync();
            return layout ?? throw new NotFoundException($"Layout of type '{type}' not found");
        }

        public async Task<Layout> CreateOrUpdateLayoutAsync(string type, LayoutResponseDto dto)
        {
            _logger.LogInformation("Creating or updating layout of type: {Type}", type);

            var existingLayout = await _layoutsCollection.Find(l => l.Type == type).FirstOrDefaultAsync();

            if (existingLayout == null)
            {
                var newLayout = new Layout
                {
                    Type = type,
                    Faq = dto.Faq.Select(f => new FaqItem { Question = f.Question, Answer = f.Answer }).ToList(),
                    Categories = dto.Categories.Select(c => new Category { Title = c.Title }).ToList()
                };

                await _layoutsCollection.InsertOneAsync(newLayout);
                return newLayout;
            }
            else
            {
                var update = Builders<Layout>.Update
                    .Set(l => l.Faq, dto.Faq.Select(f => new FaqItem { Question = f.Question, Answer = f.Answer }).ToList())
                    .Set(l => l.Categories, dto.Categories.Select(c => new Category { Title = c.Title }).ToList())
                    .Set(l => l.UpdatedAt, DateTime.UtcNow);

                return await _layoutsCollection.FindOneAndUpdateAsync(
                    l => l.Type == type,
                    update,
                    new FindOneAndUpdateOptions<Layout> { ReturnDocument = ReturnDocument.After }
                ) ?? throw new NotFoundException($"Layout of type '{type}' not found");
            }
        }

        public async Task<Layout?> AddFaqAsync(string layoutId, CreateFaqDto dto)
        {
            _logger.LogInformation("Adding FAQ to layout: {LayoutId}", layoutId);

            var faqItem = new FaqItem { Question = dto.Question, Answer = dto.Answer };
            var update = Builders<Layout>.Update.Push(l => l.Faq, faqItem);

            return await _layoutsCollection.FindOneAndUpdateAsync(
                l => l.Id == layoutId,
                update,
                new FindOneAndUpdateOptions<Layout> { ReturnDocument = ReturnDocument.After }
            );
        }

        public async Task<Layout?> UpdateFaqAsync(string layoutId, int faqIndex, CreateFaqDto dto)
        {
            _logger.LogInformation("Updating FAQ at index {Index} in layout: {LayoutId}", faqIndex, layoutId);

            var update = Builders<Layout>.Update
                .Set(l => l.Faq[faqIndex].Question, dto.Question)
                .Set(l => l.Faq[faqIndex].Answer, dto.Answer);

            return await _layoutsCollection.FindOneAndUpdateAsync(
                l => l.Id == layoutId,
                update,
                new FindOneAndUpdateOptions<Layout> { ReturnDocument = ReturnDocument.After }
            );
        }

        public async Task<bool> DeleteFaqAsync(string layoutId, int faqIndex)
        {
            _logger.LogInformation("Deleting FAQ at index {Index} from layout: {LayoutId}", faqIndex, layoutId);

            var layout = await _layoutsCollection.Find(l => l.Id == layoutId).FirstOrDefaultAsync();
            if (layout == null || faqIndex >= layout.Faq.Count)
                return false;

            layout.Faq.RemoveAt(faqIndex);
            var update = Builders<Layout>.Update.Set(l => l.Faq, layout.Faq);

            var result = await _layoutsCollection.UpdateOneAsync(
                l => l.Id == layoutId,
                update
            );

            return result.ModifiedCount > 0;
        }

        public async Task<Layout?> AddCategoryAsync(string layoutId, CreateCategoryDto dto)
        {
            _logger.LogInformation("Adding category to layout: {LayoutId}", layoutId);

            var category = new Category { Title = dto.Title };
            var update = Builders<Layout>.Update.Push(l => l.Categories, category);

            return await _layoutsCollection.FindOneAndUpdateAsync(
                l => l.Id == layoutId,
                update,
                new FindOneAndUpdateOptions<Layout> { ReturnDocument = ReturnDocument.After }
            );
        }

        public async Task<Layout?> UpdateCategoryAsync(string layoutId, int categoryIndex, CreateCategoryDto dto)
        {
            _logger.LogInformation("Updating category at index {Index} in layout: {LayoutId}", categoryIndex, layoutId);

            var update = Builders<Layout>.Update.Set(l => l.Categories[categoryIndex].Title, dto.Title);

            return await _layoutsCollection.FindOneAndUpdateAsync(
                l => l.Id == layoutId,
                update,
                new FindOneAndUpdateOptions<Layout> { ReturnDocument = ReturnDocument.After }
            );
        }

        public async Task<bool> DeleteCategoryAsync(string layoutId, int categoryIndex)
        {
            _logger.LogInformation("Deleting category at index {Index} from layout: {LayoutId}", categoryIndex, layoutId);

            var layout = await _layoutsCollection.Find(l => l.Id == layoutId).FirstOrDefaultAsync();
            if (layout == null || categoryIndex >= layout.Categories.Count)
                return false;

            layout.Categories.RemoveAt(categoryIndex);
            var update = Builders<Layout>.Update.Set(l => l.Categories, layout.Categories);

            var result = await _layoutsCollection.UpdateOneAsync(
                l => l.Id == layoutId,
                update
            );

            return result.ModifiedCount > 0;
        }

        public async Task<Layout?> UpdateBannerAsync(string layoutId, CreateBannerDto dto)
        {
            _logger.LogInformation("Updating banner in layout: {LayoutId}", layoutId);

            BannerImage? bannerImage = null;
            if (!string.IsNullOrEmpty(dto.BannerImage))
            {
                try
                {
                    var (publicId, url) = await _cloudinaryService.UploadImageAsync(dto.BannerImage, "banners");
                    bannerImage = new BannerImage { PublicId = publicId, Url = url };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading banner image");
                }
            }

            var banner = new Banner
            {
                Title = dto.Title,
                SubTitle = dto.SubTitle,
                Image = bannerImage
            };

            var update = Builders<Layout>.Update.Set(l => l.Banner, banner);

            return await _layoutsCollection.FindOneAndUpdateAsync(
                l => l.Id == layoutId,
                update,
                new FindOneAndUpdateOptions<Layout> { ReturnDocument = ReturnDocument.After }
            );
        }
    }
}
