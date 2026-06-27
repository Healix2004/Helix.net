using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.RadiologyTestResultDto
{
    public class CreateRadiologyTestResultDto
    {
        [Required(ErrorMessage = "The Order ID is required.")]
        public Guid OrderId { get; set; }

        // Added to track the external specialist writing the report
        public string? ExternalRadiologistName { get; set; }

        [Required(ErrorMessage = "Findings cannot be empty.")]
        public string Findings { get; set; } = string.Empty;

        // Renamed to match the RadiologyReport entity precisely
        [Required(ErrorMessage = "Impression cannot be empty.")]
        public string Impression { get; set; } = string.Empty;

        // IFormFile handles the raw binary upload from the Flutter or Angular frontend
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}