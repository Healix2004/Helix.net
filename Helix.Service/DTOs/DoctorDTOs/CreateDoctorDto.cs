using Helix.Data.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.DoctorDTOs
{
    public class CreateDoctorDto
    {
        // --- 1. Identity Link ---
        [Required]
        public string AppUserId { get; set; }

        // --- Step 2: Professional Information ---
        [Required(ErrorMessage = "Specialty is required.")]
        [MaxLength(100)]
        public string Specialty { get; set; }

        [Required(ErrorMessage = "Syndicate Number / Medical License is required.")]
        [MaxLength(50)]
        public string SyndicateNumber { get; set; }

        public string Country { get; set; }
        public string State { get; set; }
        public int YearsOfExperience { get; set; }
        public string ClinicAddress { get; set; }
        public string Bio { get; set; }

        // --- Step 3: Availability & Practice ---
        public EnConsultationType ConsultationType { get; set; } // e.g., "In-Person", "Video", "Both"
        public decimal ConsultationFee { get; set; }

        // This accepts the list of days and times the doctor selected
        public List<CreateDoctorAvailabilityDto> Availabilities { get; set; } = new List<CreateDoctorAvailabilityDto>();

        // --- Step 4: Verification ---
        // Using IFormFile allows Angular to upload the actual PDF/Images via multipart/form-data
        public IFormFile? MedicalLicenseDocument { get; set; }
        public IFormFile? NationalIdDocument { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}