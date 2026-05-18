using System;

namespace Helix.Service.DTOs.ConsentDTOs
{
    public class ConsentDto
    {
        public Guid Id { get; set; }
        public DateTime ConsentGrantedAt { get; set; }
        public DateTime ConsentExpiresAt { get; set; }        
        // Include foreign keys so the client knows who this consent belongs to
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
    }
}
