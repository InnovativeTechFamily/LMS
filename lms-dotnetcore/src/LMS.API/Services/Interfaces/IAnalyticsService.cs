namespace LMS.API.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task<Dictionary<string, int>> GetUserAnalyticsAsync(int days = 28);
        Task<Dictionary<string, int>> GetOrderAnalyticsAsync(int days = 28);
        Task<Dictionary<string, int>> GetNotificationAnalyticsAsync(int days = 365);
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalCoursesAsync();
        Task<decimal> GetTotalRevenueAsync();
    }
}
