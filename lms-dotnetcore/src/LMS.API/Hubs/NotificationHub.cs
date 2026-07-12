using Microsoft.AspNetCore.SignalR;

namespace LMS.API.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("User connected: {UserId}, Connection ID: {ConnectionId}", userId, Context.ConnectionId);
            
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
            }
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("User disconnected: {UserId}, Connection ID: {ConnectionId}", userId, Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotification(string userId, string title, string message)
        {
            _logger.LogInformation("Sending notification to user: {UserId}", userId);
            
            await Clients.Group($"user-{userId}")
                .SendAsync("ReceiveNotification", new { title, message, timestamp = DateTime.UtcNow });
        }

        public async Task NotifyAllUsers(string title, string message)
        {
            _logger.LogInformation("Broadcasting notification to all users");
            
            await Clients.All
                .SendAsync("ReceiveBroadcast", new { title, message, timestamp = DateTime.UtcNow });
        }

        public async Task MarkAsRead(string notificationId)
        {
            _logger.LogInformation("Marking notification as read: {NotificationId}", notificationId);
            
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            await Clients.Group($"user-{userId}")
                .SendAsync("NotificationMarkedAsRead", notificationId);
        }
    }
}
