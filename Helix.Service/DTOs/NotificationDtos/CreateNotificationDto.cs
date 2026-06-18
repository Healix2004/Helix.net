namespace Helix.Service.DTOs.NotificationDtos
{
    public class CreateNotificationDto
    {
        public string AppUserId { get; set; } // The user receiving the alert
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
    }
}
