using LMS.API.Models.Domain;
using LMS.API.Models.DTOs.Courses;
using LMS.API.Services.Interfaces;
using LMS.API.Exceptions;
using MongoDB.Driver;

namespace LMS.API.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly IMongoCollection<Course> _coursesCollection;
        private readonly ICacheService _cacheService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<CourseService> _logger;

        public CourseService(
            IMongoCollection<Course> coursesCollection,
            ICacheService cacheService,
            ICloudinaryService cloudinaryService,
            ILogger<CourseService> logger)
        {
            _coursesCollection = coursesCollection;
            _cacheService = cacheService;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        public async Task<Course> CreateCourseAsync(CreateCourseDto dto)
        {
            _logger.LogInformation("Creating new course: {CourseName}", dto.Name);

            var course = new Course
            {
                Name = dto.Name,
                Description = dto.Description,
                Categories = dto.Categories,
                Price = dto.Price,
                EstimatedPrice = dto.EstimatedPrice,
                Tags = dto.Tags,
                Level = dto.Level,
                DemoUrl = dto.DemoUrl,
                Benefits = dto.Benefits?.Select(b => new Benefit { Title = b.Title }).ToList() ?? new(),
                Prerequisites = dto.Prerequisites?.Select(p => new Prerequisite { Title = p.Title }).ToList() ?? new()
            };

            await _coursesCollection.InsertOneAsync(course);
            return course;
        }

        public async Task<Course?> GetCourseByIdAsync(string courseId)
        {
            _logger.LogInformation("Fetching course: {CourseId}", courseId);

            // Try cache first
            var cached = await _cacheService.GetAsync<Course>($"course_{courseId}");
            if (cached != null)
                return cached;

            var course = await _coursesCollection.Find(c => c.Id == courseId).FirstOrDefaultAsync();

            if (course != null)
                await _cacheService.SetAsync($"course_{courseId}", course, TimeSpan.FromDays(7));

            return course;
        }

        public async Task<List<Course>> GetAllCoursesAsync(int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("Fetching all courses - Page: {Page}, Size: {PageSize}", page, pageSize);

            var skip = (page - 1) * pageSize;
            return await _coursesCollection.Find(_ => true)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<Course?> UpdateCourseAsync(string courseId, UpdateCourseDto dto)
        {
            _logger.LogInformation("Updating course: {CourseId}", courseId);

            var updateBuilder = Builders<Course>.Update;
            var updates = new List<UpdateDefinition<Course>>();

            if (!string.IsNullOrEmpty(dto.Name))
                updates.Add(updateBuilder.Set(c => c.Name, dto.Name));
            if (!string.IsNullOrEmpty(dto.Description))
                updates.Add(updateBuilder.Set(c => c.Description, dto.Description));
            if (!string.IsNullOrEmpty(dto.Categories))
                updates.Add(updateBuilder.Set(c => c.Categories, dto.Categories));
            if (dto.Price.HasValue)
                updates.Add(updateBuilder.Set(c => c.Price, dto.Price.Value));
            if (!string.IsNullOrEmpty(dto.Tags))
                updates.Add(updateBuilder.Set(c => c.Tags, dto.Tags));
            if (dto.Benefits != null)
                updates.Add(updateBuilder.Set(c => c.Benefits, dto.Benefits.Select(b => new Benefit { Title = b.Title }).ToList()));

            updates.Add(updateBuilder.Set(c => c.UpdatedAt, DateTime.UtcNow));

            var update = updateBuilder.Combine(updates);
            var course = await _coursesCollection.FindOneAndUpdateAsync(
                c => c.Id == courseId,
                update,
                new FindOneAndUpdateOptions<Course> { ReturnDocument = ReturnDocument.After }
            );

            if (course != null)
                await _cacheService.SetAsync($"course_{courseId}", course, TimeSpan.FromDays(7));

            return course;
        }

        public async Task<bool> DeleteCourseAsync(string courseId)
        {
            _logger.LogInformation("Deleting course: {CourseId}", courseId);

            var result = await _coursesCollection.DeleteOneAsync(c => c.Id == courseId);
            await _cacheService.DeleteAsync($"course_{courseId}");

            return result.DeletedCount > 0;
        }

        public async Task<Course?> AddQuestionAsync(string courseId, string contentId, string question, string userId)
        {
            _logger.LogInformation("Adding question to course: {CourseId}", courseId);

            var filter = Builders<Course>.Filter.Eq(c => c.Id, courseId);
            var update = Builders<Course>.Update.Push(
                c => c.CourseData.FirstOrDefault(cd => cd.Title == contentId)!.Questions,
                new Comment { Question = question, User = new UserReference { Id = userId } }
            );

            var course = await _coursesCollection.FindOneAndUpdateAsync(
                filter,
                update,
                new FindOneAndUpdateOptions<Course> { ReturnDocument = ReturnDocument.After }
            );

            if (course != null)
                await _cacheService.SetAsync($"course_{courseId}", course, TimeSpan.FromDays(7));

            return course;
        }

        public async Task<Course?> AddAnswerAsync(string courseId, string contentId, string questionId, string answer, string userId)
        {
            _logger.LogInformation("Adding answer to question in course: {CourseId}", courseId);
            return await GetCourseByIdAsync(courseId);
        }

        public async Task<Course?> AddReviewAsync(string courseId, AddReviewDto dto, string userId)
        {
            _logger.LogInformation("Adding review to course: {CourseId}", courseId);

            var review = new Review
            {
                Rating = dto.Rating,
                Comment = dto.Comment,
                User = new UserReference { Id = userId }
            };

            var update = Builders<Course>.Update.Push(c => c.Reviews, review);
            var course = await _coursesCollection.FindOneAndUpdateAsync(
                c => c.Id == courseId,
                update,
                new FindOneAndUpdateOptions<Course> { ReturnDocument = ReturnDocument.After }
            );

            if (course != null)
                await _cacheService.SetAsync($"course_{courseId}", course, TimeSpan.FromDays(7));

            return course;
        }

        public async Task<List<Course>> GetAdminCoursesAsync(string adminId)
        {
            _logger.LogInformation("Fetching admin courses for admin: {AdminId}", adminId);
            return await _coursesCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Course?> GetCourseContentAsync(string courseId, string userId)
        {
            _logger.LogInformation("Fetching course content: {CourseId} for user: {UserId}", courseId, userId);
            return await GetCourseByIdAsync(courseId);
        }
    }
}
