namespace Helix.Service.DTOs.AppointmentDtos
{
    // Used for the Timeslot View (e.g., Wednesday, June 24, 2026)
    public class DayTimeSlotsDto
    {
        public DateTime Date { get; set; }
        public List<TimeSlotDto> Slots { get; set; } = new();
    }
}
