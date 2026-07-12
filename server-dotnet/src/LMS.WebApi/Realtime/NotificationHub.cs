using Microsoft.AspNetCore.SignalR;

namespace LMS.WebApi.Realtime;

/// <summary>
/// SignalR hub replacing the original Socket.IO server. Admin dashboards subscribe here
/// and receive <c>newNotification</c> messages when server-side events occur.
/// </summary>
public class NotificationHub : Hub
{
    /// <summary>Allows a client to relay a notification to all connected clients (parity with the Socket.IO handler).</summary>
    public Task Notification(object data) => Clients.All.SendAsync("newNotification", data);
}
