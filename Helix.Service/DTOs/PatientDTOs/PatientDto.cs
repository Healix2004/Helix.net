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
        public string FullName { get; set; } = default!;
        public string NationalId { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string AppUserId { get; set; }
        public string Email { get; set; }
        // --- Patient specific properties ---
        public string? PatientCategory { get; set; }
        public string? BloodType { get; set; }

        // --- Medical Information ---
        public List<string> ChronicDiseases { get; set; } = new List<string>();
        public List<string> Medication { get; set; } = new List<string>();

        // FIXED: Now uses a strongly-typed DTO to match your new FHIR database architecture
        public List<string> Surgeries { get; set; } = new List<string>();

        // --- Emergency & Insurance ---
        public InsuranceDto? Insurance { get; set; }
        public List<EmergencyContactDto> EmergencyContacts { get; set; } = new List<EmergencyContactDto>();

        // --- Allergies ---
        public List<string> Allergies { get; set; } = new List<string>();
    }
}