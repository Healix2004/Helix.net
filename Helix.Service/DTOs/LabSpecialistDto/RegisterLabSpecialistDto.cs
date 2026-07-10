using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helix.Service.DTOs.LabSpecialistDto
{
    public class RegisterLabSpecialistDto
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "National ID is required.")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "National ID must be exactly 14 characters.")]
        public string NationalId { get; set; }

        [Required(ErrorMessage = "Lab Name is required.")]
        [MaxLength(150)]
        public string LabName { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "License Number is required.")]
        public string LicenseNumber { get; set; }

        // --- Files ---
        public IFormFile? ProfileImageFile { get; set; }

        [Required(ErrorMessage = "The Lab Specialist License document is required.")]
        public IFormFile LabSpecialistLicenseFile { get; set; }

        [Required(ErrorMessage = "The National ID document is required.")]
        public IFormFile NationalIdFile { get; set; }
    }}
