namespace Helix.Data.Entities
{
    public class Notification : BaseEntity
    {
        public string AppUserId { get; set; } // The user receiving the alert
        public AppUser AppUser { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
