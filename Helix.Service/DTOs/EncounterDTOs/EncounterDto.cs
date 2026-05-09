using System;

namespace Helix.Service.DTOs.EncounterDTOs
{
    public class EncounterDto
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string status { get; set; }
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
    }
}
