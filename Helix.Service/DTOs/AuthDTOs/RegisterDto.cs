using Helix.Data.Enums;
using Hl7.Fhir.Utility;
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

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [Required(ErrorMessage ="Register As is Required")]
        public EnRoles RegisterAs { get; set; }
    }
}