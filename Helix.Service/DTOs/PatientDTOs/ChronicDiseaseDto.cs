using Helix.Service.DTOs.MedicationDTOs;

namespace Helix.Service.DTOs.PatientDTOs
{
    public class ChronicDiseaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DiagnosisDate { get; set; }
    }
    public class PatientDashboardDto
    {
        // 1. Patient Basic Data (Top Left Card)
        public Guid PatientId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Initials { get; set; } = string.Empty;
        public string DisplayId { get; set; } = string.Empty; // e.g., "45892-A"
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string InsuranceProvider { get; set; } = string.Empty;
        public string PatientStatus { get; set; } = "Active Patient";

        // 2. Side Cards (Right Column)
        public List<string> KnownAllergies { get; set; } = new();
        public List<string> ChronicConditions { get; set; } = new();
        public List<MedicationDto> CurrentMedications { get; set; } = new();

        // 3. Last Visit Summary (Center Card)
        public LastVisitSummaryDto? LastVisit { get; set; }

        // 4. State Control
        public bool IsMedicalHistoryLocked { get; set; } = true;
    }

    public class LastVisitSummaryDto
    {
        public string AppointmentTitle { get; set; } = string.Empty;
        public string AppointmentDate { get; set; } = string.Empty;

        public string PrescriptionSummary { get; set; } = string.Empty;

        public string LabResultSummary { get; set; } = string.Empty;
        public string LabResultDate { get; set; } = string.Empty;

        public string ClinicalNoteSummary { get; set; } = string.Empty;
        public string ClinicalNoteDate { get; set; } = string.Empty;
    }
}