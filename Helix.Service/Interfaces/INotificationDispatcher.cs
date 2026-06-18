using Helix.Service.DTOs.NotificationDtos;

namespace Helix.Service.Interfaces
{
    public interface INotificationDispatcher
    {
        Task SendNotificationToUserAsync(string userId, NotificationDto notification);
        Task SendUnreadCountToUserAsync(string userId, int unreadCount);
    }
}
