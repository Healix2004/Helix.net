namespace Helix.Service.DTOs.AppointmentDtos
{
    // Used for the weekly summary
    public class DayAvailabilityDto
    {
        public DateTime Date { get; set; }
        public bool HasAvailableSlots { get; set; }
    }
}
