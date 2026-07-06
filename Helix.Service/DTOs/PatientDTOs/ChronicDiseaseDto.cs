namespace Helix.Service.DTOs.PatientDTOs
{
    public class ChronicDiseaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DiagnosisDate { get; set; }
    }
    public class PatientPortalDashboardDto
    {
        // Top Cards
        public int TotalRecords { get; set; }
        public int ActivePrescriptions { get; set; }
        public int UpcomingAppointments { get; set; }

        // Health Score Widget
        public int HealthScore { get; set; }
        public string HealthScoreMessage { get; set; }

        // Lists
        public List<RecentActivityItemDto> RecentActivity { get; set; } = new();
        public List<HealthTipDto> HealthTips { get; set; } = new();
    }

    public class RecentActivityItemDto
    {
        public string ActivityType { get; set; } // e.g., "Lab", "Prescription", "Appointment"
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string TimeAgo { get; set; } // e.g., "2 hours ago"
    }

    public class HealthTipDto
    {
        public string IconType { get; set; } // e.g., "Heart", "Activity", "Drop"
        public string Title { get; set; }
        public string Description { get; set; }
    }


    public class PatientLabDashboardDto
    {
        public int TotalTests { get; set; }
        public int PendingResults { get; set; }
        public int AbnormalResults { get; set; }

        // Nullable. If null, the Angular frontend simply hides the Alert widget.
        public LabAlertDto? ActiveAlert { get; set; }

        public List<LabTestItemDto> LabTests { get; set; } = new();
        public List<LabRecentActivityDto> RecentActivity { get; set; } = new();
    }

    public class LabAlertDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string TimeAgo { get; set; }
    }

    public class LabTestItemDto
    {
        public Guid Id { get; set; }
        public string TestName { get; set; }
        public DateTime Date { get; set; }
        public string RequestedBy { get; set; }

        // Maps to the UI pill colors: "Pending", "Completed", or "Abnormal"
        public string Status { get; set; }
    }

    public class LabRecentActivityDto
    {
        public string Title { get; set; }
        public string TimeAgo { get; set; }

        // Used by Angular to render the colored dot next to the activity
        public string StatusColor { get; set; }
    }


    public class LabTestDetailsDto
    {
        public Guid OrderId { get; set; }
        public string PanelName { get; set; } // e.g., "Complete Blood Count & Metabolic Panel"

        public LabPatientInfoDto PatientInfo { get; set; }
        public LabDoctorCommentDto DoctorComment { get; set; }
        public List<LabParameterResultDto> Results { get; set; } = new();
    }

    public class LabPatientInfoDto
    {
        public string PatientName { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string DateOfBirth { get; set; } // e.g., "March 15, 1985 (41 years)"
        public string TestDate { get; set; }
        public string PatientIdDisplay { get; set; } // e.g., "PT-2026-4782"
    }

    public class LabDoctorCommentDto
    {
        public string DoctorName { get; set; }
        public string Specialty { get; set; }
        public string CommentDate { get; set; }
        public string OverallComment { get; set; }
        public List<string> Recommendations { get; set; } = new();
    }

    public class LabParameterResultDto
    {
        public string ParameterName { get; set; }
        public string ResultValue { get; set; }
        public string NormalRange { get; set; }
        public string Unit { get; set; }
        public string Status { get; set; }
    }


    public class RadiologyDashboardDto
    {
        // Top Metric Cards
        public int TotalScans { get; set; }
        public int PendingReview { get; set; }
        public int RecentUploads { get; set; }
        public int AbnormalFindings { get; set; }

        // Main Table
        public List<RadiologyScanItemDto> Scans { get; set; } = new();

        // Right Sidebar Widgets
        public LatestScanPreviewDto? LatestScanPreview { get; set; }
        public LatestRadiologistNoteDto? LatestNote { get; set; }
    }

    public class RadiologyScanItemDto
    {
        public Guid Id { get; set; }
        public string ScanType { get; set; } // e.g., "MRI Brain"
        public string Category { get; set; } // e.g., "MRI", "CT", "Ultrasound" (For frontend tab filtering)
        public DateTime Date { get; set; }
        public string RequestedBy { get; set; }
        public string Status { get; set; } // "Pending", "Completed", "Abnormal"
    }

    public class LatestScanPreviewDto
    {
        public Guid ScanId { get; set; }
        public string ScanName { get; set; }
        public string PatientName { get; set; }
    }

    public class LatestRadiologistNoteDto
    {
        public string DoctorName { get; set; }
        public string TimeAgo { get; set; }
        public string NotePreview { get; set; }
    }


    public class RadiologyStudyDetailsDto
    {
        public Guid OrderId { get; set; }
        public List<string> ImageUrls { get; set; } = new(); // The URLs to the DICOM or JPG images

        public RadiologyMetadataDto Metadata { get; set; }
        public RadiologistFindingsDto Findings { get; set; }
        public PhysicianNotesDto PhysicianNotes { get; set; }
    }

    public class RadiologyMetadataDto
    {
        public string PatientName { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string PatientIdDisplay { get; set; }

        public string StudyDate { get; set; }
        public string StudyTime { get; set; }
        public string ReferringPhysician { get; set; }
        public string Modality { get; set; } // e.g., "X-Ray"
        public string BodyPart { get; set; } // e.g., "Chest"
        public string Institution { get; set; }
    }

    public class RadiologistFindingsDto
    {
        public string Status { get; set; }
        public string RadiologistName { get; set; }
        public string ClinicalIndication { get; set; }
        public string FindingsText { get; set; }
        public string Impression { get; set; }
        public string ReportDate { get; set; }
    }

    public class PhysicianNotesDto
    {
        public string PhysicianName { get; set; }
        public string Notes { get; set; }
    }


    public class PatientPrescriptionDashboardDto
    {
        // Top Metric Cards
        public int ActivePrescriptions { get; set; }
        public int ExpiringSoon { get; set; }
        public int RenewedThisMonth { get; set; }
        public int TotalMedications { get; set; }

        // Main Table
        public List<MedicationItemDto> Medications { get; set; } = new();

        // Right Sidebar Widgets
        public List<RenewalReminderDto> RenewalReminders { get; set; } = new();
        public List<MedicationTipDto> MedicationTips { get; set; } = new();
    }

    public class MedicationItemDto
    {
        public Guid Id { get; set; }
        public Guid PrescriptionId { get; set; }
        public string MedicationName { get; set; }
        public string PrescribedBy { get; set; }
        public DateTime Date { get; set; }
        public string Duration { get; set; } // e.g., "90 Days", "2 Weeks"
        public string Status { get; set; }   // "Active", "Expiring", "Completed"
    }

    public class RenewalReminderDto
    {
        public Guid MedicationId { get; set; }
        public string MedicationName { get; set; }
        public string ExpiresInText { get; set; } // e.g., "Expires in 7 days"
    }

    public class MedicationTipDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }


    public class PrescriptionDetailsDto
    {
        public Guid PrescriptionId { get; set; }

        public PrescriptionSummaryDto Summary { get; set; }
        public string DoctorInstructions { get; set; }
        public List<PrescribedMedicationDto> Medications { get; set; } = new();
        public List<RefillHistoryItemDto> RefillHistory { get; set; } = new();
        public List<MedicationWarningDto> Warnings { get; set; } = new();
    }

    public class PrescriptionSummaryDto
    {
        public string PatientName { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string PatientIdDisplay { get; set; }

        public string IssueDate { get; set; }
        public string ValidUntil { get; set; }
        public string PrescribingDoctor { get; set; }
    }

    public class PrescribedMedicationDto
    {
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
    }

    public class RefillHistoryItemDto
    {
        public string Date { get; set; }
        public string PharmacyName { get; set; }
        public string Status { get; set; }
    }

    public class MedicationWarningDto
    {
        public string WarningType { get; set; } // e.g., "Drug Interaction", "Side Effects", "Monitoring"
        public string Description { get; set; }
    }
}
