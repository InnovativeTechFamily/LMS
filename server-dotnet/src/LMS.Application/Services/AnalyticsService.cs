using LMS.Application.Common.Interfaces.Persistence;
using LMS.Application.DTOs.Analytics;
using LMS.Application.Services.Abstractions;

namespace LMS.Application.Services;

/// <summary>
/// Reproduces the Node <c>generateLast12MothsData</c> helper: 12 rolling 28-day windows,
/// counting documents created in each.
/// </summary>
public class AnalyticsService : IAnalyticsService
{
    private readonly IUserRepository _users;
    private readonly ICourseRepository _courses;
    private readonly IOrderRepository _orders;

    public AnalyticsService(IUserRepository users, ICourseRepository courses, IOrderRepository orders)
    {
        _users = users;
        _courses = courses;
        _orders = orders;
    }

    public Task<AnalyticsData> GetUsersAnalyticsAsync(CancellationToken ct = default)
        => BuildAsync(_users.CountCreatedBetweenAsync, ct);

    public Task<AnalyticsData> GetCoursesAnalyticsAsync(CancellationToken ct = default)
        => BuildAsync(_courses.CountCreatedBetweenAsync, ct);

    public Task<AnalyticsData> GetOrdersAnalyticsAsync(CancellationToken ct = default)
        => BuildAsync(_orders.CountCreatedBetweenAsync, ct);

    private static async Task<AnalyticsData> BuildAsync(
        Func<DateTime, DateTime, CancellationToken, Task<long>> counter,
        CancellationToken ct)
    {
        var months = new List<MonthData>();
        var current = DateTime.UtcNow.Date.AddDays(1);

        for (var i = 11; i >= 0; i--)
        {
            var endDate = current.AddDays(-i * 28);
            var startDate = endDate.AddDays(-28);
            var label = endDate.ToString("d MMM yyyy");
            var count = await counter(startDate, endDate, ct);
            months.Add(new MonthData(label, count));
        }

        return new AnalyticsData(months);
    }
}
