using LMS.Application.DTOs.Analytics;

namespace LMS.Application.Services.Abstractions;

public interface IAnalyticsService
{
    Task<AnalyticsData> GetUsersAnalyticsAsync(CancellationToken ct = default);
    Task<AnalyticsData> GetCoursesAnalyticsAsync(CancellationToken ct = default);
    Task<AnalyticsData> GetOrdersAnalyticsAsync(CancellationToken ct = default);
}
