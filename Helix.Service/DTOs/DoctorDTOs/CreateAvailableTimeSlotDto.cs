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
}