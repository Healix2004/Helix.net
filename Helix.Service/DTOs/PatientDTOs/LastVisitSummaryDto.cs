namespace Helix.Service.DTOs.PatientDTOs
{
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