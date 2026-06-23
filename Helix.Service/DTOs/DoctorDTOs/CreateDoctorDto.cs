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

        [Required(ErrorMessage = "Medical License Number is required.")]
        [MaxLength(50)]
        public string MedicalLicenseNumber { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public int YearsOfExperience { get; set; }
        public string ClinicAddress { get; set; }
        public string? Bio { get; set; }

        // --- Step 3: Availability & Practice ---
        public EnConsultationType ConsultationType { get; set; } // e.g., "In-Person", "Video", "Both"
        public decimal ConsultationFee { get; set; }

        // This replaces the old Availabilities object
        public List<string> AvailabeDays { get; set; } = new List<string>();
        public List<CreateAvailableTimeSlotDto> AvailableTimeSlots { get; set; } = new List<CreateAvailableTimeSlotDto>();

        // --- Step 4: Verification ---
        // Using IFormFile allows Angular to upload the actual PDF/Images via multipart/form-data
        public IFormFile? MedicalLicenseDocument { get; set; }
        public IFormFile? NationalIdDocument { get; set; }
    }
    public class CreateAvailableTimeSlotDto
    {
        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }
    }
}