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
}
