using Helix.Data.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.AuthDTOs
{
    public class RegisterDto
    {
        // --- Account Information ---

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [EmailAddress(ErrorMessage = "Invalid alternative email address")]
        public string? AlternativeEmailAddress { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        // --- Personal Information ---

        // Added IFormFile to handle the profile picture upload from the UI
        public IFormFile? ProfileImage { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "National ID is required")]
        [StringLength(20, ErrorMessage = "National ID cannot exceed 20 characters")]
        public string NationalId { get; set; } // Added based on Step 1 UI

        // --- Contact & Location ---

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; } // For the IdentityUser.PhoneNumber

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string? Address { get; set; }
    }
}