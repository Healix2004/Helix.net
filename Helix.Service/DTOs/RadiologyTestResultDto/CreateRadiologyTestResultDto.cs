using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helix.Service.DTOs.RadiologyTestResultDto
{
    // 1. Used when the Radiologist submits the final written report
    public class CreateRadiologyTestResultDto
    {
        [Required(ErrorMessage = "The Order ID is required.")]
        public Guid OrderId { get; set; }

        [Required(ErrorMessage = "The terminology code (e.g., MRI Brain) is required.")]
        public Guid TerminologyCodeId { get; set; }

        [Required(ErrorMessage = "Findings cannot be empty.")]
        public string Findings { get; set; } = string.Empty;

        [Required(ErrorMessage = "Impression (conclusion) cannot be empty.")]
        public string Impression { get; set; } = string.Empty;
        public List<string> UploadedFilePaths { get; set; } = new List<string>();
    }
}
