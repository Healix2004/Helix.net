using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.DoctorDTOs
{
    public class CreateAvailableTimeSlotDto
    {
        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }
    }
    public class CalendarMonthDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<CalendarDayDto> Days { get; set; } = new();
    }

    public class CalendarDayDto
    {
        public int Day { get; set; } // 1, 2, 3...
        public DateTime Date { get; set; } // The full date
        public bool HasAppointments { get; set; } // Triggers the blue dot on the UI
    }
}