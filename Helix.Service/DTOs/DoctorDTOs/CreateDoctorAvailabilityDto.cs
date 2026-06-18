using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.DoctorDTOs
{
    public class CreateDoctorAvailabilityDto
    {
        [Required]
        public DayOfWeek Day { get; set; } // 0 = Sunday, 1 = Monday, etc.

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }
    }
}