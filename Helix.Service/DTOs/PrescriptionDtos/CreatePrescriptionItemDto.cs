using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.PrescriptionDtos
{
    public class CreatePrescriptionItemDto
    {
        [Required]
        public string TerminologyRxcui { get; set; } = string.Empty; // e.g. "315442" from the Catalog

        [Required]
        public string Dosage { get; set; } = string.Empty;

        [Required]
        public string Frequency { get; set; } = string.Empty;

        [Required]
        public string Duration { get; set; } = string.Empty;
    }
}
