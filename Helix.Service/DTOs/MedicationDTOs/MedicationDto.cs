using System;
using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.MedicationDTOs
{
    public class MedicationDto
    {
        // Links to your MedicationCatalog table (the RxNorm ID from the dropdown)
        [Required(ErrorMessage = "Medication code (RXCUI) is required")]
        [MaxLength(50)]
        public string Rxcui { get; set; }
        public string Name { get; set; }
        public Guid PatientId { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
