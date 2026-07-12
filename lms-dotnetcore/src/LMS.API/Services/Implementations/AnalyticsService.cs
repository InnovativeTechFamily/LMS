using LMS.API.Services.Interfaces;
using MongoDB.Driver;
using LMS.API.Models.Domain;

namespace LMS.API.Services.Implementations
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<Order> _ordersCollection;
        private readonly IMongoCollection<Notification> _notificationsCollection;
        private readonly IMongoCollection<Course> _coursesCollection;
        private readonly ILogger<AnalyticsService> _logger;

        public AnalyticsService(
            IMongoCollection<User> usersCollection,
            IMongoCollection<Order> ordersCollection,
            IMongoCollection<Notification> notificationsCollection,
            IMongoCollection<Course> coursesCollection,
            ILogger<AnalyticsService> logger)
        {
            _usersCollection = usersCollection;
            _ordersCollection = ordersCollection;
            _notificationsCollection = notificationsCollection;
            _coursesCollection = coursesCollection;
            _logger = logger;
        }

        public async Task<Dictionary<string, int>> GetUserAnalyticsAsync(int days = 28)
        {
            _logger.LogInformation("Fetching user analytics for last {Days} days", days);

            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            var usersByDate = new Dictionary<string, int>();

            var users = await _usersCollection.Find(u => u.CreatedAt >= cutoffDate)
                .ToListAsync();

            foreach (var user in users)
            {
                var dateKey = user.CreatedAt.ToString("yyyy-MM-dd");
                if (usersByDate.ContainsKey(dateKey))
                    usersByDate[dateKey]++;
                else
                    usersByDate[dateKey] = 1;
            }

            return usersByDate;
        }

        public async Task<Dictionary<string, int>> GetOrderAnalyticsAsync(int days = 28)
        {
            _logger.LogInformation("Fetching order analytics for last {Days} days", days);

            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            var ordersByDate = new Dictionary<string, int>();

            var orders = await _ordersCollection.Find(o => o.CreatedAt >= cutoffDate)
                .ToListAsync();

            foreach (var order in orders)
            {
                var dateKey = order.CreatedAt.ToString("yyyy-MM-dd");
                if (ordersByDate.ContainsKey(dateKey))
                    ordersByDate[dateKey]++;
                else
                    ordersByDate[dateKey] = 1;
            }

            return ordersByDate;
        }

        public async Task<Dictionary<string, int>> GetNotificationAnalyticsAsync(int days = 365)
        {
            _logger.LogInformation("Fetching notification analytics for last {Days} days", days);

            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            var notificationsByDate = new Dictionary<string, int>();

            var notifications = await _notificationsCollection.Find(n => n.CreatedAt >= cutoffDate)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                var dateKey = notification.CreatedAt.ToString("yyyy-MM-dd");
                if (notificationsByDate.ContainsKey(dateKey))
                    notificationsByDate[dateKey]++;
                else
                    notificationsByDate[dateKey] = 1;
            }

            return notificationsByDate;
        }

        public async Task<int> GetTotalUsersAsync()
        {
            _logger.LogInformation("Getting total user count");
            var count = await _usersCollection.CountDocumentsAsync(_ => true);
            return (int)count;
        }

        public async Task<int> GetTotalCoursesAsync()
        {
            _logger.LogInformation("Getting total course count");
            var count = await _coursesCollection.CountDocumentsAsync(_ => true);
            return (int)count;
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            _logger.LogInformation("Calculating total revenue");

            var orders = await _ordersCollection.Find(_ => true).ToListAsync();
            var courses = await _coursesCollection.Find(_ => true).ToListAsync();

            decimal totalRevenue = 0;
            foreach (var order in orders)
            {
                var course = courses.FirstOrDefault(c => c.Id == order.CourseId);
                if (course != null)
                    totalRevenue += course.Price;
            }

            return totalRevenue;
        }
    }
}
