namespace Helix.Service.DTOs.AppointmentDtos
{
    // Maps directly to the "Daily Schedule Summary" card in your UI
    public class DailyScheduleSummaryDto
    {
        public int TotalAppointments { get; set; }
        public int Confirmed { get; set; } // Status = Booked
        public int Waiting { get; set; }   // Status = Arrived
        public int Urgent { get; set; }    // Priority = Urgent/Stat
    }
}
