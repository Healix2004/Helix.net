namespace Helix.Service.DTOs.AppointmentDtos
{
    // The main wrapper returned to the frontend
    public class DoctorAvailabilityDto
    {
        // 1. The summary for the calendar UI (e.g., to draw dots under available days)
        public List<DayAvailabilityDto> WeeklyAvailability { get; set; } = new List<DayAvailabilityDto>();

        // 2. The exact clickable time buttons for the specifically requested date
        public List<TimeSlotDto> SelectedDaySlots { get; set; } = new List<TimeSlotDto>();
    }

}
