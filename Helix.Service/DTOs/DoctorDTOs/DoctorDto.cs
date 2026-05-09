using System;

namespace Helix.Service.DTOs.DoctorDTOs
{
    public class DoctorDto
    {
        public Guid Id { get; set; }
        
        // Flattened properties from AppUser
        public string AppUserId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public DateTime DataOfBrith { get; set; }

        // Doctor specific properties
        public string Specialty { get; set; }
        public decimal ConsultationFee { get; set; }
        public string Bio { get; set; }
        public string SyndicateNumber { get; set; }
    }
}
