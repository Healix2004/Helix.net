using Helix.Data.Enums;

namespace Helix.Service.DTOs.AppointmentDtos
{
    public class AppointmentListDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;

        // Just send the initials for the avatar circles (e.g., "SJ", "MC")
        public string PatientInitials { get; set; } = string.Empty;

        public string AppointmentType { get; set; } = string.Empty;

        // Formatted specifically for your UI (e.g., "09:00 AM")
        public string DisplayTime { get; set; } = string.Empty;

        // --- The Badge Logic ---
        public EnAppointmentStatus Status { get; set; }
        public EnAppointmentPriority Priority { get; set; }

        // This determines what text shows up in the UI badge
        public string BadgeText => Priority == EnAppointmentPriority.Urgent
            ? "Urgent"
            : Status switch
            {
                EnAppointmentStatus.Booked => "Upcoming",
                EnAppointmentStatus.Arrived => "Waiting",
                EnAppointmentStatus.Fulfilled => "Completed",
                _ => Status.ToString()
            };
    }
}
