namespace Helix.Service.DTOs.AppointmentDtos
{
    public class TimeSlotDto
    {
        // For the UI display (e.g., "08:30 AM")
        public string DisplayTime { get; set; } = string.Empty;

        // The actual DateTime the frontend will send back when booking
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Tells the frontend whether to "cross out" the button
        public bool IsAvailable { get; set; }
    }
}
