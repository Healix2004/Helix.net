namespace Helix.Service.DTOs.PrescriptionDtos
{
    public class PatientClinicalSummaryDto
    {
        // --- Header Info ---
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string Initials { get; set; } = string.Empty; // e.g., "SJ"
        public string DisplayId { get; set; } = string.Empty; // e.g., "45892-A"
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;

        // --- Sidebar Sections ---
        // Allergies and Chronic Conditions are lists of strings for the UI bullet points
        public List<string> Allergies { get; set; } = new List<string>();
        public List<string> ChronicConditions { get; set; } = new List<string>();

        // The list of cards for the "Current Medications" section
        public List<ActiveMedicationDto> CurrentMedications { get; set; } = new List<ActiveMedicationDto>();
    }
}
