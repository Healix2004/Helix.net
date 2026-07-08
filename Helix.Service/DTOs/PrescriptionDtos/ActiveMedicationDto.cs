using Helix.Service.DTOs.DrugDTOs;
using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.PrescriptionDtos
{
    public class ActiveMedicationDto
    {
        // Use the ItemId so the frontend can reference a specific drug if the doctor needs to modify/stop it
        public Guid PrescriptionItemId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        // This is a calculated string (e.g., "10mg - Once daily")
        public string Instructions { get; set; } = string.Empty;
        public DateTime PrescribedDate { get; set; }
        // Displays the status (e.g., "Active", "OnHold")
        public string Status { get; set; } = string.Empty;
    }
    public class PrescriptionPayloadDto
    {
        [Required]
        public Guid AppointmentId { get; set; }
        public bool DDIEnabled { get; set; } = false;
        public List<MedicationOrderItem> Medications { get; set; } = new();
        public List<string> LabOrderCodes { get; set; } = new();
        public List<string> RadiologyOrderCodes { get; set; } = new();
    }

    public class MedicationOrderItem
    {
        [Required]
        public string Rxcui { get; set; }

        [Required]
        public string DrugName { get; set; }

        [Required]
        public string Dosage { get; set; }

        [Required]
        public string Frequency { get; set; }

        [Required]
        public string Duration { get; set; }
    }

    public class CreatePrescriptionResultDto
    {
        public bool IsSuccess { get; set; }
        public Guid? PrescriptionId { get; set; } // Only populated if successful
        public List<InteractionResponseDTO> Interactions { get; set; } = new(); // Only populated if DDI fails
    }
}
