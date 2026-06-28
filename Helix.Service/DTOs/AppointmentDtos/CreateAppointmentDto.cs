using Helix.Data.Enums;

namespace Helix.Service.DTOs.AppointmentDtos
{
    public class CreateAppointmentDto
    {
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string AppointmentType { get; set; } = string.Empty;
        public EnAppointmentPriority Priority { get; set; } = EnAppointmentPriority.Routine;
        public string? PatientInstruction { get; set; }
    }
}
