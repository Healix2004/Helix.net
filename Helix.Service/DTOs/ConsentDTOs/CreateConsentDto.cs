using System;

namespace Helix.Service.DTOs.ConsentDTOs
{
    public class CreateConsentDto
    {
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime ConsentGrantedAt { get; set; } = DateTime.Now;
        public DateTime ConsentExpiresAt { get; set; }
        public bool IsEmergencyOverride { get; set; }
    }
}
