using Helix.Data.Enums;
using Helix.Service.DTOs.AllergyDTOs;
using Helix.Service.DTOs.MedicationDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.PatientDTOs
{
    public class CreatePatientDto
    {
        // --- 1. Identity Link ---
        [Required(ErrorMessage = "AppUserId is required")]
        public string AppUserId { get; set; }
        public EnPatientCategories? PatientCategory { get; set; }

        // --- Step 2: Medical Information ---
        public EnBloodTypes BloodType { get; set; }

        public List<CreatePatientChronicDiseaseDto> ChronicDiseases { get; set; } = new List<CreatePatientChronicDiseaseDto>();
        public List<CreateSurgeryDto> Surgeries { get; set; } = new List<CreateSurgeryDto>();

        // --- Step 3: Emergency & Insurance ---
        public CreateInsuranceDto? Insurance { get; set; }
        public List<CreateEmergencyContactDto>? EmergencyContacts { get; set; } = new List<CreateEmergencyContactDto>();
        // --- Step 4: Allergies & Medications  ---
        public List<CreatePatientAllergyDto> Allergies { get; set; } = new List<CreatePatientAllergyDto>();
        public List<CreatePatientMedicationDto> CurrentMedications { get; set; } = new List<CreatePatientMedicationDto>();
    }
}