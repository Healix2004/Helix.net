using Helix.Data.Entities;
using Helix.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.PatientDTOs
{
    public class CreatePatientDto
    {
        // --- 1. Identity Link ---
        [Required]
        public string AppUserId { get; set; }
        public EnPatientCategories? PatientCategory { get; set; }

        // --- Step 2: Medical Information ---
        public EnBloodTypes? BloodType { get; set; }
        public string ChronicDiseases { get; set; }
        public string PastSurgeries { get; set; }

        // --- Step 3: Emergency & Insurance ---
        public string EmergencyContactName { get; set; }
        public string EmergencyPhone { get; set; }
        public string EmergencyContactRelation { get; set; }
        public string? InsuranceProvider { get; set; }
        public string? InsurancePolicyNumber { get; set; }
        public List<Allergy> Allergies { get; set; } = new List<Allergy>();

    }
}