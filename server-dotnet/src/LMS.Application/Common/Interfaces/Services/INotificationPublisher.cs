namespace LMS.Application.Common.Interfaces.Services;

/// <summary>Pushes realtime notifications to connected admin clients (SignalR / Socket.IO equivalent).</summary>
public interface INotificationPublisher
{
    Task PublishAsync(object payload, CancellationToken ct = default);
}
