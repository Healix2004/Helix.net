using Helix.Data.Enums;
using System;

namespace Helix.Service.DTOs.PatientDTOs
{
    public class PatientDto
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

        // Patient specific properties
        public EnPatientCategories PatientCategory { get; set; }
        public EnBloodTypes? BloodType { get; set; }
    }
}
