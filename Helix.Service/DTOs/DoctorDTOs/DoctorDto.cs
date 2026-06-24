using System;

namespace Helix.Service.DTOs.DoctorDTOs
{
    public class DoctorDto
    {
        public Guid Id { get; set; }
        
        // Flattened properties from AppUser
        public string AppUserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string NationalId { get; set; } = default!;
        // Doctor specific properties
        public string Specialty { get; set; }
        public decimal ConsultationFee { get; set; }
        public string Bio { get; set; }
        public string MedicalLicenseNumber { get; set; }
    }
}
