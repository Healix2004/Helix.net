using Helix.Data.Enums;
using Helix.Service.DTOs.AllergyDTOs;
using System;
using System.Collections.Generic;

namespace Helix.Service.DTOs.PatientDTOs
{
    public class PatientDto
    {
        public Guid Id { get; set; }

        // --- Flattened properties from AppUser ---
        public string AppUserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string EgyptianNationalId { get; set; }

        // --- Patient specific properties ---
        public EnPatientCategories? PatientCategory { get; set; }
        public EnBloodTypes? BloodType { get; set; }

        // --- Medical Information ---
        public List<ChronicDiseaseDto> ChronicDiseases { get; set; } = new List<ChronicDiseaseDto>();

        // FIXED: Now uses a strongly-typed DTO to match your new FHIR database architecture
        public List<SurgeryDto> Surgeries { get; set; } = new List<SurgeryDto>();

        // --- Emergency & Insurance ---
        public InsuranceDto? Insurance { get; set; }
        public List<EmergencyContactDto> EmergencyContacts { get; set; } = new List<EmergencyContactDto>();

        // --- Allergies ---
        public List<AllergyDto> Allergies { get; set; } = new List<AllergyDto>();
    }
}