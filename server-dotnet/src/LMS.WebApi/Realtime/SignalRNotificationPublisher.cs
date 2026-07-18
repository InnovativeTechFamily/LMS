using LMS.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace LMS.WebApi.Realtime;

/// <summary>Publishes realtime notifications to connected clients via SignalR.</summary>
public class SignalRNotificationPublisher : INotificationPublisher
{
    private readonly IHubContext<NotificationHub> _hub;

    public SignalRNotificationPublisher(IHubContext<NotificationHub> hub) => _hub = hub;

    public Task PublishAsync(object payload, CancellationToken ct = default)
        => _hub.Clients.All.SendAsync("newNotification", payload, ct);
}
