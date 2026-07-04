using Helix.Data.Enums;

namespace Helix.Service.DTOs.AppointmentDtos
{
    public class DailyScheduleSummaryDto
    {
        public int TotalAppointments { get; set; }
        public int Confirmed { get; set; }
        public int Waiting { get; set; }
        public int Urgent { get; set; }
    }
}
