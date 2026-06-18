using Helix.API.Hubs;
using Helix.Service.DTOs.NotificationDtos;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Helix.API.Adapters
{
    public class SignalRNotificationDispatcher(IHubContext<NotificationHub> hubContext) : INotificationDispatcher
    {
        public async Task SendNotificationToUserAsync(string userId, NotificationDto notification)
        {
            // Pushes the full notification object to the specific user's Angular frontend
            await hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notification);
        }
        public async Task SendUnreadCountToUserAsync(string userId, int unreadCount)
        {
            // Pushes just the integer count so the Angular UI can update the little red bell icon
            await hubContext.Clients.User(userId).SendAsync("UpdateUnreadCount", unreadCount);
        }
    }
}
