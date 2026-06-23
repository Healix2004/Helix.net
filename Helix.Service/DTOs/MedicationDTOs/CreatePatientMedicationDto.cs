using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.MedicationDTOs
{
    public class CreatePatientMedicationDto
    {
        [Required(ErrorMessage = "Medication code (RXCUI) is required")]
        [MaxLength(50)]
        public string medicationCatalogRxcui { get; set; }
    }
}
