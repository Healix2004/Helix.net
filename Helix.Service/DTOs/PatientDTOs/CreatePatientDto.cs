using Helix.Data.Enums;
using Helix.Service.DTOs.AllergyDTOs;
using Helix.Service.DTOs.MedicationDTOs;
using Hl7.Fhir.ElementModel.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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
        public string FullName { get; set; } = default!;
        public string NationalId { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public IFormFile? ProfileImage { get; set; }
        public EnPatientCategories? PatientCategory { get; set; }

        // --- Step 2: Medical Information ---
        public EnBloodTypes BloodType { get; set; }

        public List<string> ChronicDiseasesCode { get; set; } = new List<string>();
        public List<string> SurgeriesCode { get; set; } = new List<string>();

        // --- Step 3: Emergency & Insurance ---
        public string? InsuranceProvider { get; set; }
        public string? InsurancePolicyNumber { get; set; }
        [Required(ErrorMessage = "Emergency contact name is required")]
        public string EmergencyContactName { get; set; }

        [Required(ErrorMessage = "Emergency contact phone number is required")]
        public string EmergencyContactPhoneNumber { get; set; }    

        // --- Step 4: Allergies & Medications  ---
        public List<string> AllergiesCode { get; set; } = new List<string>();
        public List<string> CurrentMedicationsCode { get; set; } = new List<string>();
    }
}