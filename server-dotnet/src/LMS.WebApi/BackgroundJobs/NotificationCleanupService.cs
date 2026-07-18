using LMS.Application.Services.Abstractions;

namespace LMS.WebApi.BackgroundJobs;

/// <summary>
/// Daily job that purges read notifications older than 30 days, replacing the
/// original <c>node-cron</c> schedule ("0 0 0 * * *").
/// </summary>
public class NotificationCleanupService : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromDays(1);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationCleanupService> _logger;

    public NotificationCleanupService(IServiceScopeFactory scopeFactory, ILogger<NotificationCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);
        do
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var notifications = scope.ServiceProvider.GetRequiredService<INotificationService>();
                await notifications.PurgeReadOlderThan30DaysAsync(stoppingToken);
                _logger.LogInformation("Deleted read notifications older than 30 days");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Notification cleanup failed");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
